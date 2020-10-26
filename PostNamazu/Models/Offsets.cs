using System;

namespace PostNamazu.Models
{
    /// <summary>
    ///     获取游戏对应的Offsets
    /// </summary>
    public class Offsets
    {
        private readonly SigScanner _scanner;
        public IntPtr ProcessChatBoxPtr { get; }
        public IntPtr UiModulePtr { get; }
        public IntPtr UiModule => _scanner.ReadIntPtr(_scanner.ReadIntPtr(UiModulePtr));
        public IntPtr ModuleOffsetPtr { get; }
        public int ModuleOffset { get; }
        public IntPtr RaptureModule => UiModule + ModuleOffset;
        public IntPtr Waymarks { get; }
        public Offsets(SigScanner scanner) {
            _scanner = scanner;
            ProcessChatBoxPtr = _scanner.ScanText("40 53 56 57 48 83 EC 70 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B 02");
            UiModulePtr = _scanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8B D9 8B 40 14 85 C0");
            ModuleOffsetPtr = _scanner.ScanText("48 8D 8F ?? ?? ?? ?? 4C 8B C7 48 8D 54 24 ??") + 3;
            ModuleOffset = _scanner.ReadInt32(ModuleOffsetPtr);
            //uiModule = scanner.ReadIntPtr(scanner.ReadIntPtr(uiModulePtr));
            //raptureModule = uiModule + moduleOffset;
            Waymarks = _scanner.GetStaticAddressFromSig("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01")+ 432;
        }
    }
}
