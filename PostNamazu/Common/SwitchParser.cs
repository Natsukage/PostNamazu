using System;
using System.Collections.Generic;

namespace PostNamazu.Common
{
    internal class SwitchParser
    {
        private readonly SigScanner _memoryService;
        private readonly List<IntPtr> _jumpAddress;

        public SwitchParser(SigScanner memoryService, string sig, ushort offset = 0)
        {
            _memoryService = memoryService;
            _jumpAddress = new List<IntPtr>();
            var jumpTableAddress = _memoryService.ScanText(sig);
            var jumpTableIndexPtr = _memoryService._baseAddress + _memoryService.Read<int>(jumpTableAddress + offset);
            GetCases(jumpTableIndexPtr);

        }

        public IntPtr Case(ushort i)
        {
            var address = IntPtr.Zero;
            if (i < _jumpAddress.Count)
                address = _jumpAddress[i];
            return address;
        }

        private void GetCases(IntPtr jumpTableIndexPtr)
        {
            const ushort maxTry = 0x50;
            ushort tryNum = 0;
            while (tryNum < maxTry)
            {
                var caseOffset = _memoryService.Read<uint>(jumpTableIndexPtr);
                if (caseOffset == 0xCCCCCCCC)
                    break;
                var address = _memoryService._baseAddress + (int)caseOffset;
                _jumpAddress.Add(address);
                tryNum++;
                jumpTableIndexPtr += 4;
            }

        }
    }
}
