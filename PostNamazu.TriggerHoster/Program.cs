using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Advanced_Combat_Tracker;
using TriggernometryProxy;

namespace PostNamazu.TriggerHoster
{
    public class Program
    {
        public Action<string, string> PostNamazuDelegate = null;
        public Action<string> LogDelegate = null;
        private readonly ProxyPlugin _triggPlugin;

        private readonly List<int> _triggCallbackId = new ();

        public Program(IActPluginV1 plugin)
        {
            _triggPlugin = (ProxyPlugin)plugin;
        }

        public void Init(string[] commands)
        {
            Type triggPluginType = _triggPlugin.GetType();
            Type[] paramTypes = { typeof(string), typeof(ProxyPlugin.CustomCallbackDelegate), typeof(object), typeof(string) };
            MethodInfo registerNamedCallbackMethod = triggPluginType.GetMethod("RegisterNamedCallback", paramTypes);
            if (registerNamedCallbackMethod == null)
            {
                string fullName = ActGlobals.oFormActMain.ActPlugins
                    .FirstOrDefault(x => x.pluginObj?.GetType().ToString() == "TriggernometryProxy.ProxyPlugin")
                    .pluginFile.FullName;
                string version = FileVersionInfo.GetVersionInfo(fullName).FileVersion;
                string minVersion = "1.2.0.7";  // 第一个适配 7.0 并修复了脚本环境的稳定版本
                if (new Version(version) < new Version(minVersion))
                {
                    bool isCN = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "zh";
                    string msg = isCN ? $"Triggernometry 触发器版本 {version} 过旧，不适配 7.0，未注册回调。请尽快更新至 {minVersion} 或更高。"
                                      : $"Triggernometry version {version} is too old to support 7.0, callbacks were not registered. Please update to >{minVersion} or higher asap.";
                    throw new Exception(msg);
                }
            }
            foreach (string command in commands)
            {
                object[] parameters = new object[]
                {
                    command,
                    new ProxyPlugin.CustomCallbackDelegate((object _, string payload) => PostNamazuDelegate(command, payload)),
                    null,
                    "PostNamazu" // registrant
                };
                _triggCallbackId.Add((int)registerNamedCallbackMethod!.Invoke(_triggPlugin, parameters));
            }
            _triggCallbackId.Add((int)registerNamedCallbackMethod!.Invoke(_triggPlugin, 
                new object[] { "NamazuLog", new ProxyPlugin.CustomCallbackDelegate((_, log) => LogDelegate(log)), null, "PostNamazu" })
            );
        }

        public void DeInit()
        {
            ClearAction();
        }

        public void ClearAction()
        {
            foreach (var id in _triggCallbackId) 
                _triggPlugin.UnregisterNamedCallback(id);
            _triggCallbackId.Clear();
        }
    }
}
