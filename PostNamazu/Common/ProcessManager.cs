using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace PostNamazu.Common
{
    /// <summary>
    /// FFXIV进程管理器
    /// </summary>
    public class ProcessManager
    {
        private readonly PostNamazu _plugin;
        private BackgroundWorker _processSwitcher;

        public ProcessManager(PostNamazu plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
        }

        /// <summary>
        /// 开始进程监控
        /// </summary>
        public void StartProcessMonitoring()
        {
            _processSwitcher = new BackgroundWorker { WorkerSupportsCancellation = true };
            _processSwitcher.DoWork += ProcessSwitcher;
            _processSwitcher.RunWorkerAsync();
        }

        /// <summary>
        /// 停止进程监控
        /// </summary>
        public void StopProcessMonitoring()
        {
            _processSwitcher?.CancelAsync();
        }

        /// <summary>
        /// 获取FFXIV进程
        /// </summary>
        /// <returns>FFXIV进程或null</returns>
        public Process GetFFXIVProcess()
        {
            if (_plugin.FFXIV_ACT_Plugin?.DataRepository == null) return null;
            return _plugin.FFXIV_ACT_Plugin.DataRepository.GetCurrentFFXIVProcess();
        }

        /// <summary>
        /// 进程切换监控循环
        /// </summary>
        private void ProcessSwitcher(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (_processSwitcher.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                try
                {
                    var currentXiv = GetFFXIVProcess();
                    if (_plugin.FFXIV != currentXiv)
                    {
                        HandleProcessChange(currentXiv);
                    }
                    else if (_plugin.FFXIV == null && _plugin.State != PostNamazu.StateEnum.NotReady)
                    {
                        _plugin.SetState(PostNamazu.StateEnum.NotReady);
                    }
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("集合已修改") || ex.Message.Contains("Collection was modified"))
                {
                    // 重构修正：集合修改异常通常是并发访问导致，不是真正的进程注入错误
                    // 记录调试信息但不显示给用户，静默重试即可
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"ProcessSwitcher: 集合访问异常 - {ex.Message}");
#endif
                }
                catch (Exception ex)
                {
                    // 只有真正的进程相关错误才记录为进程注入异常
                    ExceptionHandler.HandleProcessInjectionException(ex, _plugin.PluginUi);
                    _plugin.SetState(PostNamazu.StateEnum.Failure);
                    _plugin.Detach();
                }

                Thread.Sleep(Constants.ProcessSwitchInterval);
            }
        }

        /// <summary>
        /// 处理进程变更
        /// </summary>
        private void HandleProcessChange(Process newProcess)
        {
            _plugin.SetState(PostNamazu.StateEnum.Waiting);
            _plugin.Detach();
            _plugin.FFXIV = newProcess;

            if (_plugin.FFXIV == null)
            {
                _plugin.SetState(PostNamazu.StateEnum.NotReady);
            }
            else if (_plugin.GetOffsets())
            {
                _plugin.Attach();
            }
            else
            {
                _plugin.SetState(PostNamazu.StateEnum.Failure);
            }
        }
    }
} 