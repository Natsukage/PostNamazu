using System;
using System.Text;
using PostNamazu.Attributes;
using PostNamazu.Common;
using PostNamazu.Common.Localization;
using GreyMagic;
#pragma warning disable CS0649 // 从未对字段赋值，字段将一直保持其默认值

namespace PostNamazu.Actions
{
    internal class Command : NamazuModule
    {
        private IntPtr ProcessChatBoxPtr;
        private IntPtr GetUiModulePtr;

        // 本地化字符串定义
        [LocalizationProvider("Command")]
        private static class Localizations
        {
            [Localized("To avoid sending wrong text to public channels, only commands starting with \"/\" are permitted. Add the prefix \"{0}\" to post to the current channel.",
                       "为防止误操作导致错误文本发送至公共频道，仅允许以 \"/\" 开头的指令。如需发送至当前频道，请加前缀 \"{0}\"。")]
            public static readonly string NoChannelError;
        }

        public override void GetOffsets()
        {
            base.GetOffsets();
            // 函数本体：40 53 56 57 48 83 EC ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B 02 49 8B F0 ...
            try // 7.3
            {
                ProcessChatBoxPtr = SigScanner.ScanText("FF 52 ? 4C 8B C7 48 8D 54 24 ? 48 8B C8 E8 * * * *", nameof(ProcessChatBoxPtr));
            }
            catch // 7.2
            {
                ProcessChatBoxPtr = SigScanner.ScanText("E8 * * * * FE 86 ? ? ? ? C7 86", nameof(ProcessChatBoxPtr));
            }
            
            GetUiModulePtr = SigScanner.ScanText("E8 * * * * 80 7B 1D 01", nameof(GetUiModulePtr));
        }

        void CheckChannel(ref string command)
        {
            if (!command.StartsWith("/"))
            {
                throw new ArgumentException(L.Get("Command/NoChannelError", Constants.CurrentChannelPrefix));
            }
            if (command.StartsWith(Constants.CurrentChannelPrefix))
            {
                command = command.Substring(Constants.CurrentChannelPrefix.Length);
            }
        }

        /// <summary>
        ///     执行给出的文本指令
        /// </summary>
        /// <param name="command">文本指令</param>
        [Command("command")] [Command("DoTextCommand")]
        public void DoTextCommand(string command)
        {
            CheckBeforeExecution(command);
            CheckChannel(ref command);
            PluginUI.Log(command);

            ExecuteWithLock(() =>
            {
                var array = Encoding.UTF8.GetBytes(command);
                using AllocatedMemory allocatedMemory = Memory.CreateAllocatedMemory(Constants.MemoryAllocationSize), 
                      allocatedMemory2 = Memory.CreateAllocatedMemory(array.Length + Constants.CommandBufferSize);
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
            });
        }
    }
}