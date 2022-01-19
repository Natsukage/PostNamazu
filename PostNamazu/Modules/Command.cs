using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GreyMagic;
using PostNamazu.Attributes;
using PostNamazu.Modules;

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
            ProcessChatBoxPtr = SigScanner.ScanText("E8 ? ? ? ? 48 8D 4C 24 ? E8 ? ? ? ? C7 07 ? ? ? ? 48 8B C7 48 8B 8C 24");

            UiModulePtr = SigScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8B D9 8B 40 14 85 C0");
            ModuleOffsetPtr = SigScanner.ScanText("48 8D 8F ?? ?? ?? ?? 4C 8B C7 48 8D 54 24 ??") + 3;
            ModuleOffset = SigScanner.ReadInt32(ModuleOffsetPtr);
        }

        /// <summary>
        ///     执行给出的文本指令
        /// </summary>
        /// <param name="command">文本指令</param>
        [Command("command"), Command("DoTextCommand")]
        public void DoTextCommand(string command)
        {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            PluginUI.Log(command);
            if (command == "")
                throw new Exception("指令为空");

            var assemblyLock = Memory.Executor.AssemblyLock;

            var flag = false;
            try {
                Monitor.Enter(assemblyLock, ref flag);
                var array = Encoding.UTF8.GetBytes(command);
                using (AllocatedMemory allocatedMemory = Memory.CreateAllocatedMemory(400), allocatedMemory2 = Memory.CreateAllocatedMemory(array.Length + 30)) {
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
                    _ = Memory.CallInjected64<int>(ProcessChatBoxPtr, RaptureModule,
                        allocatedMemory.Address, UiModule);
                }
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }
    }
}
