using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PostNamazu.Actions;
using PostNamazu.Attributes;
using PostNamazu.Common;
using Advanced_Combat_Tracker;
using GreyMagic;

namespace PostNamazu
{
    public class PostNamazu : IActPluginV1
    {
        public PostNamazu()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        internal PostNamazuUi PluginUI;
        private Label _lblStatus; // The status label that appears in ACT's Plugin tab

        private BackgroundWorker _processSwitcher;

        private HttpServer _httpServer;
        private OverlayHoster.Program _overlayHoster;
        private TriggerHoster.Program _triggerHoster;

        internal Process FFXIV;
        internal FFXIV_ACT_Plugin.FFXIV_ACT_Plugin FFXIV_ACT_Plugin;
        public ExternalProcessMemory Memory;
        public SigScanner SigScanner;

        private IntPtr _entrancePtr;
        public Dictionary<string, bool> ActionEnabled => PluginUI.ActionEnabled; //直接使用UI控件上的ActionEnabled状态
        private Dictionary<string, HandlerDelegate> CmdBind = new(StringComparer.OrdinalIgnoreCase); //key不区分大小写

        private List<NamazuModule> Modules = new();

        #region Init
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            _lblStatus = pluginStatusText;

            PluginUI = new PostNamazuUi();
            pluginScreenSpace.Controls.Add(PluginUI);
            pluginScreenSpace.Text = I18n.Translate("PostNamazu", "鲶鱼精邮差");
            PluginUI.Log(I18n.Translate("PostNamazu/PluginVersion", "插件版本：{0}。", Assembly.GetExecutingAssembly().GetName().Version));

            FFXIV_ACT_Plugin = GetFFXIVPlugin();


            //目前解析插件有bug，在特定情况下无法正常触发ProcessChanged事件。因此只能通过后台线程实时监控
            //FFXIV_ACT_Plugin.DataSubscription.ProcessChanged += ProcessChanged;
            _processSwitcher = new BackgroundWorker { WorkerSupportsCancellation = true };
            _processSwitcher.DoWork += ProcessSwitcher;
            _processSwitcher.RunWorkerAsync();

            if (PluginUI.AutoStart)
                ServerStart();
            PluginUI.ButtonStart.Click += ServerStart;
            PluginUI.ButtonStop.Click += ServerStop;

            InitializeActions();
            TriggIntegration();
            OverlayIntegration();

            _lblStatus.Text = I18n.Translate("PostNamazu/PluginInit", "鲶鱼精邮差已启动。");
        }

        public void DeInitPlugin()
        {
            //FFXIV_ACT_Plugin.DataSubscription.ProcessChanged -= ProcessChanged;
            PluginUI.SaveSettings();
            Detach();
            _overlayHoster.DeInit();
            _triggerHoster.DeInit();
            if (_httpServer != null) ServerStop();
            _processSwitcher.CancelAsync();
            
            _lblStatus.Text = I18n.Translate("PostNamazu/PluginDeInit", "鲶鱼精邮差已退出。");
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
                module.Init(this);
                Modules.Add(module);
                PluginUI.RegisterAction(t.Name);
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

        private void ServerStart(object sender = null, EventArgs e = null)
        {
            try {
                _httpServer = new HttpServer((int)PluginUI.TextPort.Value);
                _httpServer.PostNamazuDelegate = DoAction;
                _httpServer.OnException += OnException;

                PluginUI.ButtonStart.Enabled = false;
                PluginUI.ButtonStop.Enabled = true;
                PluginUI.Log(I18n.Translate("PostNamazu/HttpStart", "在 {0} 端口启动 HTTP 监听。", _httpServer.Port));
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

            PluginUI.ButtonStart.Enabled = true;
            PluginUI.ButtonStop.Enabled = false;
            PluginUI.Log(I18n.Translate("PostNamazu/HttpStop", "已停止 HTTP 监听。"));
        }

        /// <summary>
        /// 委托给HttpServer类的异常处理
        /// </summary>
        /// <param name="ex"></param>
        private void OnException(Exception ex)
        {
            string errorMessage = I18n.Translate("PostNamazu/HttpException", "无法在 {0} 端口启动监听：\n{1}", _httpServer.Port, ex.Message);

            PluginUI.ButtonStart.Enabled = true;
            PluginUI.ButtonStop.Enabled = false;

            PluginUI.Log(errorMessage);
            MessageBox.Show(errorMessage);
        }

        /// <summary>
        ///     对当前解析插件对应的游戏进程进行注入
        /// </summary>
        private void Attach()
        {
            try {
                Memory = new ExternalProcessMemory(FFXIV, true, false, _entrancePtr, false, 5, true);
                PluginUI.Log(I18n.Translate("PostNamazu/XivProcInject", "已找到 FFXIV 进程 {0}。", FFXIV.Id));
            }
            catch (Exception ex) {
                MessageBox.Show(
                    I18n.Translate("PostNamazu/XivProcInjectException", "注入 FFXIV 进程时发生错误！\n{0}", ex),
                    I18n.Translate("PostNamazu", "鲶鱼精邮差"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                Detach();
            }
            _frameworkPtrPtr = IntPtr.Zero;
            _isCN = null;
            GetRegion();
            foreach (var m in Modules)
            {
                m.Setup();
            }
        }

        /// <summary>
        ///     解除注入
        /// </summary>
        private void Detach()
        {
            try {
                if (Memory != null && !Memory.Process.HasExited)
                    Memory.Dispose();
            }
            catch (Exception) {
                // ignored
            }
        }

        /// <summary>
        ///     取得解析插件
        /// </summary>
        /// <returns></returns>
        private FFXIV_ACT_Plugin.FFXIV_ACT_Plugin GetFFXIVPlugin()
        {
            var plugin = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.pluginObj?.GetType().ToString() == "FFXIV_ACT_Plugin.FFXIV_ACT_Plugin")?.pluginObj;
            return (FFXIV_ACT_Plugin.FFXIV_ACT_Plugin)plugin 
                ?? throw new Exception(I18n.Translate("PostNamazu/ParserNotFound", "找不到 FFXIV 解析插件，请确保其加载顺序位于鲶鱼精邮差之前。"));
        }

        /// <summary>
        ///     取得解析插件对应的游戏进程
        /// </summary>
        /// <returns>解析插件当前对应进程</returns>
        private Process GetFFXIVProcess()
        {
            return FFXIV_ACT_Plugin.DataRepository.GetCurrentFFXIVProcess();
        }

        /// <summary>
        ///     获取几个重要的地址
        /// </summary>
        /// <returns>返回是否成功找到入口地址</returns>
        private bool GetOffsets()
        {
            PluginUI.Log(I18n.Translate("PostNamazu/SigScanning", "正在扫描内存特征……"));
            SigScanner = new SigScanner(FFXIV);
            try {
                _entrancePtr = SigScanner.ScanText("4C 8B DC 56 41 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 48 83 B9 ? ? ? ? ? 4C 8B FA"); //7.0
                return true;
            }
            catch (ArgumentOutOfRangeException) {
                //PluginUI.Log(I18n.Translate("PostNamazu/XivProcInjectFail", "无法注入当前进程，可能是已经被其他进程注入了，请尝试重启游戏。"));
            }

            try {
                _entrancePtr = SigScanner.ScanText("E9 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 83 B9");
                return true;
            }
            catch (ArgumentOutOfRangeException) {
                PluginUI.Log(I18n.Translate("PostNamazu/XivProcInjectFail", "无法注入当前进程，可能是已经被其他进程注入了，请尝试重启游戏。"));
            }
            return false;
        }

        internal static bool _playerDetected = false;
        internal static bool? _isCN = null;
        public bool IsCN
        {
            get
            {
                GetRegion();
                return _isCN.Value;
            }
        }

        private void GetRegion()
        {
            /*
            // Always try to detect the region by player name if it has not been done yet
            if (!_playerDetected)
            {
                GetRegionByPlayer();
            } */

            // If the region has not been detected by player name,
            // try to estimate it by memory (only run once when attached to each FFXIV process)
            if (_frameworkPtrPtr == IntPtr.Zero)
            {
                GetRegionByMemory();
            }
        }

        /* for future use if possible
        private void GetRegionByPlayer()
        {
            var myId = FFXIV_ACT_Plugin.DataRepository.GetCurrentPlayerID();
            var myName = FFXIV_ACT_Plugin.DataRepository.GetCombatantList().FirstOrDefault(c => c.ID == myId)?.Name;
            bool? result = !myName?.Contains(" ");
            if (result.HasValue)
            {
                _playerDetected = true;
                PluginUI.Log(_isCN.Value
                    ? I18n.Translate("PostNamazu/XivDetectRegionCN", "已设置为国服配置（根据角色名称判断）。")
                    : I18n.Translate("PostNamazu/XivDetectRegionGlobal", "已设置为国际服配置（根据角色名称判断）。")
                );
            }
            _isCN = result ?? _isCN;
        } */

        private void GetRegionByMemory()
        {
            if (FrameworkPtrPtr != IntPtr.Zero) // scanning FrameworkPtrPtr
            {
                byte language = Memory.Read<byte>(FrameworkPtr + 0x580);
                bool? result = language switch
                {
                    0 or 1 or 2 or 3 => false,
                    4 => true,
                    _ => null,
                };
                if (result.HasValue)
                {
                    _isCN = result;
                    PluginUI.Log(_isCN.Value
                        ? I18n.Translate("PostNamazu/XivDetectMemRegionCN", "已设置为国服配置（根据内存信息判断）。")
                        : I18n.Translate("PostNamazu/XivDetectMemRegionGlobal", "已设置为国际服配置（根据内存信息判断）。")
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
                try
                {   // 7.0 CN
                    var sigStart = SigScanner.ScanText("49 8B C4 48 8B 0D");
                    _frameworkPtrPtr = sigStart + 24 + Memory.Read<int>(sigStart + 20);
                    return _frameworkPtrPtr;
                } 
                catch { }
                try
                {   // 7.0 global
                    var sigStart = SigScanner.ScanText("49 8B DC 48 89 1D");
                    _frameworkPtrPtr = sigStart + 10 + Memory.Read<int>(sigStart + 6);
                    return _frameworkPtrPtr;
                } 
                catch 
                {
                    PluginUI.Log(I18n.Translate("PostNamazu/XivFrameworkNotFound", "未找到 Framework 的内存签名，部分功能无法使用，可能需要更新插件版本。"));
                    return IntPtr.Zero; 
                }
            }
        }
        public IntPtr FrameworkPtr => Memory.Read<IntPtr>(FrameworkPtrPtr);

        private void LogRegion()
        {
            if (!_isCN.HasValue) return;
            PluginUI.Log(_isCN.Value
                ? I18n.Translate("PostNamazu/XivDetectRegionCN", "已设置为国服。")
                : I18n.Translate("PostNamazu/XivDetectRegionGlobal", "已设置为国际服。")
            );
        }

        /// <summary>
        ///     代替ProcessChanged委托，手动循环检测当前活动进程并进行注入。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessSwitcher(object sender, DoWorkEventArgs e)
        {
            while (true) {
                if (_processSwitcher.CancellationPending) {
                    e.Cancel = true;
                    break;
                }

                if (FFXIV != GetFFXIVProcess()) {
                    Detach();
                    FFXIV = GetFFXIVProcess();
                    if (FFXIV != null && GetOffsets())
                        Attach();
                }
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// TriggerNometry集成
        /// </summary>
        private void TriggIntegration()
        {
            try {
                var plugin = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.pluginObj?.GetType().ToString() == "TriggernometryProxy.ProxyPlugin")?.pluginObj;
                if (plugin == null) {
                    PluginUI.Log(I18n.Translate("PostNamazu/TrigNotFound", "没有找到 Triggernometry。"));
                    return;
                }
                _triggerHoster = new TriggerHoster.Program(plugin) { PostNamazuDelegate = DoAction, LogDelegate = PluginUI.Log };
                _triggerHoster.Init(CmdBind.Keys.ToArray());
                PluginUI.Log(I18n.Translate("PostNamazu/Trig", "已绑定 Triggernometry。"));
            }
            catch (Exception ex) {
                PluginUI.Log(ex.Message);
            }
        }

        /// <summary>
        /// OverlayPlugin集成
        /// </summary>
        private void OverlayIntegration()
        {
            try {
                var plugin = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.pluginObj?.GetType().ToString() == "RainbowMage.OverlayPlugin.PluginLoader")?.pluginObj;
                if (plugin == null) {
                    PluginUI.Log(I18n.Translate("PostNamazu/OPNotFound", "没有找到 OverlayPlugin。"));
                    return;
                }
                _overlayHoster = new OverlayHoster.Program { PostNamazuDelegate = DoAction };
                _overlayHoster.Init();
                PluginUI.Log(I18n.Translate("PostNamazu/OP", "已绑定 OverlayPlugin。"));
            }
            catch (Exception ex) {
                PluginUI.Log(ex.Message);
            }
        }

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
                I18n.Translate("PostNamazu/XivProcSwitch", "已切换至 FFXIV 进程 {0}。", tProcess.Id);
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
                string reflectedType = GetAction(command).GetMethodInfo().ReflectedType!.Name;

                if (ActionEnabled.ContainsKey(reflectedType) && ActionEnabled[reflectedType]) //不响应没有启用的动作
                    GetAction(command)(payload);
                else
                    PluginUI.Log(I18n.Translate("PostNamazu/ActionIgnored", "忽略动作：{0}: {1}", command, payload));
            }
            catch (NamazuModule.IgnoredException) { }
            catch (Exception ex)
            {
                PluginUI.Log(ex.ToString());
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
                throw new Exception(I18n.Translate("PostNamazu/ActionNotFound", "不支持的操作：{0}。", command));
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
