#pragma warning disable CS0169 // 字段从未被使用
namespace PostNamazu.Common.Localization
{
    /// <summary>
    /// 核心本地化字符串定义
    /// </summary>
    [LocalizationProvider("PostNamazu")]
    public static class CoreLocalizations
    {
        // UI相关
        [Localized("PostNamazu", "鲶鱼精邮差")]
        private static readonly string title;

        [Localized("Export", "导出")]
        private static readonly string btnWaymarksExport;

        [Localized("Import", "导入")]
        private static readonly string btnWaymarksImport;

        [Localized("Clear Logs", "清空全部日志")]
        private static readonly string ButtonClearMessage;

        [Localized("Copy All Logs", "复制全部日志")]
        private static readonly string ButtonCopyProblematic;

        [Localized("Copy Selection", "复制所选日志")]
        private static readonly string ButtonCopySelection;

        [Localized("Start", "开始")]
        private static readonly string ButtonStart;

        [Localized("Stop", "停止")]
        private static readonly string ButtonStop;

        [Localized("Auto-start Listening", "自动启动监听")]
        private static readonly string CheckAutoStart;

        [Localized("PostNamazu", "鲶鱼精邮差")]
        private static readonly string mainGroupBox;

        [Localized("Enabled Commands:", "启用以下动作")]
        private static readonly string lblEnabledCmd;

        [Localized("Port", "端口")]
        private static readonly string lbPort;

        [Localized("Enabled Commands", "启用以下动作")]
        private static readonly string grpEnabledCmd;

        [Localized("HTTP", "HTTP")]
        private static readonly string grpHttp;

        [Localized("Language", "语言")]
        private static readonly string grpLang;

        [Localized("Waymarks", "场地标点")]
        private static readonly string grpWaymarks;

        [Localized("Import waymarks", "导入场地标点")]
        private static readonly string importWaymarksForm;

        [Localized("Fill in default data", "填入默认数据")]
        private static readonly string importWaymarksFormBtnDefault;

        [Localized("Import as local waymarks", "导入为本地标点")]
        private static readonly string importWaymarksFormBtnPlace;

        [Localized("Import as public waymarks", "导入为公开标点")]
        private static readonly string importWaymarksFormBtnPublic;

        [Localized("Input waymarks JSON string", "输入标点 JSON 字符串")]
        private static readonly string importWaymarksFormGrpMain;

        // 主程序相关
        [Localized("Action Ignored: {0}: {1}", "忽略动作：{0}: {1}")]
        private static readonly string actionIgnored;

        [Localized("Unsupported operation: {0}", "不支持的操作：{0}。")]
        private static readonly string actionNotFound;

        [Localized("Exception when executing action {0}: \n{1}", "执行 {0} 动作时遇到错误：{1}")]
        private static readonly string doActionFail;

        [Localized("Unable to start listening on HTTP port {0}: \n{1}", "无法在 {0} 端口启动监听：\n{1}")]
        private static readonly string httpException;

        [Localized("Started HTTP listening on port {0}.", "在 {0} 端口启动 HTTP 监听。")]
        private static readonly string httpStart;

        [Localized("HTTP listening stopped.", "已停止 HTTP 监听。")]
        private static readonly string httpStop;

        [Localized("OverlayPlugin registered.", "已绑定 OverlayPlugin。")]
        private static readonly string op;

        [Localized("OverlayPlugin not found.", "没有找到 OverlayPlugin。")]
        private static readonly string opNotFound;

        [Localized("FFXIV parsing plugin not found, please ensure it is loaded before PostNamazu.", 
                   "找不到 FFXIV 解析插件，请确保其加载顺序位于鲶鱼精邮差之前。")]
        private static readonly string parserNotFound;

        [Localized("PostNamazu has exited.", "鲶鱼精邮差已退出。")]
        private static readonly string pluginDeInit;

        [Localized("PostNamazu has started.", "鲶鱼精邮差已启动。")]
        private static readonly string pluginInit;

        [Localized("Plugin Version: {0}", "插件版本：{0}。")]
        private static readonly string pluginVersion;

        [Localized("Failed to read memory during initialization, which might be because of Dalamud or other plugins.", 
                   "初始化时读取内存失败，可能是卫月等插件所致。")]
        private static readonly string readMemoryFail;

        [Localized("Scanning memory signatures...", "正在扫描内存特征……")]
        private static readonly string sigScanning;

        [Localized("Triggernometry registered.", "已绑定 Triggernometry。")]
        private static readonly string trig;

        [Localized("Triggernometry not found.", "没有找到 Triggernometry。")]
        private static readonly string trigNotFound;

        [Localized("Set region to Chinese server (detected by memory).", "已设置为国服配置（根据内存信息判断）。")]
        private static readonly string xivDetectMemRegionCN;

        [Localized("Set region to global server (detected by memory).", "已设置为国际服配置（根据内存信息判断）。")]
        private static readonly string xivDetectMemRegionGlobal;

        [Localized("Set region to Chinese server.", "已设置为国服。")]
        private static readonly string xivDetectRegionCN;

        [Localized("Set region to global server.", "已设置为国际服。")]
        private static readonly string xivDetectRegionGlobal;

        [Localized("Detected GLOBAL key: {0}", "检测到GLOBAL密钥：{0}")]
        private static readonly string xivDetectKey;

        [Localized("Failed to find memory signature for Framework, some features will not be available. The plugin may need to be updated. Exception: {0}", 
                   "未找到 Framework 的内存签名，部分功能无法使用，可能需要更新插件版本。错误：{0}")]
        private static readonly string xivFrameworkNotFound;

        [Localized("Found FFXIV process {0}.", "已找到 FFXIV 进程 {0}。")]
        private static readonly string xivProcInject;

        [Localized("Connected to FFXIV process {0}.", "已连接至 FFXIV 进程 {0}。")]
        private static readonly string xivProcInjectConnected;

        [Localized("Error when injecting into FFXIV process, retry later: \n{0}", "注入 FFXIV 进程时发生错误，正在重试：\n{0}")]
        private static readonly string xivProcInjectException;

        [Localized("Unable to inject into the current FFXIV process, it may have already been injected by another process. Please try restarting the game.", 
                   "无法注入当前进程，可能是已经被其他进程注入了，请尝试重启游戏。")]
        private static readonly string xivProcInjectFail;

        [Localized("Failed to connect to FFXIV process {0}:\n{1}", "无法连接至FFXIV进程 {0}：\n{1}")]
        private static readonly string xivProcInjectFailWithError;

        [Localized("Switched to FFXIV process {0}.", "已切换至 FFXIV 进程 {0}。")]
        private static readonly string xivProcSwitch;

        [Localized("Failed to scan server version: {0}", "扫描服务器版本时出现错误：{0}")]
        private static readonly string getRegionMemoryFail;

        // NamazuModule相关
        [Localized("Empty command.", "指令为空")]
        private static readonly string emptyCommand;

        [Localized("Failed to initialize the module {0}: \n{1}", "初始化 {0} 模组失败：\n{1}")]
        private static readonly string getOffsetsFail;

        [Localized("Module {0} failed to initialize, please check previous error messages.", 
                   "{0} 模组初始化失败，请检查更早的错误信息。")]
        private static readonly string moduleInitFail;

        [Localized("Module {0} was not initialized.", "{0} 模组尚未初始化。")]
        private static readonly string moduleNotReady;

        [Localized("FFXIV process not found.", "没有对应的 FFXIV 进程")]
        private static readonly string xivProcNotFound;

        // ImportWaymarksForm相关
        [Localized("Failed to apply waymarks:\n{0}", "应用标点失败：\n{0}")]
        private static readonly string importWaymarksFormFail;

        [Localized("Currently in combat, unable to place public waymarks.", "当前正在战斗中，无法放置公开标点。")]
        private static readonly string importWaymarksFormInCombat;

        [Localized("Waymarks applied locally.", "已应用本地标点。")]
        private static readonly string importWaymarksFormLocal;

        [Localized("Waymarks have been made public.", "已放置公开标点。")]
        private static readonly string importWaymarksFormPublic;

        // PostNamazuUi相关
        [Localized("Exception occurred when loading configuration file: \n{0}", "加载配置文件时发生异常：\n{0}")]
        private static readonly string cfgLoadException;

        [Localized("Configuration has been reset.", "配置已重置。")]
        private static readonly string cfgReset;

        [Localized("Waymarker text has been copied to the clipboard.", "标点文本已复制到剪贴板。")]
        private static readonly string exportWaymarks;

        [Localized("Failed to read existing waymarkers:\n{0}", "读取现有标点失败：\n{0}")]
        private static readonly string exportWaymarksFail;

        // SigScanner相关
        [Localized("Relative addressing sigcode ({0}) must contain at least 4 consecutive stars (* * * * ...) and no additional * elsewhere.",
                   "相对寻址签名码（{0}）必须含至少四个连续星号通配符（* * * * ...），且无额外星号。")]
        private static readonly string relAddressingFormatError;

        [Localized("Scanned{0} and found {1} memory signatures, unable to determine a unique location.", 
                   "扫描{0}发现了 {1} 个内存签名，无法确定唯一位置。")]
        private static readonly string resultMultiple;

        [Localized("Scanned{0} and did not find the required memory signatures.", "扫描{0}未找到所需的内存签名。")]
        private static readonly string resultNone;
    }
} 
