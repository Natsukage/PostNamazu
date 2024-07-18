using System;
using System.Collections.Generic;
using System.Reflection;
using Advanced_Combat_Tracker;
using TriggernometryProxy;

namespace PostNamazu.TriggerHoster
{
    public class Program
    {
        public Action<string, string> PostNamazuDelegate = null;
        private readonly ProxyPlugin _triggPlugin;

        private readonly List<int> _triggCallbackId = new ();

        public Program(IActPluginV1 plugin)
        {
            _triggPlugin= (ProxyPlugin)plugin;
        }
        public void Init(string[] commands)
        {
            Type triggPluginType = _triggPlugin.GetType();

            // new version (> 1.2.0.1):
            // RegisterNamedCallback(string name, CustomCallbackDelegate callback, object o, string registrant)
            Type[] newParamTypes = { typeof(string), typeof(ProxyPlugin.CustomCallbackDelegate), typeof(object), typeof(string) };

            // old version (<= 1.2.0.1): (also available in new version)
            // RegisterNamedCallback(string name, CustomCallbackDelegate callback, object o)  
            Type[] oldParamTypes = { typeof(string), typeof(ProxyPlugin.CustomCallbackDelegate), typeof(object) };

            MethodInfo registerNamedCallbackMethod = triggPluginType.GetMethod("RegisterNamedCallback", newParamTypes);
            bool isNewSignature = registerNamedCallbackMethod != null;
            registerNamedCallbackMethod ??= triggPluginType.GetMethod("RegisterNamedCallback", oldParamTypes);
            foreach (string command in commands)
            {
                List<object> parameters = new()
                {
                    command,
                    new ProxyPlugin.CustomCallbackDelegate((object _, string payload) => PostNamazuDelegate(command, payload)),
                    null
                };
                if (isNewSignature) parameters.Add("PostNamazu"); // registrant
                _triggCallbackId.Add((int)registerNamedCallbackMethod!.Invoke(_triggPlugin, parameters.ToArray()));
            }
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
