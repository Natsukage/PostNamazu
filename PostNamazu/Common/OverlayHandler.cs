using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using RainbowMage.OverlayPlugin;

namespace PostNamazu.Models
{
    public class OverlayHandler : EventSourceBase
    {
        public delegate void ReceivedRequestEventHandler(string payload);
        private Dictionary<string, ReceivedRequestEventHandler> CmdBind = new Dictionary<string, ReceivedRequestEventHandler>();
        public OverlayHandler(TinyIoCContainer container) : base(container)
        {
            Name = "鲶鱼精邮差";
            RegisterEventHandler("PostNamazu", DoAction);
        }

        private JToken DoAction(JObject jo)
        {
            string action, payload;
            action = jo["c"]?.Value<string>() ?? "null";
            payload = jo["p"]?.Value<string>() ?? "";
            GetAction(action)?.Invoke(payload);
            return null;
        }

        /// <summary>
        ///     设置指令与对应的方法委托
        /// </summary>
        /// <param name="cmd">通过onclick="callOverlayHandler传递的指令类型</param>
        /// <param name="action">对应指令的方法委托</param>
        public void SetAction(string cmd, ReceivedRequestEventHandler action)
        {
            CmdBind[cmd] = action;
        }

        /// <summary>
        ///     获取指令对应的方法
        /// </summary>
        /// <param name="cmd">通过onclick="callOverlayHandler传递的指令类型</param>
        /// <returns>对应指令的委托方法</returns>
        private ReceivedRequestEventHandler GetAction(string cmd)
        {
            try
            {
                return CmdBind[cmd];
            }
            catch
            {
                throw new Exception($@"不支持的操作{cmd}");
            }
        }

        /// <summary>
        ///     清空绑定的委托列表
        /// </summary>
        public void ClearAction()
        {
            CmdBind.Clear();
        }
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
        protected override void Update()
        {
        }
    }
}
