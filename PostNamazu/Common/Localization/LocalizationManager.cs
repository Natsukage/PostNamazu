using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace PostNamazu.Common.Localization
{
    public enum Language
    {
        EN,
        CN
    }

    public static class LocalizationManager
    {
        private static readonly Dictionary<string, LocalizationEntry> Localizations = new();
        private static Language _currentLanguage = Language.EN;

        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set => _currentLanguage = value;
        }

        static LocalizationManager()
        {
            // 初始化时扫描所有程序集中的本地化字符串
            DiscoverLocalizations();
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            if (Localizations.TryGetValue(key, out var entry))
            {
                var text = CurrentLanguage == Language.CN ? entry.Chinese : entry.English;
                return args.Length > 0 ? string.Format(text, args) : text;
            }
            
            // 如果找不到翻译，返回key本身，方便调试
            return $"[{key}]";
        }

        /// <summary>
        /// 获取所有已注册的本地化键（用于调试）
        /// </summary>
        public static Dictionary<string, LocalizationEntry> GetAllLocalizations()
        {
            return new Dictionary<string, LocalizationEntry>(Localizations);
        }

        /// <summary>
        /// 获取已注册键的数量（用于调试）
        /// </summary>
        public static int GetRegisteredCount()
        {
            return Localizations.Count;
        }

        /// <summary>
        /// 注册本地化字符串
        /// </summary>
        public static void Register(string key, string english, string chinese)
        {
            Localizations[key] = new LocalizationEntry { English = english, Chinese = chinese };
        }

        /// <summary>
        /// 自动发现并注册所有标记了LocalizedAttribute的字段
        /// </summary>
        private static void DiscoverLocalizations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            foreach (var type in assembly.GetTypes())
            {
                // 检查类是否标记了LocalizationProvider
                var providerAttr = type.GetCustomAttribute<LocalizationProviderAttribute>();
                if (providerAttr == null) continue;

                var prefix = providerAttr.Prefix ?? type.Name;

                // 查找所有标记了Localized的字段
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    var localizedAttr = field.GetCustomAttribute<LocalizedAttribute>();
                    if (localizedAttr == null) continue;

                    var key = $"{prefix}/{field.Name}";
                    Register(key, localizedAttr.English, localizedAttr.Chinese);
                }
            }

            // 设置默认语言
            CurrentLanguage = CultureInfo.CurrentCulture.Name.StartsWith("zh") ? Language.CN : Language.EN;
        }

        public class LocalizationEntry
        {
            public string English { get; set; }
            public string Chinese { get; set; }
        }
    }

    /// <summary>
    /// 简化的本地化访问类
    /// </summary>
    public static class L
    {
        /// <summary>
        /// 获取本地化字符串的快捷方法
        /// </summary>
        public static string Get(string key, params object[] args) => LocalizationManager.Get(key, args);
    }
} 