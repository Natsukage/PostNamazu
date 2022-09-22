using System;
using System.Collections.Generic;

namespace PostNamazu.Common
{
    class SwitchParser
    {
        private readonly SigScanner MemoryService;
        private readonly List<IntPtr> jumpAddress;

        public SwitchParser(SigScanner memoryService, string sig, ushort offset = 0)
        {
            MemoryService = memoryService;
            jumpAddress = new List<IntPtr>();
            var jumpTableAddress = MemoryService.ScanText(sig);
            var jumpTableIndexPtr = MemoryService._baseAddress + MemoryService.Read<Int32>(jumpTableAddress + offset);
            GetCases(jumpTableIndexPtr);

        }

        public IntPtr Case(ushort i)
        {
            IntPtr Address = IntPtr.Zero;
            if (i < jumpAddress.Count)
                Address = jumpAddress[i];
            return Address;
        }

        private void GetCases(IntPtr jumpTableIndexPtr)
        {
            ushort maxTry = 0x50;
            ushort tryNum = 0;
            while (tryNum < maxTry)
            {
                var CaseOffset = MemoryService.Read<uint>(jumpTableIndexPtr);
                if (CaseOffset == 0xCCCCCCCC)
                    break;
                var address = MemoryService._baseAddress + (int)CaseOffset;
                jumpAddress.Add(address);
                tryNum++;
                jumpTableIndexPtr += 4;
            }

        }
    }
}
