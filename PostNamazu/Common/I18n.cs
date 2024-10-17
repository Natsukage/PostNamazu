﻿using System.Collections.Generic;
using System.Globalization;

namespace PostNamazu.Common
{
    public static class I18n
    {

        public static Language CurrentLanguage;

        public enum Language { EN, CN }

        private static readonly Dictionary<string, Dictionary<Language, string>> UiTranslations = new()
        {
            ["btnWaymarksExport"]               = new() { [Language.EN] = "Export",                     [Language.CN] = "导出" },
            ["btnWaymarksImport"]               = new() { [Language.EN] = "Import",                     [Language.CN] = "导入" },
            ["ButtonClearMessage"]              = new() { [Language.EN] = "Clear Logs",                 [Language.CN] = "清空全部日志" },
            ["ButtonCopyProblematic"]           = new() { [Language.EN] = "Copy All Logs",              [Language.CN] = "复制全部日志" },
            ["ButtonCopySelection"]             = new() { [Language.EN] = "Copy Selection",             [Language.CN] = "复制所选日志" },
            ["ButtonStart"]                     = new() { [Language.EN] = "Start",                      [Language.CN] = "开始" },
            ["ButtonStop"]                      = new() { [Language.EN] = "Stop",                       [Language.CN] = "停止" },
            ["CheckAutoStart"]                  = new() { [Language.EN] = "Auto-start Listening",       [Language.CN] = "自动启动监听" },
            ["mainGroupBox"]                    = new() { [Language.EN] = "PostNamazu",                 [Language.CN] = "鲶鱼精邮差" },
            ["lblEnabledCmd"]                   = new() { [Language.EN] = "Enabled Commands:",          [Language.CN] = "启用以下动作" },
            ["lbPort"]                          = new() { [Language.EN] = "Port",                       [Language.CN] = "端口" },
            ["grpEnabledCmd"]                   = new() { [Language.EN] = "Enabled Commands",           [Language.CN] = "启用以下动作" },
            ["grpHttp"]                         = new() { [Language.EN] = "HTTP",                       [Language.CN] = "HTTP" },
            ["grpLang"]                         = new() { [Language.EN] = "Language",                   [Language.CN] = "语言" },
            ["grpWaymarks"]                     = new() { [Language.EN] = "Waymarks",                   [Language.CN] = "场地标点" },
            ["ImportWaymarksForm"]              = new() { [Language.EN] = "Import waymarks",            [Language.CN] = "导入场地标点" },
            ["ImportWaymarksForm/btnPlace"]     = new() { [Language.EN] = "Import as local waymarks",   [Language.CN] = "导入为本地标点" },
            ["ImportWaymarksForm/btnPublic"]    = new() { [Language.EN] = "Import as public waymarks",  [Language.CN] = "导入为公开标点" },
            ["ImportWaymarksForm/grpMain"]      = new() { [Language.EN] = "Input waymarks JSON string", [Language.CN] = "输入标点 JSON 字符串" },
        };

        private static readonly Dictionary<string, string> TranslationsEN = new()
        {
            ["ImportWaymarksForm/Fail"] = "Failed to apply waymarks:\n{0}",
            ["ImportWaymarksForm/InCombat"] = "Currently in combat, unable to place public waymarks.",
            ["ImportWaymarksForm/Local"] = "Waymarks applied locally.",
            ["ImportWaymarksForm/Public"] = "Waymarks have been made public.",
            ["NamazuModule/EmptyCommand"] = "Empty command.",
            ["NamazuModule/GetOffsetsFail"] = "Failed to initialize the module {0}: \n{1}",
            ["NamazuModule/XivProcNotFound"] = "FFXIV process not found.",
            ["PostNamazu"] = "PostNamazu",
            ["PostNamazu/ActionIgnored"] = "Action Ignored: {0}: {1}",
            ["PostNamazu/ActionNotFound"] = "Unsupported operation: {0}",
            ["PostNamazu/DoActionFail"] = "Exception when executing action {0}: \n{1}",
            ["PostNamazu/HttpException"] = "Unable to start listening on HTTP port {0}: \n{1}",
            ["PostNamazu/HttpStart"] = "Started HTTP listening on port {0}.",
            ["PostNamazu/HttpStop"] = "HTTP listening stopped.",
            ["PostNamazu/OP"] = "OverlayPlugin registered.",
            ["PostNamazu/OPNotFound"] = "OverlayPlugin not found.",
            ["PostNamazu/ParserNotFound"] = "FFXIV parsing plugin not found, please ensure it is loaded before PostNamazu.",
            ["PostNamazu/PluginDeInit"] = "PostNamazu has exited.",
            ["PostNamazu/PluginInit"] = "PostNamazu has started.",
            ["PostNamazu/PluginVersion"] = "Plugin Version: {0}",
            ["PostNamazu/SigScanning"] = "Scanning memory signatures...",
            ["PostNamazu/Trig"] = "Triggernometry registered.",
            ["PostNamazu/TrigNotFound"] = "Triggernometry not found.",
            ["PostNamazu/XivDetectMemRegionCN"] = "Set region to Chinese server (detected by memory).",
            ["PostNamazu/XivDetectMemRegionGlobal"] = "Set region to global server (detected by memory).",
            ["PostNamazu/XivDetectRegionCN"] = "Set region to Chinese server (detected by player name).",
            ["PostNamazu/XivDetectRegionGlobal"] = "Set region to global server (detected by player name).",
            ["PostNamazu/XivFrameworkNotFound"] = "Failed to find memory signature for Framework, some features will not be available. The plugin may need to be updated.",
            ["PostNamazu/XivProcInject"] = "Found FFXIV process {0}.",
            ["PostNamazu/XivProcInjectException"] = "Error when injecting into FFXIV process! \n{0}",
            ["PostNamazu/XivProcInjectFail"] = "Unable to inject into the current FFXIV process, it may have already been injected by another process. Please try restarting the game.",
            ["PostNamazu/XivProcSwitch"] = "Switched to FFXIV process {0}.",
            ["PostNamazuUi/CfgLoadException"] = "Exception occurred when loading configuration file: \n{0}",
            ["PostNamazuUi/CfgReset"] = "Configuration has been reset.",
            ["PostNamazuUi/ExportWaymarks"] = "Waymarker text has been copied to the clipboard.",
            ["PostNamazuUi/ExportWaymarksFail"] = "Failed to read existing waymarkers:\n{0}",
            ["SigScanner/ResultMultiple"] = "Scanned{0} and found {1} memory signatures, unable to determine a unique location.",
            ["SigScanner/ResultNone"] = "Scanned{0} and did not find the required memory signatures.",

        };

        public static string Translate(string key, string CN, params object[] args)
        {
            if (TranslationsEN.TryGetValue(key, out string translation))
            {
                return string.Format(CurrentLanguage == Language.CN ? CN : translation, args);
            }
            return null; // so it could be noticed when the translation does not exist but currently language is CN
        }

        public static string TranslateUi(string key)
        {
            if (UiTranslations.TryGetValue(key, out Dictionary<Language, string> translation))
            {
                string text = translation.TryGetValue(CurrentLanguage, out text) ? text : translation[Language.EN];
                return text;
            }
            else
            {
                return null;
            }
        }

    }
}