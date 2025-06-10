using System;
using System.Diagnostics;
using PostNamazu.Common;
using PostNamazu.Common.Localization;
using GreyMagic;
using System.Threading;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace PostNamazu.Actions
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
{
    public abstract class NamazuModule
    {
        protected static PostNamazu PostNamazu => PostNamazu.Plugin;
        protected static FFXIV_ACT_Plugin.FFXIV_ACT_Plugin FFXIV_ACT_Plugin => PostNamazu?.FFXIV_ACT_Plugin;
        protected static Process FFXIV => PostNamazu?.FFXIV;
        protected static ExternalProcessMemory Memory => PostNamazu?.Memory;
        protected static PostNamazuUi PluginUI => PostNamazu?.PluginUi;
        protected static SigScanner SigScanner => PostNamazu?.SigScanner;

        public static bool IsPluginReady => FFXIV_ACT_Plugin != null && PostNamazu.State == PostNamazu.StateEnum.Ready;
        
        
        protected bool complaintAboutModuleNotReady = false;

        protected PostNamazu.StateEnum _state = PostNamazu.StateEnum.NotReady;
        public PostNamazu.StateEnum State
        {
            get => _state;
            internal set
            {
                _state = value;
                PluginUI.UpdateActionColorByState(GetType().Name, _state);
#if DEBUG
                PluginUI.Log($"{GetType().Name} 模组状态变更：{value}");
#endif
            }
        }

        public void Setup()
        {
            try {
                State = PostNamazu.StateEnum.Waiting;
                GetOffsets();
                State = PostNamazu.StateEnum.Ready;
            }
            catch (Exception ex) {
                PluginUI?.Log(L.Get("PostNamazu/getOffsetsFail", GetType().Name, ex.Message + " \n" + ex.StackTrace));
                State = PostNamazu.StateEnum.Failure;
            }
            //Log("初始化完成");
        }

        public virtual void GetOffsets()
        {

        }

        public void Log(string msg)
        {
            PluginUI?.Log(msg);
        }

        /// <summary> 检查插件和模组是否准备就绪，若不是则抛出异常，避免在错误的地址调用函数导致游戏崩溃。</summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="IgnoredException">模组初始化失败的情况下非首次报错，忽略此异常。</exception>
        public void CheckBeforeExecution()
        {
            if (!IsPluginReady)
            {
                throw new Exception(L.Get("PostNamazu/xivProcNotFound"));
            }
            if (State == PostNamazu.StateEnum.NotReady)
            {
                throw new Exception(L.Get("PostNamazu/moduleNotReady", GetType().Name));
            }
            var count = 0;
            // 在已扫描到游戏进程到模组初始化完成的期间，延迟等待动作直至模组就绪（或就绪失败）
            // ACT 晚于游戏进程启动，且触发器中有进入游戏时立刻执行的指令时，会出现此情况
            while (State == PostNamazu.StateEnum.Waiting)
            {
                count++;
#if DEBUG
                Log($"{GetType().Name} 模组未就绪，正在等待第 {count} / {Constants.ModuleInitMaxWaitCount} 次…");
#endif
                System.Threading.Thread.Sleep(Constants.ModuleInitWaitInterval);
                if (count > Constants.ModuleInitMaxWaitCount)
                {
                    // 不应进入此分支，进入此分支说明 State 由于程序逻辑问题而错误地保持在 Waiting 状态
                    Log($"{GetType().Name} 模组长期未能初始化，已跳过。"); 
                    State = PostNamazu.StateEnum.Failure;
                }
            }
            if (State == PostNamazu.StateEnum.Failure)
            {
                var noModuleMsg = L.Get("PostNamazu/moduleInitFail", GetType().Name);
                // 对于模组初始化失败的情况，只抛出一次异常，之后忽略
                if (complaintAboutModuleNotReady)
                {
                    throw new IgnoredException(noModuleMsg);
                }
                else
                {
                    complaintAboutModuleNotReady = true;
                    throw new Exception(noModuleMsg);
                }
            }
            else // Ready
                 complaintAboutModuleNotReady = false; 
        }

        /// <summary> 检查插件和模组是否准备就绪，且指令是否非空，若不是则抛出异常。</summary>
        public void CheckBeforeExecution(string command)
        {
            CheckBeforeExecution();
            if (string.IsNullOrWhiteSpace(command))
                throw new Exception(L.Get("PostNamazu/emptyCommand"));
        }



        internal class IgnoredException : Exception {
            internal IgnoredException(string msg) : base(msg) { }
        }

        public static void ExecuteWithLock(Action action)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(Memory.Executor.AssemblyLock, ref lockTaken);
                action();
            }
            finally 
            { 
                if (lockTaken) Monitor.Exit(Memory.Executor.AssemblyLock); 
            }
        }

        public static T ExecuteWithLock<T>(Func<T> function)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(Memory.Executor.AssemblyLock, ref lockTaken);
                return function();
            }
            finally 
            { 
                if (lockTaken) Monitor.Exit(Memory.Executor.AssemblyLock); 
            }
        }
    }

}


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace PostNamazu.Attributes
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        public string Command { get; }

        public CommandAttribute(string command)
        {
            Command = command;
        }
    }
}
