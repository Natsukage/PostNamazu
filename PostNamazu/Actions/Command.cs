using GreyMagic;
using PostNamazu.Attributes;
using System;
using System.Text;
using System.Threading;

namespace PostNamazu.Actions
{
    internal class Command : NamazuModule
    {
        private IntPtr ProcessChatBoxPtr;
        private IntPtr GetUiModulePtr;

        public override void GetOffsets()
        {
            base.GetOffsets();
            ProcessChatBoxPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? FE 86 ?? ?? ?? ?? C7 86");
            GetUiModulePtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 80 7B 1D 01");
        }

        /// <summary>
        ///     执行给出的文本指令
        /// </summary>
        /// <param name="command">文本指令</param>
        [Command("command")] [Command("DoTextCommand")]
        public void DoTextCommand(string command)
        {
            CheckBeforeExecution(command);
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
                var uiModulePtr = Memory.CallInjected64<IntPtr>(GetUiModulePtr, PostNamazu.FrameworkPtr);
                var raptureModule = Memory.CallInjected64<IntPtr>(Memory.Read<IntPtr>(Memory.Read<IntPtr>(uiModulePtr) + (0x8 * 9)), uiModulePtr);
                _ = Memory.CallInjected64<int>(ProcessChatBoxPtr, raptureModule, allocatedMemory.Address, uiModulePtr);
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }
    }
    
}