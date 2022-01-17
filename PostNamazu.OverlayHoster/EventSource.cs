using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using Newtonsoft.Json.Linq;
using RainbowMage.OverlayPlugin;

namespace PostNamazu.OverlayHoster
{
    internal class EventSource : EventSourceBase
    {
        public Action<string, string> PostNamazuDelegate = null;
        public EventSource(TinyIoCContainer container) : base(container)
        {
            Name = "鲶鱼精邮差";
            try
            {
                RegisterEventHandler("PostNamazu", DoAction);
            }
            catch (Exception)
            {
                //ignored
            }
        }
        private JToken DoAction(JObject jo)
        {
            string command = jo["c"]?.Value<string>() ?? "null";
            string payload = jo["p"]?.Value<string>() ?? "";
            if (PostNamazuDelegate == null) 
                throw new ArgumentNullException("没有活动的鲶鱼精邮差插件本体");
            PostNamazuDelegate(command, payload);
            return null;
        }
        #region EventSourceBaseRequired

        public override Control CreateConfigControl()
        {
            return new Control();
        }

        public override void LoadConfig(IPluginConfig config)
        {
        }

        public override void SaveConfig(IPluginConfig config)
        {
        }
        /// <summary>
        /// This method is called periodically when using the embedded timer.
        /// </summary>
        protected override void Update()
        {
        }

        #endregion
    }
}
