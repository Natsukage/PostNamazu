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
        public ProxyPlugin _triggPlugin;

        public List<int> TriggCallbackId = new ();

        public Program(IActPluginV1 plugin)
        {
            _triggPlugin= (ProxyPlugin)plugin;
        }
        public void Init(string[] commands)
        {
            Type triggPluginType = _triggPlugin.GetType();

            // new version (> 1.2.0.1):
            // RegisterNamedCallback(string name, CustomCallbackDelegate callback, object o, string registrant)
            Type[] newParamTypes = new Type[] { typeof(string), typeof(ProxyPlugin.CustomCallbackDelegate), typeof(object), typeof(string) };

            // old version (<= 1.2.0.1): (also available in new version)
            // RegisterNamedCallback(string name, CustomCallbackDelegate callback, object o)  
            Type[] oldParamTypes = new Type[] { typeof(string), typeof(ProxyPlugin.CustomCallbackDelegate), typeof(object) };

            MethodInfo registerNamedCallbackMethod = triggPluginType.GetMethod("RegisterNamedCallback", newParamTypes);
            bool isNewSignature = registerNamedCallbackMethod != null;
            registerNamedCallbackMethod = registerNamedCallbackMethod ?? triggPluginType.GetMethod("RegisterNamedCallback", oldParamTypes);
            foreach (string command in commands)
            {
                List<object> parameters = new()
                {
                    command,
                    new ProxyPlugin.CustomCallbackDelegate((object _, string payload) => PostNamazuDelegate(command, payload)),
                    null
                };
                if (isNewSignature) parameters.Add("PostNamazu"); // registrant
                TriggCallbackId.Add((int)registerNamedCallbackMethod.Invoke(_triggPlugin, parameters.ToArray()));
            }
        }
        public void DeInit()
        {
            ClearAction();
        }
        public void ClearAction()
        {
            foreach (int id in TriggCallbackId) 
                _triggPlugin.UnregisterNamedCallback(id);
            TriggCallbackId.Clear();
        }
    }
}
