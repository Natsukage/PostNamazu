using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static PostNamazuUi PluginUI;
        private Label _lblStatus; // The status label that appears in ACT's Plugin tab

        private HttpServer _httpServer;
        private BackgroundWorker _processSwitcher;

        public static Process FFXIV;
        private static ExternalProcessMemory Memory;
        private FFXIV_ACT_Plugin.FFXIV_ACT_Plugin _ffxivPlugin;
        //private TriggernometryProxy.ProxyPlugin triggPlugin;

        private IntPtr _entrancePtr;
        private Offsets Offsets;
        private WayMarks tempMarks;

        #region Init
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText) {
            pluginScreenSpace.Text = "鲶鱼精邮差";

            PluginUI = new PostNamazuUi();

            PluginUI.InitializeComponent(pluginScreenSpace);
            PluginUI.Log($"插件版本:{Assembly.GetExecutingAssembly().GetName().Version}");

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
                _httpServer.SetAction("command", DoTextCommand);
                _httpServer.SetAction("place", DoWaymarks);
                _httpServer.SetAction("sendkey", DoSendKey);
                _httpServer.SetAction("mark", DoMarking);
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
            _httpServer.ClearAction();
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
        ///     取得解析插件的进程（从獭爹那里偷来的）
        /// </summary>
        /// <returns></returns>
        private FFXIV_ACT_Plugin.FFXIV_ACT_Plugin GetFfxivPlugin() {
            FFXIV_ACT_Plugin.FFXIV_ACT_Plugin ffxivActPlugin = null;
            foreach (var actPluginData in ActGlobals.oFormActMain.ActPlugins)
                if (actPluginData.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                    actPluginData.lblPluginStatus.Text.ToUpper().Contains("FFXIV_ACT_Plugin Started.".ToUpper()))
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
                    _entrancePtr = scanner.ScanText("4C 8B DC 53 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 83 B9");
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
                //triggPlugin = (TriggernometryProxy.ProxyPlugin)trigg.pluginObj;

                if (trigg == null || trigg.pluginObj == null)
                    throw new Exception("找不到Triggernometry插件，请确保其加载顺序位于鲶鱼精邮差之前。");

                var triggType = trigg.pluginObj.GetType();
                var deleType = triggType.GetNestedType("CustomCallbackDelegate");
                if (deleType == null)
                    throw new Exception($"{DateTime.Now.Year}年了，害搁这用老版本触发器呢？");

                var registerType = triggType.GetMethod("RegisterNamedCallback");

                var textCommandDele = Delegate.CreateDelegate(deleType, this, typeof(PostNamazu).GetMethod("DoTextCommand"));
                registerType?.Invoke(trigg.pluginObj, new object[] { "DoTextCommand", textCommandDele, null });
                registerType?.Invoke(trigg.pluginObj, new object[] { "command", textCommandDele, null });

                var wayMarkDele = Delegate.CreateDelegate(deleType, this, typeof(PostNamazu).GetMethod("DoWaymarks"));
                registerType?.Invoke(trigg.pluginObj, new object[] { "DoWaymarks", wayMarkDele, null });
                registerType?.Invoke(trigg.pluginObj, new object[] { "place", wayMarkDele, null });

                var sendKeyDele = Delegate.CreateDelegate(deleType, this, typeof(PostNamazu).GetMethod("DoSendKey"));
                registerType?.Invoke(trigg.pluginObj, new object[] { "sendkey", sendKeyDele, null });

                var markingDele = Delegate.CreateDelegate(deleType, this, typeof(PostNamazu).GetMethod("DoMarking"));
                registerType?.Invoke(trigg.pluginObj, new object[] { "mark", markingDele, null });

                /*triggPlugin.RegisterNamedCallback("DoTextCommand", DoTextCommand, null);
                triggPlugin.RegisterNamedCallback("DoWaymarks", DoWaymarks, null);
                triggPlugin.RegisterNamedCallback("command", DoTextCommand, null);
                triggPlugin.RegisterNamedCallback("place", DoWaymarks, null);
                triggPlugin.RegisterNamedCallback("sendkey", DoSendKey, null);
                triggPlugin.RegisterNamedCallback("mark", DoMarking, null);*/
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
        ///     GrayMagic也打包了，已经不需要再从外部加载dll了
        /// </summary>
        /// <param name="sender">事件引发源</param>
        /// <param name="args">事件参数，从该参数中可以获取加载失败的程序集的名称</param>
        /// <returns></returns>
        [Obsolete]
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
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

        #region TextCommand

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

        public void DoTextCommand(object _, string command) {
            //MessageBox.Show(command);
            DoTextCommand(command);
        }
        #endregion

        #region WayMarks

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

        public void DoWaymarks(object _, string command) {
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
        #endregion

        #region SendKey
        private void DoSendKey(string command) {
            PluginUI.Log($"收到按键：{command}");
            try {
                var keycode = int.Parse(command);
                SendKeycode(keycode);
            }
            catch(Exception ex) {
                PluginUI.Log($"发送按键失败：{ex}");
            }
        }
        public void DoSendKey(object _, string command) {
            //MessageBox.Show(command);
            DoSendKey(command);
        }

        public static void SendKeycode(int keycode) {
            SendMessageToWindow(WinAPI.WM_KEYDOWN, keycode, 0);
            SendMessageToWindow(WinAPI.WM_KEYUP, keycode, 0);
        }

        public static void SendMessageToWindow(uint code, int wparam, int lparam) {
            IntPtr hwnd = FFXIV.MainWindowHandle;
            if (hwnd != IntPtr.Zero) {
                IntPtr res = WinAPI.SendMessage(hwnd, code, (IntPtr)wparam, (IntPtr)lparam);
            }
        }
        #endregion

        #region Marking
        private void DoMarking(string command) {
            if (FFXIV == null) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            if (command == "")
                throw new Exception("指令为空");
            var dic = ParseQueryString(command);

            PluginUI.Log(command);

            bool localOnly = dic.ContainsKey("Local") && bool.Parse(dic["Local"]);

            if (dic.ContainsKey("MarkType")) {
                var MarkTypeStr = dic["MarkType"];
                var markingType = MarkingType.attack1;
                if (!Enum.TryParse < MarkingType > (MarkTypeStr, true,out markingType)) {
                    PluginUI.Log($"未知的标记类型:{MarkTypeStr}");
                    return;
                }
                if (dic.ContainsKey("ActorID")) {
                    var ActorIDStr = dic["ActorID"];
                    var ActorID = UInt32.Parse(ActorIDStr, NumberStyles.HexNumber);
                    DoMarkingByActorID(ActorID, markingType, localOnly);
                }
                else if (dic.ContainsKey("Name")){
                    var Name = dic["Name"];
                    GetActorIDByName(Name, markingType, localOnly);
                }
                else {
                    PluginUI.Log("错误指令");
                }
            }
            else {
                PluginUI.Log("错误指令");
            };
            return;
        }
        private void GetActorIDByName(string Name, MarkingType markingType, bool localOnly = false) {
            var combatant = _ffxivPlugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.Name != null && i.ID != 0xE0000000 && i.Name.Equals(Name));
            if (combatant == null) {
                PluginUI.Log($"未能找到{Name}");
                return;
            }
            //PluginUI.Log($"BNpcID={combatant.BNpcNameID},ActorID={combatant.ID:X},markingType={markingType}");
            DoMarkingByActorID(combatant.ID, markingType, localOnly);
        }
        private void DoMarkingByActorID(uint ActorID, MarkingType markingType, bool localOnly = false) {
            var combatant = _ffxivPlugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.ID==ActorID);
            if (combatant == null) {
                PluginUI.Log($"未能找到{ActorID}");
                return;
            }
            PluginUI.Log($"ActorID={ActorID:X},markingType={(int)markingType},LocalOnly={localOnly}");
            var assemblyLock = Memory.Executor.AssemblyLock;
            var flag = false;
            try {
                Monitor.Enter(assemblyLock, ref flag);
                if (!localOnly)
                    _ = Memory.CallInjected64<char>(Offsets.MarkingFunc, Offsets.MarkingController, markingType, ActorID);
                else //本地标点的markingType从0开始，因此需要-1
                    _ = Memory.CallInjected64<char>(Offsets.LocalMarkingFunc, Offsets.MarkingController, markingType - 1, ActorID, 0);
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }

        public void DoMarking(object _, string command) {
            DoMarking(command);
        }
        public static Dictionary<string, string> ParseQueryString(string url) {
            if (string.IsNullOrWhiteSpace(url)) {
                throw new ArgumentNullException("字符串为空");
            }
            if (string.IsNullOrWhiteSpace(url)) {
                return new Dictionary<string, string>();
            }
            var dic = url
                    //2.通过&划分各个参数
                    .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                    //3.通过=划分参数key和value,且保证只分割第一个=字符
                    .Select(param => param.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                    //4.通过相同的参数key进行分组
                    .GroupBy(part => part[0], part => part.Length > 1 ? part[1] : string.Empty)
                    //5.将相同key的value以,拼接
                    .ToDictionary(group => group.Key, group => string.Join(",", group));

            return dic;
        }
        #endregion

    }
}
