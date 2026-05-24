using GreyMagic;
using PostNamazu.Common;
using PostNamazu.Common.Localization;
using System;

namespace PostNamazu
{
    public partial class PostNamazu
    {
        /// <summary>
        ///   尝试在 GreyMagic FrameLock 窗口内同步执行 Call 等操作。<br />
        ///   若短期内多次调用，会自动尝试在同一帧内调用，注意避免传入高延迟动作阻塞 UI。<br />
        /// </summary>
        /// <remarks>
        ///   用于降低连续直接调用 CallInjected64 时因等待 hook 入口再次触发而产生的高延迟。<br />
        ///   如果只是调用函数，可直接使用 Call()。
        /// </remarks>
        public void ExecuteInFrameLock(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var scheduler = GetScheduler();
            scheduler.Execute(action);
        }

        /// <summary>
        ///   尝试在 GreyMagic FrameLock 窗口内同步执行 Call 等操作。<br />
        ///   若短期内多次调用，会自动尝试在同一帧内调用，注意避免传入高延迟动作阻塞 UI。<br />
        /// </summary>
        /// <remarks>
        ///   用于降低连续直接调用 CallInjected64 时因等待 hook 入口再次触发而产生的高延迟。<br />
        ///   如果只是调用函数，可直接使用 Call<T>()。
        /// </remarks>
        public T ExecuteInFrameLock<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            T result = default;
            ExecuteInFrameLock(() => result = func());
            return result;
        }

        /// <summary> 直接通过 GreyMagic 在指定的函数地址调用 CallInjected64，不经过 FrameLock 调度器。</summary>
        public void DirectCall(IntPtr ptr, params object[] args)
        {
            var memory = GetMemory();
            ValidateFunctionPointer(ptr);

            var normalizedArgs = NormalizeArgs(args);
            _ = WrapCallInjected64<IntPtr>(memory, ptr, normalizedArgs);
        }

        /// <summary> 直接通过 GreyMagic 在指定的函数地址调用 CallInjected64，不经过 FrameLock 调度器。</summary>
        public T DirectCall<T>(IntPtr ptr, params object[] args) where T : struct
        {
            var memory = GetMemory();
            ValidateFunctionPointer(ptr);

            var normalizedArgs = NormalizeArgs(args);
            return WrapCallInjected64<T>(memory, ptr, normalizedArgs);
        }

        /// <summary> 使用自动复用的 FrameLock 窗口，在指定的函数地址调用 CallInjected64。</summary>
        public void Call(IntPtr ptr, params object[] args)
        {
            var memory = GetMemory();
            var scheduler = GetScheduler();
            ValidateFunctionPointer(ptr);

            var normalizedArgs = NormalizeArgs(args);
            scheduler.Execute(() => _ = WrapCallInjected64<IntPtr>(memory, ptr, normalizedArgs));
        }

        /// <summary> 使用自动复用的 FrameLock 窗口，在指定的函数地址调用 CallInjected64。</summary>
        public T Call<T>(IntPtr ptr, params object[] args) where T : struct
        {
            var memory = GetMemory();
            var scheduler = GetScheduler();
            ValidateFunctionPointer(ptr);

            T result = default;
            var normalizedArgs = NormalizeArgs(args);
            scheduler.Execute(() => result = WrapCallInjected64<T>(memory, ptr, normalizedArgs));
            return result;
        }

        private static T WrapCallInjected64<T>(ExternalProcessMemory memory, IntPtr ptr, object[] args) where T : struct
        {
            try
            {
                return memory.CallInjected64<T>(ptr, args);
            }
            catch (Exception ex) when (IsInjectionFinishedEventNeverFired(ex))
            {
                throw new Exception(L.Get("PostNamazu/greyMagicInjectionNoResponse"), ex);
            }
        }

        private static bool IsInjectionFinishedEventNeverFired(Exception ex)
        {
            while (ex != null)
            {
                if (ex.Message?.Contains("InjectionFinishedEvent was never fired") == true)
                    return true;

                ex = ex.InnerException;
            }

            return false;
        }

        private ExternalProcessMemory GetMemory() => Memory 
                ?? throw new InvalidOperationException(L.Get("PostNamazu/greyMagicMemoryNotInitialized"));

        private GreyMagicCallScheduler GetScheduler() => CallScheduler 
                ?? throw new InvalidOperationException(L.Get("PostNamazu/greyMagicCallSchedulerNotInitialized"));

        private static void ValidateFunctionPointer(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException(L.Get("PostNamazu/greyMagicFunctionPointerNull"), nameof(ptr));
        }

        private static object[] NormalizeArgs(object[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var normalized = new object[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i]
                    ?? throw new ArgumentException(L.Get("PostNamazu/greyMagicArgumentNull", i), nameof(args));

                normalized[i] = NormalizeArg(arg);
            }

            return normalized;
        }

        /// <summary>
        /// 防止多处 Call 使用的参数类型不一致，检查缓存时因类型变动而调用失败。
        /// 同时兼容了原本不支持的 enum、bool、UIntPtr 类型。
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static object NormalizeArg(object arg)
        {
            var type = arg.GetType();

            if (type.IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(type);
                var value = Convert.ChangeType(arg, underlyingType);
                return NormalizeArg(value);
            }

            if (arg is bool b)
                return (byte)(b ? 1 : 0);

            if (arg is UIntPtr u)
                return u.ToUInt64();

            // GreyMagic 传参时，将参数固定写为 8 字节。
            // 如对于 0xFFFFFFFF：
            //   当做 int (-1) 时写为 0xFFFFFFFF_FFFFFFFF；
            //   当做 uint (0xFFFFFFFF) 时写为 0x00000000_FFFFFFFF。
            // 但目标函数按 32 位参数读取时，高位差异不会影响结果。

            // 所以这里不区分有符号和无符号的整数，统一转换，防止调用时因类型不一致而失败。
            // 这是为了稳定 GreyMagic CachedCall 的参数类型；调用方仍需保证传入值的宽度符合目标函数签名。

            if (arg is sbyte sb)
                return unchecked((byte)sb);

            if (arg is short s)
                return unchecked((ushort)s);

            if (arg is char c)
                return (ushort)c;

            if (arg is int i)
                return unchecked((uint)i);

            if (arg is long l)
                return unchecked((ulong)l);

            if (arg is byte ||
                arg is ushort ||
                arg is uint ||
                arg is ulong ||
                arg is IntPtr ||
                arg is float ||
                arg is double)
            {
                return arg;
            }

            throw new ArgumentException(
                L.Get("PostNamazu/greyMagicUnsupportedArgumentType", type.Name),
                nameof(arg));
        }
    }
}
