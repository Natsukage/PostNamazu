using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using GreyMagic;
using Newtonsoft.Json;
using PostNamazu.Models;

namespace PostNamazu
{
    public class PostNamazu : UserControl, IActPluginV1
    {
        public PostNamazu() {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static PostNamazuUi PluginUI;
        private Label _lblStatus; // The status label that appears in ACT's Plugin tab

        private HttpServer _httpServer;
        private BackgroundWorker _processSwitcher;

        public static Process FFXIV;
        private static ExternalProcessMemory Memory;
        private FFXIV_ACT_Plugin.FFXIV_ACT_Plugin _ffxivPlugin;
        private TriggernometryProxy.ProxyPlugin triggPlugin;

        private IntPtr _entrancePtr;
        private Offsets Offsets;
        private WayMarks tempMarks;


        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText) {
            pluginScreenSpace.Text = "鲶鱼精邮差";

            PluginUI = new PostNamazuUi();

            PluginUI.InitializeComponent(pluginScreenSpace);
            Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space
            _lblStatus = pluginStatusText; // Hand the status label's reference to our local var

            _ffxivPlugin = GetFfxivPlugin();

            //目前解析插件有bug，在特定情况下无法正常触发ProcessChanged事件。因此只能通过后台线程实时监控
            //_ffxivPlugin.DataSubscription.ProcessChanged += ProcessChanged;

            _processSwitcher = new BackgroundWorker { WorkerSupportsCancellation = true };
            _processSwitcher.DoWork += ProcessSwitcher;
            _processSwitcher.RunWorkerAsync();
            if (PluginUI.AutoStart)
                ServerStart();

            TriggIntegration();
            PluginUI.ButtonStart.Click += ServerStart;
            PluginUI.ButtonStop.Click += ServerStop;

            _lblStatus.Text = "鲶鱼精邮差已启动";
        }

        public void DeInitPlugin() {
            //_ffxivPlugin.DataSubscription.ProcessChanged -= ProcessChanged;
            if (_httpServer != null) ServerStop();
            _processSwitcher.CancelAsync();
            Detach();
            PluginUI.SaveSettings();
            _lblStatus.Text = "鲶鱼精邮差已退出";
        }

        private void ServerStart(object sender = null, EventArgs e = null) {
            try {
                _httpServer = new HttpServer((int)PluginUI.TextPort.Value);
                _httpServer.ReceivedCommandRequest += DoTextCommand;
                _httpServer.ReceivedWayMarksRequest += DoWaymarks;
                _httpServer.OnException += OnException;

                PluginUI.ButtonStart.Enabled = false;
                PluginUI.ButtonStop.Enabled = true;
                PluginUI.Log($"在{_httpServer.Port}端口启动监听");
            }
            catch (Exception ex) {
                OnException(ex);
            }
        }

        private void ServerStop(object sender = null, EventArgs e = null) {
            _httpServer.Stop();
            _httpServer.ReceivedCommandRequest -= DoTextCommand;
            _httpServer.ReceivedWayMarksRequest -= DoWaymarks;
            _httpServer.OnException -= OnException;

            PluginUI.ButtonStart.Enabled = true;
            PluginUI.ButtonStop.Enabled = false;
            PluginUI.Log("已停止监听");
        }
        /// <summary>
        /// 委托给HttpServer类的异常处理
        /// </summary>
        /// <param name="ex"></param>
        private void OnException(Exception ex) {
            string errorMessage = $"无法在{_httpServer.Port}端口启动监听\n{ex.Message}";

            PluginUI.ButtonStart.Enabled = true;
            PluginUI.ButtonStop.Enabled = false;

            PluginUI.Log(errorMessage);
            MessageBox.Show(errorMessage);
        }



        /// <summary>
        ///     对当前解析插件对应的游戏进程进行注入
        /// </summary>
        private void Attach() {
            Debug.Assert(FFXIV != null);
            try {
                Memory = new ExternalProcessMemory(FFXIV, false, false);
                Memory.WriteBytes(_entrancePtr, new byte[] { 76, 139, 220, 83, 86 });
                Memory = new ExternalProcessMemory(FFXIV, true, false, _entrancePtr, false, 5, true);
                PluginUI.Log($"已找到FFXIV进程 {FFXIV.Id}");
            }
            catch (Exception ex) {
                MessageBox.Show($"注入进程时发生错误！\n{ex}", "鲶鱼精邮差", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Detach();
            }
        }

        /// <summary>
        ///     解除注入
        /// </summary>
        private void Detach() {
            try {
                if (Memory != null && !Memory.Process.HasExited)
                    Memory.Dispose();
            }
            catch (Exception) {
                // ignored
            }
        }

        /// <summary>
        ///     在游戏进程中执行给出的指令
        /// </summary>
        /// <param name="command">需要执行的指令</param>
        private void DoTextCommand(string command) {
            if (FFXIV == null) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            PluginUI.Log(command);
            if (command == "")
                throw new Exception("指令为空");

            var assemblyLock = Memory.Executor.AssemblyLock;

            var flag = false;
            try {
                Monitor.Enter(assemblyLock, ref flag);
                var array = Encoding.UTF8.GetBytes(command);
                using (AllocatedMemory allocatedMemory = Memory.CreateAllocatedMemory(400), allocatedMemory2 = Memory.CreateAllocatedMemory(array.Length + 30)) {
                    allocatedMemory2.AllocateOfChunk("cmd", array.Length);
                    allocatedMemory2.WriteBytes("cmd", array);
                    allocatedMemory.AllocateOfChunk<IntPtr>("cmdAddress");
                    allocatedMemory.AllocateOfChunk<long>("t1");
                    allocatedMemory.AllocateOfChunk<long>("tLength");
                    allocatedMemory.AllocateOfChunk<long>("t3");
                    allocatedMemory.Write("cmdAddress", allocatedMemory2.Address);
                    allocatedMemory.Write("t1", 0x40);
                    allocatedMemory.Write("tLength", array.Length + 1);
                    allocatedMemory.Write("t3", 0x00);
                    _ = Memory.CallInjected64<int>(Offsets.ProcessChatBoxPtr, Offsets.RaptureModule,
                        allocatedMemory.Address, Offsets.UiModule);
                }
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }

        private void DoTextCommand(object _, string command) {
            //MessageBox.Show(command);
            DoTextCommand(command);
        }

        /// <summary>
        ///     在游戏进程中进行场地标点
        /// </summary>
        /// <param name="waymarks">标点合集对象</param>
        private void DoWaymarks(WayMarks waymarks) {
            WriteWaymark(waymarks.A, 0);
            WriteWaymark(waymarks.B, 1);
            WriteWaymark(waymarks.C, 2);
            WriteWaymark(waymarks.D, 3);
            WriteWaymark(waymarks.One, 4);
            WriteWaymark(waymarks.Two, 5);
            WriteWaymark(waymarks.Three, 6);
            WriteWaymark(waymarks.Four, 7);
        }
        /// <summary>
        ///     在游戏进程中进行场地标点
        /// </summary>
        /// <param name="waymarksStr">标点合集序列化Json字符串</param>
        private void DoWaymarks(string waymarksStr) {
            if (FFXIV == null) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            switch (waymarksStr.ToLower())
            {
                case "save":
                case "backup":
                    SaveWaymark();
                    break;
                case "load":
                case "restore":
                    LoadWaymark();
                    break;
                default:
                    var waymarks = JsonConvert.DeserializeObject<WayMarks>(waymarksStr);
                    PluginUI.Log(waymarksStr);
                    PluginUI.Log("开始标记");
                    DoWaymarks(waymarks);
                    break;
            }
        }

        private void DoWaymarks(object _, string command) {
            //MessageBox.Show(command);
            DoWaymarks(command);
        }

        /// <summary>
        ///     暂存当前标点
        /// </summary>
        public void SaveWaymark()
        {
            tempMarks = new WayMarks();

            Waymark ReadWaymark(IntPtr addr, WaymarkID id) => new Waymark
            {
                X = Memory.Read<float>(addr),
                Y = Memory.Read<float>(addr + 0x4),
                Z = Memory.Read<float>(addr + 0x8),
                Active = Memory.Read<byte>(addr + 0x1C) == 1,
                ID = id
            };

            try
            {
                tempMarks.A = ReadWaymark(Offsets.Waymarks + 0x00, WaymarkID.A);
                tempMarks.B = ReadWaymark(Offsets.Waymarks + 0x20, WaymarkID.B);
                tempMarks.C = ReadWaymark(Offsets.Waymarks + 0x40, WaymarkID.C);
                tempMarks.D = ReadWaymark(Offsets.Waymarks + 0x60, WaymarkID.D);
                tempMarks.One = ReadWaymark(Offsets.Waymarks + 0x80, WaymarkID.One);
                tempMarks.Two = ReadWaymark(Offsets.Waymarks + 0xA0, WaymarkID.Two);
                tempMarks.Three = ReadWaymark(Offsets.Waymarks + 0xC0, WaymarkID.Three);
                tempMarks.Four = ReadWaymark(Offsets.Waymarks + 0xE0, WaymarkID.Four);
                PluginUI.Log("暂存当前标点");
            }
            catch (Exception ex)
            {
                PluginUI.Log("保存标记错误："+ex.Message);
            }

        }

        /// <summary>
        ///     恢复暂存标点
        /// </summary>
        public void LoadWaymark()
        {
            if (tempMarks == null)
                return;
            DoWaymarks(tempMarks);
            PluginUI.Log("恢复暂存标点");
        }

        /// <summary>
        ///     写入指定标点
        /// </summary>
        /// <param name="waymark">标点</param>
        /// <param name="id">ID</param>
        public void WriteWaymark(Waymark waymark, int id = -1) {
            if (waymark == null)
                return;

            var wId = id == -1 ? (byte)waymark.ID : id;

            var markAddr = IntPtr.Zero;
            switch (wId) {
                case (int)WaymarkID.A:
                    markAddr = Offsets.Waymarks + 0x00;
                    break;
                case (int)WaymarkID.B:
                    markAddr = Offsets.Waymarks + 0x20;
                    break;
                case (int)WaymarkID.C:
                    markAddr = Offsets.Waymarks + 0x40;
                    break;
                case (int)WaymarkID.D:
                    markAddr = Offsets.Waymarks + 0x60;
                    break;
                case (int)WaymarkID.One:
                    markAddr = Offsets.Waymarks + 0x80;
                    break;
                case (int)WaymarkID.Two:
                    markAddr = Offsets.Waymarks + 0xA0;
                    break;
                case (int)WaymarkID.Three:
                    markAddr = Offsets.Waymarks + 0xC0;
                    break;
                case (int)WaymarkID.Four:
                    markAddr = Offsets.Waymarks + 0xE0;
                    break;
            }

            // Write the X, Y and Z coordinates
            Memory.Write(markAddr, waymark.X);
            Memory.Write(markAddr + 0x4, waymark.Y);
            Memory.Write(markAddr + 0x8, waymark.Z);

            Memory.Write(markAddr + 0x10, (int)(waymark.X * 1000));
            Memory.Write(markAddr + 0x14, (int)(waymark.Y * 1000));
            Memory.Write(markAddr + 0x18, (int)(waymark.Z * 1000));

            // Write the active state
            Memory.Write(markAddr + 0x1C, (byte)(waymark.Active ? 1 : 0));
        }

        /// <summary>
        ///     取得解析插件的进程（从獭爹那里偷来的）
        /// </summary>
        /// <returns></returns>
        private FFXIV_ACT_Plugin.FFXIV_ACT_Plugin GetFfxivPlugin() {
            FFXIV_ACT_Plugin.FFXIV_ACT_Plugin ffxivActPlugin = null;
            foreach (var actPluginData in ActGlobals.oFormActMain.ActPlugins)
                if (actPluginData.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                    actPluginData.lblPluginStatus.Text.ToUpper().Contains("FFXIV Plugin Started.".ToUpper()))
                    ffxivActPlugin = (FFXIV_ACT_Plugin.FFXIV_ACT_Plugin)actPluginData.pluginObj;
            return ffxivActPlugin ?? throw new Exception("找不到FFXIV解析插件，请确保其加载顺序位于鲶鱼精邮差之前。");
        }

        /// <summary>
        ///     获取几个重要的地址
        /// </summary>
        /// <returns>返回是否成功找到入口地址</returns>
        private bool GetOffsets() {
            PluginUI.Log("Getting Offsets......");
            try {
                var scanner = new SigScanner(FFXIV);
                try {
                    _entrancePtr = scanner.ScanText("4C 8B DC 53 56 48 81 EC 18 02 00 00 48 8B 05");
                }
                catch (ArgumentOutOfRangeException) {
                    PluginUI.Log("无法对当前进程注入\n可能是已经被其他进程注入了？");
                    return false;
                }

                Offsets = new Offsets(scanner);
#if DEBUG
                PluginUI.Log(Offsets.ProcessChatBoxPtr);
                PluginUI.Log(Offsets.UiModule);
                PluginUI.Log(Offsets.RaptureModule);
#endif
            }
            catch (ArgumentOutOfRangeException) {
                PluginUI.Log("查找失败：找不到特征值");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     获取当前FFXIV解析插件的活动进程
        /// </summary>
        /// <returns>解析插件当前对应进程</returns>
        private Process GetFFXIVProcess() {
            return _ffxivPlugin.DataRepository.GetCurrentFFXIVProcess();
        }

        /// <summary>
        ///     代替ProcessChanged委托，手动循环检测当前活动进程并进行注入。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessSwitcher(object sender, DoWorkEventArgs e) {
            while (true) {
                if (_processSwitcher.CancellationPending) {
                    e.Cancel = true;
                    break;
                }

                if (FFXIV != GetFFXIVProcess()) {
                    Detach();
                    FFXIV = GetFFXIVProcess();
                    if (FFXIV != null)
                        if (FFXIV.ProcessName == "ffxiv")
                            PluginUI.Log("错误：游戏运行于DX9模式下");
                        else if (GetOffsets())
                            Attach();
                }

                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// TriggerNemotry集成
        /// </summary>
        private void TriggIntegration() {
            try {
                var trigg = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.pluginFile.Name.ToUpper().Contains("Triggernometry".ToUpper()));
                triggPlugin = (TriggernometryProxy.ProxyPlugin)trigg.pluginObj;
                if (triggPlugin == null)
                    throw new Exception("找不到Triggernometry插件，请确保其加载顺序位于鲶鱼精邮差之前。");
                triggPlugin.RegisterNamedCallback("DoTextCommand", DoTextCommand, null);
                triggPlugin.RegisterNamedCallback("command", DoTextCommand, null);
                triggPlugin.RegisterNamedCallback("DoWaymarks", DoWaymarks, null);
                triggPlugin.RegisterNamedCallback("place", DoWaymarks, null);
            }
            catch (Exception ex) {
                PluginUI.Log(ex.Message);
            }
        }

        /// <summary>
        /// 取消TriggerNemotry集成，不过不取消似乎也没啥问题
        /// </summary>
        private void TriggPartition() {
            //triggPlugin.UnregisterNamedCallback();
        }

        /// <summary>
        ///     解析插件对应进程改变时触发，解除当前注入并注入新的游戏进程
        ///     目前由于解析插件的bug，ProcessChanged事件无法正常触发，暂时弃用。
        /// </summary>
        /// <param name="tProcess"></param>
        [Obsolete]
        private void ProcessChanged(Process tProcess) {
            if (tProcess.Id != FFXIV?.Id) {
                Detach();
                FFXIV = tProcess;
                if (FFXIV != null)
                    if (GetOffsets())
                        Attach();
                PluginUI.Log($"已切换至进程{tProcess.Id}");
            }
        }

        /// <summary>
        ///     AssemblyResolve事件的处理函数，该函数用来自定义程序集加载逻辑
        /// </summary>
        /// <param name="sender">事件引发源</param>
        /// <param name="args">事件参数，从该参数中可以获取加载失败的程序集的名称</param>
        /// <returns></returns>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            var name = args.Name.Split(',')[0];
            //if (name != "GreyMagic") return null;
            switch (name) {
                case "GreyMagic":
                case "Nancy":
                case "Nancy.Hosting.Self":
                    var selfPluginData = ActGlobals.oFormActMain.PluginGetSelfData(this);
                    var path = selfPluginData.pluginFile.DirectoryName;
                    return Assembly.LoadFile($@"{path}\{name}.dll");
                    break;
                default:
                    return null;
            }

        }
    }
}
