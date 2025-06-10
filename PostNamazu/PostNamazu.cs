using Advanced_Combat_Tracker;
using GreyMagic;
using PostNamazu.Actions;
using PostNamazu.Attributes;
using PostNamazu.Common;
using PostNamazu.Common.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PostNamazu
{
    public class PostNamazu : IActPluginV1
    {
        public PostNamazu()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public static PostNamazu Plugin;
        internal PostNamazuUi PluginUi;
        private Label _lblStatus; // The status label that appears in ACT's Plugin tab

        private ProcessManager _processManager;
        private PluginIntegrationManager _integrationManager;

        private HttpServer _httpServer;

        internal Process FFXIV;
        internal FFXIV_ACT_Plugin.FFXIV_ACT_Plugin FFXIV_ACT_Plugin;
        public ExternalProcessMemory Memory;
        public SigScanner SigScanner;

        private IntPtr _entrancePtr;
        public Dictionary<string, bool> ActionEnabled => PluginUi.ActionEnabled; //直接使用UI控件上的ActionEnabled状态
        private readonly Dictionary<string, HandlerDelegate> CmdBind = new(StringComparer.OrdinalIgnoreCase); //key不区分大小写

        private readonly List<NamazuModule> Modules = new();

        /// <summary> 插件或模组的当前状态。 </summary>
        public enum StateEnum 
        { 
            /// <summary> 尚未开始。 </summary>
            NotReady,
            /// <summary> 扫描失败。 </summary>
            Failure,
            /// <summary> 正在启动或尝试扫描。 </summary>
            Waiting,
            /// <summary> 扫描成功，已预备。 </summary>
            Ready
        };

        private StateEnum _state = StateEnum.Waiting;
        public StateEnum State 
        { 
            get => _state;
            private set => SetState(value);
        }

        /// <summary>
        /// 设置插件状态（供内部类使用）
        /// </summary>
        internal void SetState(StateEnum value)
        {
            _state = value;
#if DEBUG
            PluginUI?.Log($"插件状态变更：{value}");
#endif
        }

        #region Init
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            Plugin = this;
            _lblStatus = pluginStatusText;

            PluginUi = new PostNamazuUi();
            pluginScreenSpace.Controls.Add(PluginUi);
            pluginScreenSpace.Text = L.Get("PostNamazu/title");
            
            PluginUi.Log(L.Get("PostNamazu/pluginVersion", Assembly.GetExecutingAssembly().GetName().Version));

            FFXIV_ACT_Plugin = GetFFXIVPlugin();

            // 初始化管理器
            _processManager = new ProcessManager(this);
            _integrationManager = new PluginIntegrationManager(this);

            //目前解析插件有bug，在特定情况下无法正常触发ProcessChanged事件。因此只能通过后台线程实时监控
            //FFXIV_ACT_Plugin.DataSubscription.ProcessChanged += ProcessChanged;
            _processManager.StartProcessMonitoring();

            if (PluginUi.AutoStart)
                ServerStart();
            PluginUi.ButtonStart.Click += ServerStart;
            PluginUi.ButtonStop.Click += ServerStop;

            InitializeActions();
            _integrationManager.InitializeIntegrations();

            Assembly.Load("GreyMagic"); // 直接加载而非首次调用时延迟加载，防止没开启游戏而没调用 GreyMagic 初始化 Memory 时其他插件找不到 GreyMagic

            _lblStatus.Text = L.Get("PostNamazu/pluginInit");
            LogACT("Initialized");
        }

        public void DeInitPlugin()
        {
            //FFXIV_ACT_Plugin.DataSubscription.ProcessChanged -= ProcessChanged;
            PluginUi.SaveSettings();
            Detach();
            _integrationManager?.DeInitializeIntegrations();
            if (_httpServer != null) ServerStop();
            _processManager?.StopProcessMonitoring();
            
            _lblStatus.Text = L.Get("PostNamazu/pluginDeInit");
            Plugin = null;
        }

        public delegate void HandlerDelegate(string command);

        /// <summary>
        ///     注册命令
        /// </summary>
        public void InitializeActions()
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(NamazuModule)) && !t.IsAbstract)) {
#if DEBUG
                PluginUI.Log($"Initalizing Module: {t.Name}");
#endif
                var module = (NamazuModule)Activator.CreateInstance(t);
                Modules.Add(module);
                PluginUi.RegisterAction(t.Name);
                var commands = module.GetType().GetMethods().Where(method => method.GetCustomAttributes<CommandAttribute>().Any());
                foreach (var action in commands) {
                    var handlerDelegate = (HandlerDelegate)Delegate.CreateDelegate(typeof(HandlerDelegate), module, action);
                    foreach (var command in action.GetCustomAttributes<CommandAttribute>())
                    {
                        SetAction(command.Command, handlerDelegate);
#if DEBUG
                        PluginUI.Log($"{action.Name}@{command.Command}");
#endif
                    }
                    
                }
            }
        }

        public T GetModuleInstance<T>() where T : NamazuModule
        {
            return (T)Modules.FirstOrDefault(m => m is T);
        }

        /// <summary>
        /// 获取所有命令键（供集成管理器使用）
        /// </summary>
        internal string[] GetCommandKeys()
        {
            return CmdBind.Keys.ToArray();
        }

        private void ServerStart(object sender = null, EventArgs e = null)
        {
            try {
                _httpServer = new HttpServer((int)PluginUi.TextPort.Value)
                {
                    PostNamazuDelegate = DoAction
                };
                _httpServer.OnException += OnException;

                PluginUi.ButtonStart.Enabled = false;
                PluginUi.ButtonStop.Enabled = true;
                PluginUi.Log(L.Get("PostNamazu/httpStart", _httpServer.Port));
            }
            catch (Exception ex) {
                OnException(ex);
            }
        }

        private void ServerStop(object sender = null, EventArgs e = null)
        {
            if (_httpServer != null)
            {
                _httpServer.Stop();
                _httpServer.PostNamazuDelegate = null;
                _httpServer.OnException -= OnException;
            }

            PluginUi.ButtonStart.Enabled = true;
            PluginUi.ButtonStop.Enabled = false;
            PluginUi.Log(L.Get("PostNamazu/httpStop"));
        }

        /// <summary>
        /// 委托给HttpServer类的异常处理
        /// </summary>
        /// <param name="ex"></param>
        private void OnException(Exception ex)
        {
            ExceptionHandler.HandleHttpServerException(ex, _httpServer.Port, PluginUi,
                () => PluginUi.ButtonStart.Enabled = true,
                () => PluginUi.ButtonStop.Enabled = false);
        }

        #endregion

        #region Memory and Process Management

        internal void Attach()
        {
            if (FFXIV?.HasExited != false) return;
            try {
                Memory = new ExternalProcessMemory(FFXIV, true, false, _entrancePtr, false, 5, true);
                PluginUi.Log(L.Get("PostNamazu/xivProcInject", FFXIV.Id));
                State = StateEnum.Ready;
                LogACT("Attached");

                foreach (var m in Modules)
                {
                    m.State = StateEnum.Waiting;
                }
                _frameworkPtrPtr = IntPtr.Zero;
                _isCN = null;
                GetRegion();
                foreach (var m in Modules)
                {
                    m.Setup();
                }
                LogACT("ModulesInitialized");
            }
            catch (Exception ex) {
                PluginUi.Log(L.Get("PostNamazu/xivProcInjectFailWithError", FFXIV.Id, ex.Message + " \n" + ex.StackTrace));
                FFXIV = null;
                State = StateEnum.Failure;
            }
        }

        internal void Detach()
        {
            FFXIV = null;
            _frameworkPtrPtr = IntPtr.Zero;
            foreach (var m in Modules)
            {
                m.State = StateEnum.NotReady;
            }
            try 
            {
                if (Memory != null && !Memory.Process.HasExited)
                    Memory.Dispose();
            }
            catch (Exception) {
                // ignored
            }
        }

        private FFXIV_ACT_Plugin.FFXIV_ACT_Plugin GetFFXIVPlugin()
        {
            var plugin = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.pluginObj?.GetType()?.ToString() == "FFXIV_ACT_Plugin.FFXIV_ACT_Plugin")?.pluginObj;
            return (FFXIV_ACT_Plugin.FFXIV_ACT_Plugin)plugin 
                ?? throw new Exception(L.Get("PostNamazu/parserNotFound"));
        }

        internal bool GetOffsets()
        {
            PluginUi.Log(L.Get("PostNamazu/sigScanning"));
            SigScanner = new SigScanner(FFXIV);
            try {
                _entrancePtr = SigScanner.ScanText("4C 8B DC 56 41 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 48 83 B9 ? ? ? ? ? 4C 8B FA"); //7.0
                _frameworkPtrPtr = IntPtr.Zero;
                _isCN = null;
                GetRegion();
                return true;
            }
            catch (ArgumentException) {
                //PluginUI.Log(L.Get("PostNamazu/xivProcInjectFail"));
            }

            try {
                _entrancePtr = SigScanner.ScanText("E9 * * * * 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 83 B9");
                _frameworkPtrPtr = IntPtr.Zero;
                _isCN = null;
                GetRegion();
                return true;
            }
            catch (ArgumentException) {
                PluginUi.Log(L.Get("PostNamazu/xivProcInjectFail"));
            }
            return false;
        }

        #endregion

        #region Region Detection

        internal static bool _playerDetected = false;
        internal static bool? _isCN = null;
        public bool IsCN
        {
            get
            {
                if (!_isCN.HasValue) GetRegion();
                return _isCN ?? false;
            }
        }

        private void GetRegion()
        {
            try {
                GetRegionByMemory();
            }
            catch (Exception ex) {
                ExceptionHandler.HandleRegionDetectionException(ex, PluginUi);
            }
        }

        private void GetRegionByMemory()
        {
            if (FrameworkPtrPtr != IntPtr.Zero) // scanning FrameworkPtrPtr
            {
                byte language;
                try
                {
                    language = Memory.Read<byte>(FrameworkPtr + 0x580);
                }
                catch
                {
                    ExceptionHandler.HandleMemoryReadException();
                    // 重构修正：内存读取失败时静默返回，避免显示错误信息
                    return;
                }
                bool? result = language switch
                {
                    0 or 1 or 2 or 3 => false,
                    4 => true,
                    _ => null,
                };
                if (result.HasValue)
                {
                    _isCN = result;
                    PluginUi.Log(_isCN.Value
                        ? L.Get("PostNamazu/xivDetectMemRegionCN")
                        : L.Get("PostNamazu/xivDetectMemRegionGlobal")
                    );
                }
                else _isCN = false; // default
            }
        }

        private IntPtr _frameworkPtrPtr = IntPtr.Zero;
        public IntPtr FrameworkPtrPtr
        {
            get
            {
                if (_frameworkPtrPtr != IntPtr.Zero)
                {
                    return _frameworkPtrPtr;
                }
                try // 7.0 CN
                {
                    _frameworkPtrPtr = SigScanner.ScanText("49 8B C4 48 8B 0D ? ? ? ? 48 8D 15 ? ? ? ? 48 89 05 * * * *", nameof(_frameworkPtrPtr));
                    return _frameworkPtrPtr;
                }
                catch { }
                try // 7.0 global
                {
                    _frameworkPtrPtr = SigScanner.ScanText("49 8B DC 48 89 1D * * * *", nameof(_frameworkPtrPtr));
                    return _frameworkPtrPtr;
                }
                catch (Exception ex)
                {
                    PluginUi.Log(L.Get("PostNamazu/xivFrameworkNotFound", ex.Message));
                    return IntPtr.Zero;
                }
            }
        }

        public IntPtr FrameworkPtr
        {
            get
            {
                try
                {
                    return Memory.Read<IntPtr>(FrameworkPtrPtr);
                }
                catch
                {
                    ExceptionHandler.HandleMemoryReadException();
                    // 重构修正：内存读取失败时返回零指针，避免抛出异常
                    return IntPtr.Zero;
                }
            }
        }

        private void LogRegion()
        {
            if (!_isCN.HasValue) return;
            PluginUi.Log(_isCN.Value
                ? L.Get("PostNamazu/xivDetectRegionCN")
                : L.Get("PostNamazu/xivDetectRegionGlobal")
            );
        }

        #endregion

        #region Logging

        internal void LogACT(string msg)
        {
            var log = $"00|{DateTime.Now:O}|FFFF|{Constants.PluginName}|{msg}|0000000000000000";
            ActGlobals.oFormActMain.ParseRawLogLine(false, DateTime.Now, log);
        }

        #endregion

        #region Obsolete Methods

        /// <summary>
        ///     解析插件对应进程改变时触发，解除当前注入并注入新的游戏进程
        ///     目前由于解析插件的bug，ProcessChanged事件无法正常触发，暂时弃用。
        /// </summary>
        /// <param name="tProcess"></param>
        [Obsolete]
        private void ProcessChanged(Process tProcess)
        {
            if (tProcess.Id != FFXIV?.Id) {
                Detach();
                FFXIV = tProcess;
                if (FFXIV != null)
                    if (GetOffsets())
                        Attach();
                L.Get("PostNamazu/xivProcSwitch", tProcess.Id);
            }
        }

        /// <summary>
        ///     AssemblyResolve事件的处理函数，该函数用来自定义程序集加载逻辑
        ///     GrayMagic也打包了，已经不需要再从外部加载dll了
        /// </summary>
        /// <param name="sender">事件引发源</param>
        /// <param name="args">事件参数，从该参数中可以获取加载失败的程序集的名称</param>
        /// <returns></returns>
        [Obsolete]
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(',')[0];
            //if (name != "GreyMagic") return null;
            switch (name) {
                case "GreyMagic":
                    var selfPluginData = ActGlobals.oFormActMain.PluginGetSelfData(this);
                    var path = selfPluginData.pluginFile.DirectoryName;
                    return Assembly.LoadFile($@"{path}\{name}.dll");
                default:
                    return null;
            }
        }
        #endregion

        #region Delegate

        /// <summary>
        ///     执行指令对应的方法
        /// </summary>
        /// <param name="command"></param>
        /// <param name="payload"></param>
        public void DoAction(string command, string payload)
        {
            try
            {
                var reflectedType = GetAction(command).GetMethodInfo().ReflectedType!.Name;

                if (ActionEnabled.ContainsKey(reflectedType) && ActionEnabled[reflectedType]) //不响应没有启用的动作
                    GetAction(command)(payload);
                else
                    PluginUi.Log(L.Get("PostNamazu/actionIgnored", command, payload));
            }
            catch (NamazuModule.IgnoredException) { }
            catch (Exception ex)
            {
                ExceptionHandler.HandleActionExecutionException(ex, command, PluginUi);
            }
        }

        /// <summary>
        ///     设置指令与对应的方法
        /// </summary>
        /// <param name="command">指令类型</param>
        /// <param name="action">对应指令的方法委托</param>
        public void SetAction(string command, HandlerDelegate action)
        {
            CmdBind[command] = action;
        }

        /// <summary>
        ///     获取指令对应的方法
        /// </summary>
        /// <param name="command">指令类型</param>
        /// <returns>对应指令的委托方法</returns>
        private HandlerDelegate GetAction(string command)
        {
            try {
                return CmdBind[command];
            }
            catch {
                throw new Exception(L.Get("PostNamazu/actionNotFound", command));
            }
        }

        /// <summary>
        ///     清空绑定的委托列表
        /// </summary>
        public void ClearAction()
        {
            CmdBind.Clear();
        }
        #endregion
    }
}