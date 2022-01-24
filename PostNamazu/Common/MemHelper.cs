using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace PostNamazu.Common
{
    public unsafe class MemHelper
    {
        #region imports

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool ReadProcessMemory(IntPtr hProcess,
            [Out] [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr MoveMemory(byte* dest, byte* src, int count);

        #endregion

        public Process target;

        public MemHelper(Process p)
        {
            target = p;
            BaseAddress = target.MainModule.BaseAddress;
        }

        public IntPtr BaseAddress { get; }

        public byte[] ReadBytes(IntPtr address, int count)
        {
            var bytes = new byte[count];
            ReadProcessMemory(target.Handle, address, bytes, count, out var read);
            return bytes;
        }

        public T[] Read<T>(IntPtr address, int count) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
            {
                var ptr = Marshal.AllocHGlobal(SizeCache<T>.Size * count);
                Marshal.Copy(ReadBytes(address, SizeCache<T>.Size * count), 0, ptr, SizeCache<T>.Size * count);
                var arr = new T[count];
                // Unfortunate part of the marshaler, is that each instance needs to be pulled in separately.
                // Can't just do a bulk memcpy.
                for (var i = 0; i < count; i++) arr[i] = Marshal.PtrToStructure<T>(ptr + SizeCache<T>.Size * i);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }

            if (count == 0) return new T[0];

            var ret = new T[count];
            fixed (byte* pB = ReadBytes(address, SizeCache<T>.Size * count))
            {
                var genericPtr = (byte*) SizeCache<T>.GetUnsafePtr(ref ret[0]);
                MoveMemory(genericPtr, pB, SizeCache<T>.Size * count);
            }

            return ret;
        }


        public T Read<T>(IntPtr address) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
            {
                var ptr = Marshal.AllocHGlobal(SizeCache<T>.Size);
                Marshal.Copy(ReadBytes(address, SizeCache<T>.Size), 0, ptr, SizeCache<T>.Size);
                var mret = Marshal.PtrToStructure<T>(ptr);
                Marshal.FreeHGlobal(ptr);
                return mret;
            }

            // OPTIMIZATION!
            var ret = new T();
            fixed (byte* b = ReadBytes(address, SizeCache<T>.Size))
            {
                var tPtr = (byte*) SizeCache<T>.GetUnsafePtr(ref ret);
                MoveMemory(tPtr, b, SizeCache<T>.Size);
            }

            return ret;
        }
    }
}