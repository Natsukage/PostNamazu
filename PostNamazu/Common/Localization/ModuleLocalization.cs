using System.Reflection;

namespace PostNamazu.Common.Localization
{
    /// <summary>
    /// Module本地化基类，提供简洁的本地化定义方式
    /// </summary>
    public abstract class ModuleLocalization
    {
        protected ModuleLocalization()
        {
            RegisterLocalizations();
        }

        /// <summary>
        /// 注册当前类中定义的所有本地化字符串
        /// </summary>
        private void RegisterLocalizations()
        {
            var type = GetType();
            var moduleType = type.DeclaringType;
            var prefix = moduleType?.Name ?? type.Name;

            // 获取所有公共字段
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(LocalizedString))
                {
                    var value = (LocalizedString)field.GetValue(null);
                    if (value != null)
                    {
                        var key = $"{prefix}/{field.Name}";
                        LocalizationManager.Register(key, value.English, value.Chinese);
                    }
                }
            }
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        protected static string Get(string key, params object[] args)
        {
            // 获取调用者的类型来构建完整的key
            var stackFrame = new System.Diagnostics.StackFrame(1);
            var method = stackFrame.GetMethod();
            var type = method?.DeclaringType;
            
            // 如果是嵌套类，使用外部类的名称作为前缀
            if (type != null && type.IsNested)
            {
                type = type.DeclaringType;
            }
            
            var fullKey = type != null ? $"{type.Name}/{key}" : key;
            return LocalizationManager.Get(fullKey, args);
        }
    }

    /// <summary>
    /// 表示一个本地化字符串
    /// </summary>
    public class LocalizedString
    {
        public string English { get; }
        public string Chinese { get; }

        public LocalizedString(string english, string chinese)
        {
            English = english;
            Chinese = chinese;
        }

        /// <summary>
        /// 隐式转换，方便直接使用
        /// </summary>
        public static implicit operator string(LocalizedString str)
        {
            return LocalizationManager.CurrentLanguage == Language.CN ? str.Chinese : str.English;
        }
    }
} 