using System;
using System.Linq;
using Advanced_Combat_Tracker;
using PostNamazu.Common.Localization;

namespace PostNamazu.Common
{
    /// <summary>
    /// 插件集成管理器
    /// </summary>
    public class PluginIntegrationManager
    {
        private readonly PostNamazu _plugin;
        private TriggerHoster.Program _triggerHoster;
        private OverlayHoster.Program _overlayHoster;

        public PluginIntegrationManager(PostNamazu plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
        }

        /// <summary>
        /// 初始化所有插件集成
        /// </summary>
        public void InitializeIntegrations()
        {
            InitializeTriggerIntegration();
            InitializeOverlayIntegration();
        }

        /// <summary>
        /// 反初始化所有插件集成
        /// </summary>
        public void DeInitializeIntegrations()
        {
            _overlayHoster?.DeInit();
            _triggerHoster?.DeInit();
        }

        /// <summary>
        /// Triggernometry集成初始化
        /// </summary>
        private void InitializeTriggerIntegration()
        {
            try
            {
                var plugin = ActGlobals.oFormActMain.ActPlugins
                    .FirstOrDefault(x => x.pluginObj?.GetType().ToString() == Constants.TriggernometryPluginType)?.pluginObj;

                if (plugin == null)
                {
                    _plugin.PluginUi?.Log(L.Get("PostNamazu/trigNotFound"));
                    return;
                }

                _triggerHoster = new TriggerHoster.Program(plugin)
                {
                    PostNamazuDelegate = _plugin.DoAction,
                    LogDelegate = _plugin.PluginUi.Log
                };

                _triggerHoster.Init(_plugin.GetCommandKeys());
                _plugin.PluginUi?.Log(L.Get("PostNamazu/trig"));
            }
            catch (Exception ex)
            {
                _plugin.PluginUi?.Log(ex.Message);
            }
        }

        /// <summary>
        /// OverlayPlugin集成初始化
        /// </summary>
        private void InitializeOverlayIntegration()
        {
            try
            {
                var plugin = ActGlobals.oFormActMain.ActPlugins
                    .FirstOrDefault(x => x.pluginObj?.GetType().ToString() == Constants.OverlayPluginType)?.pluginObj;

                if (plugin == null)
                {
                    _plugin.PluginUi?.Log(L.Get("PostNamazu/opNotFound"));
                    return;
                }

                _overlayHoster = new OverlayHoster.Program { PostNamazuDelegate = _plugin.DoAction };
                _overlayHoster.Init();
                _plugin.PluginUi?.Log(L.Get("PostNamazu/op"));
            }
            catch (Exception ex)
            {
                _plugin.PluginUi?.Log(ex.Message);
            }
        }
    }
} 