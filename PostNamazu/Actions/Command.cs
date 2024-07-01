using System;
using System.Text;
using System.Threading;
using PostNamazu.Attributes;
using GreyMagic;

namespace PostNamazu.Actions
{
    internal class Command : NamazuModule
    {
        private IntPtr ProcessChatBoxPtr;
        private IntPtr UiModulePtr;
        private IntPtr UiModule => SigScanner.ReadIntPtr(SigScanner.ReadIntPtr(UiModulePtr));
        private IntPtr ModuleOffsetPtr;
        private int ModuleOffset;
        private IntPtr RaptureModule => UiModule + ModuleOffset;

        public override void GetOffsets()
        {
            base.GetOffsets();

            //Compatible with some plugins of Dalamud
            //ProcessChatBoxPtr = _scanner.ScanText("40 53 56 57 48 83 EC 70 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B 02");
            ProcessChatBoxPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? FE 86 ?? ?? ?? ?? C7 86");

            //48 83 EC ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 44 24 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0
            
            var FrameworkPtr = Memory.Read<IntPtr>(SigScanner.GetStaticAddressFromSig("49 8B DC 48 89 1D ?? ?? ?? ??", 6));
            var GetUiModulePtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 80 7B 1D 01");
            UiModulePtr = Memory.CallInjected64<IntPtr>(GetUiModulePtr, FrameworkPtr);
            //PluginUI.Log(UiModulePtr);
            UiModulePtr = SigScanner.ScanText("48 83 EC ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0");
            //PluginUI.Log(UiModulePtr);
            ModuleOffsetPtr = SigScanner.ScanText("48 8D 8B ?? ?? ?? ?? C6 83 ?? ?? ?? ?? ?? 4C 8B C3 C7 83 ?? ?? ?? ?? ?? ?? ?? ?? 48 8D 54 24") + 3;
            ModuleOffset = SigScanner.ReadInt32(ModuleOffsetPtr);
        }

        /// <summary>
        ///     执行给出的文本指令
        /// </summary>
        /// <param name="command">文本指令</param>
        [Command("command")] [Command("DoTextCommand")]
        public void DoTextCommand(string command)
        {
            if (!isReady) 
                throw new Exception("没有对应的游戏进程");
            
            if (command == "")
                throw new Exception("指令为空");

            PluginUI.Log(command);

            var assemblyLock = Memory.Executor.AssemblyLock;

            var flag = false;
            try {
                Monitor.Enter(assemblyLock, ref flag);
                var array = Encoding.UTF8.GetBytes(command);
                using AllocatedMemory allocatedMemory = Memory.CreateAllocatedMemory(400), allocatedMemory2 = Memory.CreateAllocatedMemory(array.Length + 30);
                allocatedMemory2.AllocateOfChunk("cmd", array.Length);
                allocatedMemory2.WriteBytes("cmd", array);
                allocatedMemory.AllocateOfChunk<IntPtr>("cmdAddress");
                allocatedMemory.AllocateOfChunk<long>("t1");
                allocatedMemory.AllocateOfChunk<long>("tLength");
                allocatedMemory.AllocateOfChunk<long>("t3");
                allocatedMemory.Write("cmdAddress", allocatedMemory2.Address);
                allocatedMemory.Write("t1", 0x40);
                allocatedMemory.Write("tLength", array.Length + 1);
                allocatedMemory.Write("t3", 0x00);
                _ = Memory.CallInjected64<int>(ProcessChatBoxPtr, RaptureModule, allocatedMemory.Address, UiModule);
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }
    }
}