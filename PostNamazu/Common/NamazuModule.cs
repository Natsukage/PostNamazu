using System;
using System.Diagnostics;
using PostNamazu.Common;
using GreyMagic;

namespace PostNamazu.Actions
{
    internal abstract class NamazuModule
    {
        protected FFXIV_ACT_Plugin.FFXIV_ACT_Plugin FFXIV_ACT_Plugin;
        protected Process FFXIV;
        protected ExternalProcessMemory Memory;
        protected PostNamazuUi PluginUI;
        protected SigScanner SigScanner;

        internal bool isReady = false;

        public void Setup(PostNamazu postNamazu)
        {
            FFXIV_ACT_Plugin = postNamazu.FFXIV_ACT_Plugin;
            FFXIV = postNamazu.FFXIV;
            Memory = postNamazu.Memory;
            PluginUI = postNamazu.PluginUI;
            SigScanner = postNamazu.SigScanner;
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
