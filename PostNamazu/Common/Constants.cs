namespace PostNamazu.Common
{
    /// <summary>
    /// 项目常量定义
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 进程切换检查间隔（毫秒）
        /// </summary>
        public const int ProcessSwitchInterval = 3000;

        /// <summary>
        /// 模组初始化最大等待次数
        /// </summary>
        public const int ModuleInitMaxWaitCount = 20;

        /// <summary>
        /// 模组初始化等待间隔（毫秒）
        /// </summary>
        public const int ModuleInitWaitInterval = 1000;

        /// <summary>
        /// 内存分配大小
        /// </summary>
        public const int MemoryAllocationSize = 400;

        /// <summary>
        /// 命令缓冲区大小
        /// </summary>
        public const int CommandBufferSize = 30;

        /// <summary>
        /// 当前频道前缀
        /// </summary>
        public const string CurrentChannelPrefix = "/current ";

        /// <summary>
        /// 插件名称
        /// </summary>
        public const string PluginName = "PostNamazu";

        /// <summary>
        /// Triggernometry 插件类型名
        /// </summary>
        public const string TriggernometryPluginType = "TriggernometryProxy.ProxyPlugin";

        /// <summary>
        /// OverlayPlugin 插件类型名
        /// </summary>
        public const string OverlayPluginType = "RainbowMage.OverlayPlugin.PluginLoader";
    }
} 