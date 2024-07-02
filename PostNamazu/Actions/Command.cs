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
        private IntPtr RaptureModule;

        public override void GetOffsets()
        {
            base.GetOffsets();
            ProcessChatBoxPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? FE 86 ?? ?? ?? ?? C7 86");
            var sigAddress = SigScanner.ScanText("49 8B DC 48 89 1D");
            IntPtr targetAddress = sigAddress + 10 + Memory.Read<int>(sigAddress+6);
            var FrameworkPtr = Memory.Read<IntPtr>(targetAddress);
            var GetUiModulePtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 80 7B 1D 01");
            UiModulePtr = Memory.CallInjected64<IntPtr>(GetUiModulePtr, FrameworkPtr);
            RaptureModule = Memory.CallInjected64<IntPtr>(Memory.Read<IntPtr>(Memory.Read<IntPtr>(UiModulePtr) + (0x8 * 9)), UiModulePtr);
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
                _ = Memory.CallInjected64<int>(ProcessChatBoxPtr, RaptureModule, allocatedMemory.Address, UiModulePtr);
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }
    }
    
}