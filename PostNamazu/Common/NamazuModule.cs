using System;
using System.Diagnostics;
using PostNamazu.Common;
using GreyMagic;

namespace PostNamazu.Actions
{
    internal abstract class NamazuModule
    {
        protected PostNamazu PostNamazu;
        protected FFXIV_ACT_Plugin.FFXIV_ACT_Plugin FFXIV_ACT_Plugin => PostNamazu?.FFXIV_ACT_Plugin;
        protected Process FFXIV => PostNamazu?.FFXIV;
        protected ExternalProcessMemory Memory => PostNamazu?.Memory;
        protected PostNamazuUi PluginUI => PostNamazu?.PluginUI;
        protected SigScanner SigScanner => PostNamazu?.SigScanner;

        internal bool isReady = false;

        public void Setup(PostNamazu postNamazu)
        {
            PostNamazu=postNamazu;
            try {
                GetOffsets();
            }
            catch (Exception ex) {
                Log(ex.ToString());
                isReady = false;
            }
            //Log("初始化完成");
            if (FFXIV_ACT_Plugin != null && FFXIV != null && Memory != null) {
                isReady = true;
            }
            else {
                isReady = false;
            }
        }

        public virtual void GetOffsets()
        {

        }

        public void Log(string msg)
        {
            PluginUI.Log(msg);
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
