using System;
using System.Diagnostics;
using PostNamazu.Common;
using GreyMagic;
using System.Collections.Generic;
using static PostNamazu.Common.I18n;

namespace PostNamazu.Actions
{
    public abstract class NamazuModule
    {
        protected PostNamazu PostNamazu;
        protected FFXIV_ACT_Plugin.FFXIV_ACT_Plugin FFXIV_ACT_Plugin => PostNamazu?.FFXIV_ACT_Plugin;
        protected Process FFXIV => PostNamazu?.FFXIV;
        protected ExternalProcessMemory Memory => PostNamazu?.Memory;
        protected PostNamazuUi PluginUI => PostNamazu?.PluginUI;
        protected SigScanner SigScanner => PostNamazu?.SigScanner;

        internal static bool isReady = false;

        internal static bool complaintAboutNotReady = false;

        public void Init(PostNamazu postNamazu) => PostNamazu = postNamazu;

        public void Setup()
        {
            try {
                GetOffsets();
                isReady = FFXIV_ACT_Plugin != null && FFXIV != null && Memory != null;
            }
            catch (Exception ex) {
                Log(ex.ToString());
                isReady = false;
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

        public void CheckBeforeExecution()
        {
            if (!isReady)
            {
                string noXivMsg = I18n.Translate("NamazuModule/XivProcNotFound", "没有对应的 FFXIV 进程");
                if (complaintAboutNotReady)
                {
                    throw new IgnoredException(noXivMsg);
                }
                else
                {
                    complaintAboutNotReady = true;
                    throw new Exception(noXivMsg);
                }
            }
            else complaintAboutNotReady = false;
        }

        public void CheckBeforeExecution(string command)
        {
            CheckBeforeExecution();
            if (string.IsNullOrWhiteSpace(command))
                throw new Exception(I18n.Translate("NamazuModule/EmptyCommand", "指令为空"));
        }

        protected virtual Dictionary<string, Dictionary<Language, string>> LocalizedStrings { get; } = new();

        protected string GetLocalizedString(string key, params object[] args)
        {
            if (LocalizedStrings.TryGetValue(key, out var translations) && translations.TryGetValue(CurrentLanguage, out var localizedString))
                return string.Format(localizedString, args);
            return key;
        }

        internal class IgnoredException : Exception {
            internal IgnoredException(string msg) : base(msg) { }
        }
    }

}


namespace PostNamazu.Attributes
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
