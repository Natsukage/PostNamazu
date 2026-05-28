using GreyMagic;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace PostNamazu.Common
{
    /// <summary>
    ///   管理 GreyMagic FrameLock 的自动复用窗口，并在其中尝试连续同步执行调用。
    /// </summary>
    /// <remarks>
    ///   用于降低连续直接调用 CallInjected64 时因等待 hook 入口再次触发而产生的高延迟。<br />
    ///   短时间内的调用会尽量复用同一个 FrameLock 窗口；Execute 只等待当前操作完成。<br />
    ///   窗口到期、空闲或调度器释放后，放行入口函数的原始流程。
    /// </remarks>
    public sealed class GreyMagicCallScheduler : IDisposable
    {
        private sealed class ScheduledCall
        {
            public Action Action;
            public ManualResetEventSlim Done = new ManualResetEventSlim(false);
            public Exception Exception;
            internal ScheduledCall(Action action)
            {
                Action = action;
            }
        }

        private readonly ExternalProcessMemory _memory;
        private readonly object _gate = new object();
        private readonly Queue<ScheduledCall> _queue = new Queue<ScheduledCall>();

        /// <summary>
        ///   单个自动调度 FrameLock 的最大持有时间 (ms)。<br />
        ///   如果到达时间上限时仍有调用正在执行，会等待当前调用完成后再释放 FrameLock。
        /// </summary>
        private readonly int _frameLeaseMs;

        private bool _workerRunning;
        private bool _disposed;

        /// <summary>
        /// 用于在 Dispose 时等待当前 worker 线程完成，
        /// 避免过早释放调度器导致 worker 在执行过程中访问已释放的资源。
        /// </summary>
        private readonly ManualResetEventSlim _workerFinished = new ManualResetEventSlim(true);

        /// <summary>
        ///   当前线程正在执行的调度器实例，用于识别 Action 内部的重入调用，避免重新入队导致 worker 等待自己。
        /// </summary>
        [ThreadStatic]
        private static GreyMagicCallScheduler _currentWorkerScheduler;

        internal GreyMagicCallScheduler(ExternalProcessMemory memory, int frameLeaseMs = 10)
        {
            if (frameLeaseMs <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameLeaseMs));

            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _frameLeaseMs = frameLeaseMs;
        }

        /// <summary>
        ///   入口，同步执行一个需要进入 GreyMagic FrameLock 的操作。
        /// </summary>
        /// <remarks>
        ///   调用方会等待当前操作执行完成并取得异常结果，但不会等待整个 FrameLock 窗口释放。<br />
        ///   如果当前没有活动的调度窗口，本次调用会开启一个新的 FrameLock 窗口。<br />
        ///   如果已有 FrameLock 窗口仍在有效期内，本次调用会复用该窗口，从而与前后的短间隔调用落在同一次被捕获的 hook 调用中。<br />
        ///   FrameLock 会在到期、队列空闲或调度器释放后结束，放行被 hook 函数的原始流程。<br />
        ///   如果操作内部再次通过本调度器发起调用，会在当前 FrameLock 窗口内直接执行。
        /// </remarks>
        public void Execute(Action action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            // 如果当前已经在调度器的工作线程中，说明这是 Action 内部的重入调用。
            // 这里必须直接执行，否则会把新调用重新入队，然后工作线程等待自己处理队列，造成死锁。
            if (_currentWorkerScheduler == this)
            {
                action();
                return;
            }

            var call = new ScheduledCall(action);
            EnqueueCall(call);
            WaitCall(call);
        }

        /// <summary>
        ///   将调用加入调度队列，并确保工作线程已经启动。
        /// </summary>
        /// <remarks>
        ///   如果当前没有工作线程，会启动一轮新的 FrameLock 调度。<br />
        ///   如果工作线程正在等待新队列项，会通过 PulseAll 唤醒它继续处理。
        /// </remarks>
        private void EnqueueCall(ScheduledCall call)
        {
            lock (_gate)
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(GreyMagicCallScheduler));

                // 将 ScheduledCall 交给工作线程，工作线程在其持有的 FrameLock 窗口内依次执行队列项
                _queue.Enqueue(call);

                // 如果当前没有工作线程，启动一轮新的 FrameLock 调度
                if (!_workerRunning)
                {
                    _workerRunning = true;
                    _workerFinished.Reset();
                    StartWorkerThread();
                }

                // 尝试唤醒已有工作线程在 TryTakeNextCall 中 Monitor.Wait 的等待
                Monitor.PulseAll(_gate);
            }
        }

        /// <summary>
        ///   等待指定调用执行完成，并将工作线程中捕获的异常重新抛回调用方。
        /// </summary>
        /// <remarks>
        ///   Execute 对外保持同步语义：调用方只等待自己的 Action 完成，不等待整个 FrameLock 窗口释放。<br />
        ///   等待结束后会释放该调用项的 Done 句柄。
        /// </remarks>
        private void WaitCall(ScheduledCall call)
        {
            try
            {
                call.Done.Wait();

                if (call.Exception != null)
                    ExceptionDispatchInfo.Capture(call.Exception).Throw();
            }
            finally
            {
                call.Done.Dispose();
            }
        }

        /// <summary>
        ///   启动工作线程执行调度任务。
        /// </summary>
        private void StartWorkerThread()
        {
            var thread = new Thread(WorkerProc)
            {
                IsBackground = true,
                Name = "PostNamazu GreyMagicCallScheduler"
            };

            thread.Start();
        }

        /// <summary>
        ///   工作线程入口，在一个由调度器持有的 FrameLock 窗口内依次执行已入队的调用。
        /// </summary>
        /// <remarks>
        ///   FrameLock 到期、队列空闲或调度器释放时结束当前 FrameLock。<br />
        ///   如果工作线程本身发生异常，会将异常传播给尚未执行的队列项。
        /// </remarks>

        private void WorkerProc()
        {
            _currentWorkerScheduler = this;

            try
            {
                using (_memory.AcquireFrame(true))
                {
                    var frameDeadline = DateTime.UtcNow.AddMilliseconds(_frameLeaseMs);

                    // 工作线程的主循环
                    while (TryTakeNextCall(frameDeadline, out ScheduledCall call))
                    {
                        SafeExecuteCall(call);

                        if (DateTime.UtcNow >= frameDeadline)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                FailQueuedCalls(ex);
            }
            finally
            {
                FinalizeWorkerAndRestartIfNeeded();
            }
        }

        /// <summary>
        ///   尝试从队列中取得下一个需要执行的调用。
        /// </summary>
        /// <remarks>
        ///   如果队列为空，会等待直到有新调用入队、调度器释放或当前 FrameLock 到期。<br />
        ///   返回 false 表示当前 FrameLock 应结束，不再继续等待新调用。
        /// </remarks>
        private bool TryTakeNextCall(DateTime frameDeadline, out ScheduledCall call)
        {
            call = null;

            lock (_gate)
            {
                while (true)
                {
                    if (_disposed)
                        return false;

                    var remaining = frameDeadline - DateTime.UtcNow;
                    if (remaining <= TimeSpan.Zero)
                        return false;

                    if (_queue.Count > 0)
                    {
                        call = _queue.Dequeue();
                        return true;
                    }

                    remaining = frameDeadline - DateTime.UtcNow;
                    if (remaining <= TimeSpan.Zero)
                        return false;

                    Monitor.Wait(_gate, remaining);
                }
            }
        }

        /// <summary>
        ///   执行单个 <see cref="ScheduledCall"/> 调用，记录异常，并将执行结果通知等待方。
        /// </summary>
        private void SafeExecuteCall(ScheduledCall call)
        {
            try
            {
                call.Action();
            }
            catch (Exception ex)
            {
                call.Exception = ex;
            }
            finally
            {
                call.Done.Set();
            }
        }

        /// <summary>
        ///   将指定异常传播给所有尚未执行的队列调用，用于工作线程自身发生异常时清空剩余队列。
        /// </summary>
        private void FailQueuedCalls(Exception ex)
        {
            List<ScheduledCall> calls;

            lock (_gate)
            {
                calls = DrainQueueNoLock();
            }

            foreach (var call in calls)
            {
                call.Exception = ex;
                call.Done.Set();
            }
        }

        /// <summary>
        ///   收尾当前工作线程，并在必要时启动新的工作线程。
        /// </summary>
        /// <remarks>
        ///   如果当前 FrameLock 结束时队列中仍有请求，会立即启动新的 worker 处理下一轮 FrameLock。
        /// </remarks>
        private void FinalizeWorkerAndRestartIfNeeded()
        {
            // 当前 worker 已经终止调度任务，清除当前线程上的调度器标记，防止误判重入调用
            // 如果 Action 内部再次调用 Call，应直接执行而不是重新入队，避免 worker 等待自己
            _currentWorkerScheduler = null;

            var shouldRestart = false;

            lock (_gate)
            {
                _workerRunning = false;

                // 如果此时队列里还有请求，说明这些请求没有被当前 FrameLock 处理完，需要立刻重启 worker。
                // 可能是当前 frame 到期后退出，或 worker 退出后又有新的请求入队。
                if (!_disposed && _queue.Count > 0)
                {
                    _workerRunning = true;
                    _workerFinished.Reset();
                    shouldRestart = true;
                }
                else
                {
                    _workerFinished.Set();
                }
            }

            if (shouldRestart)
                StartWorkerThread();
        }

        private List<ScheduledCall> DrainQueueNoLock()
        {
            var calls = new List<ScheduledCall>();

            while (_queue.Count > 0)
            {
                calls.Add(_queue.Dequeue());
            }

            return calls;
        }

        public void Dispose()
        {
            List<ScheduledCall> calls;
            bool shouldWaitWorker;

            lock (_gate)
            {
                if (_disposed) return;

                _disposed = true;
                calls = DrainQueueNoLock();
                shouldWaitWorker = _workerRunning && _currentWorkerScheduler != this;

                Monitor.PulseAll(_gate);
            }

            var ex = new ObjectDisposedException(nameof(GreyMagicCallScheduler));

            foreach (var call in calls)
            {
                call.Exception = ex;
                call.Done.Set();
            }

            // 如果有 worker 正在执行，并且当前不是 worker 线程，等待它自然退出。
            // 避免随后释放 Memory 时，worker 仍在使用 GreyMagic 资源。
            if (shouldWaitWorker)
                _workerFinished.Wait(1000);
        }
    }
}