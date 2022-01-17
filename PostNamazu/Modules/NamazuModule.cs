using GreyMagic;
using PostNamazu.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PostNamazu.Modules
{
    internal abstract class NamazuModule
    {
        protected FFXIV_ACT_Plugin.FFXIV_ACT_Plugin _ffxivPlugin;
        protected Process FFXIV;
        protected ExternalProcessMemory Memory;
        protected PostNamazuUi PluginUI;
        protected SigScanner SigScanner;

        internal bool isReady = false;

        public void Setup(PostNamazu postNamazu)
        {
            _ffxivPlugin = postNamazu._ffxivPlugin;
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
            if (_ffxivPlugin != null && FFXIV != null && Memory != null) {
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
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Command { get; }

        public CommandAttribute(string command)
        {
            Command = command;
        }
    }
}
