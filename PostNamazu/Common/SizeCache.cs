using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PostNamazu.Common
{
    /// <summary>
    ///     A cheaty way to get very fast "Marshal.SizeOf" support without the overhead of the Marshaler each time.
    ///     Also provides a way to get the pointer of a generic type (useful for fast memcpy and other operations)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class SizeCache<T> where T : struct
    {
        /// <summary> The size of the Type </summary>
        public static readonly int Size;

        /// <summary> The real, underlying type. </summary>
        public static readonly Type Type;

        /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
        public static readonly bool TypeRequiresMarshal;

        private static int[] _fieldsizes;

        public static int[] FieldSizes {
            get {

                if (_fieldsizes != null)
                    return _fieldsizes;


                var fields = Type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _fieldsizes = new int[fields.Length];
                int i = 0;
                foreach (var field in fields) {
                    var attr = field.GetCustomAttributes(typeof(FixedBufferAttribute), false);

                    if (attr.Length > 0) {
                        var fba = (FixedBufferAttribute)attr[0];
                        var size = GetSizeOf(fba.ElementType) * fba.Length;
                        _fieldsizes[i] = size;
                    }
                    else {
                        var size = GetSizeOf(field.FieldType);
                        _fieldsizes[i] = size;
                    }

                    i++;
                }


                return _fieldsizes;
            }
        }

        public static readonly GetUnsafePtrDelegate GetUnsafePtr;

        static SizeCache() {
            Type = typeof(T);
            // Bools = 1 char.
            if (typeof(T) == typeof(bool)) {
                Size = 1;
            }
            else if (typeof(T).IsEnum) {
                Type = typeof(T).GetEnumUnderlyingType();
                Size = GetSizeOf(Type);
            }
            else {
                Size = GetSizeOf(Type);
            }

            TypeRequiresMarshal = GetRequiresMarshal(Type);

            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod(string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")),
                typeof(void*),
                new[] { typeof(T).MakeByRefType() },
                typeof(SizeCache<>).Module);

            ILGenerator generator = method.GetILGenerator();

            // ldarg 0
            generator.Emit(OpCodes.Ldarg_0);
            // (IntPtr)arg0
            generator.Emit(OpCodes.Conv_U);
            // ret arg0
            generator.Emit(OpCodes.Ret);
            GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
        }

        private static int GetSizeOf(Type t) {
            try {
                // Try letting the marshaler handle getting the size.
                // It can *sometimes* do it correctly
                // If it can't, fall back to our own methods.
                var o = Activator.CreateInstance(t);
                return Marshal.SizeOf(o);
            }
            catch (Exception) {
                int totalSize = 0;
                var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields) {
                    var attr = field.GetCustomAttributes(typeof(FixedBufferAttribute), false);

                    if (attr.Length > 0) {
                        var fba = (FixedBufferAttribute)attr[0];
                        totalSize += GetSizeOf(fba.ElementType) * fba.Length;
                        continue;
                    }

                    totalSize += GetSizeOf(field.FieldType);
                }
                return totalSize;
            }
        }

        private static bool GetRequiresMarshal(Type t) {
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                var requires = field.GetCustomAttributes(typeof(MarshalAsAttribute), true).Length != 0;

                if (requires) {
                    return true;
                }

                if (t == typeof(IntPtr)) {
                    continue;
                }

                if (Type.GetTypeCode(t) == TypeCode.Object) {
                    requires |= GetRequiresMarshal(field.FieldType);
                }

                return requires;
            }
            return false;
        }

        #region Nested type: GetUnsafePtrDelegate

        public unsafe delegate void* GetUnsafePtrDelegate(ref T value);

        #endregion
    }
}