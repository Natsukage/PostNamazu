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
        public IntPtr MarkingFunc { get; }
        public IntPtr UiModulePtr { get; }
        public IntPtr UiModule => _scanner.ReadIntPtr(_scanner.ReadIntPtr(UiModulePtr));
        public IntPtr ModuleOffsetPtr { get; }
        public int ModuleOffset { get; }
        public IntPtr RaptureModule => UiModule + ModuleOffset;
        public IntPtr MarkingController { get; }
        public IntPtr Waymarks => MarkingController + 432;
        public Offsets(SigScanner scanner) {
            _scanner = scanner;
            //Compatible with some plugins of Dalamud
            //ProcessChatBoxPtr = _scanner.ScanText("40 53 56 57 48 83 EC 70 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B 02");
            ProcessChatBoxPtr = _scanner.ScanText("E8 ? ? ? ? 48 8D 4C 24 ? E8 ? ? ? ? E9 ? ? ? ? 49 8B CE");
            UiModulePtr = _scanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8B D9 8B 40 14 85 C0");
            ModuleOffsetPtr = _scanner.ScanText("48 8D 8F ?? ?? ?? ?? 4C 8B C7 48 8D 54 24 ??") + 3;
            ModuleOffset = _scanner.ReadInt32(ModuleOffsetPtr);
            //uiModule = scanner.ReadIntPtr(scanner.ReadIntPtr(uiModulePtr));
            //raptureModule = uiModule + moduleOffset;
            MarkingController = _scanner.GetStaticAddressFromSig("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01");

            //char __fastcall sub_1407A6A60(__int64 g_MarkingController, __int64 MarkType, __int64 ActorID)
            MarkingFunc = _scanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 57 48 83 EC ?? 8D 42");

            //Waymarks = MarkingController + 432;
        }
    }
}
