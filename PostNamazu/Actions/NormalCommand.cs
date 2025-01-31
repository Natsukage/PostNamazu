using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PostNamazu.Attributes;
using static PostNamazu.Common.I18n;
using GreyMagic;

namespace PostNamazu.Actions
{
    internal class NormalCommand : NamazuModule
    {
        private IntPtr ProcessChatBoxPtr;
        private IntPtr GetUiModulePtr;

        public override void GetOffsets()
        {
            base.GetOffsets();
            try // 7.1
            {
                ProcessChatBoxPtr = SigScanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B F2 48 8B F9 45 84 C9");
            }
            catch // 7.0
            {
                ProcessChatBoxPtr = SigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9", nameof(ProcessChatBoxPtr));
            }
            GetUiModulePtr = SigScanner.ScanText("E8 * * * * 80 7B 1D 01", nameof(GetUiModulePtr));
        }

        const string CurrentChannelPrefix = "/current ";
        void CheckChannel(ref string command)
        {
            if (!command.StartsWith("/"))
            {
                throw new ArgumentException(GetLocalizedString("NoChannelError"));
            }
            if (command.StartsWith(CurrentChannelPrefix))
            {
                command = command.Substring(CurrentChannelPrefix.Length);
            }
        }

        /// <summary>
        ///     执行给出的文本指令
        /// </summary>
        /// <param name="command">文本指令</param>
        [Command("normalcommand")] [Command("DoNormalTextCommand")]
        public void DoNormalTextCommand(string command)
        {
            CheckBeforeExecution(command);
            CheckChannel(ref command);
            PluginUI.Log(command);

            ExecuteWithLock(() =>
            {
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
                _ = Memory.CallInjected64<int>(ProcessChatBoxPtr, uiModulePtr, allocatedMemory.Address, IntPtr.Zero, (byte)0);
            });
        }

        protected override Dictionary<string, Dictionary<Language, string>> LocalizedStrings { get; } = new()
        {
            ["NoChannelError"] = new()
            {
                [Language.EN] = $"To avoid sending wrong text to public channels, only commands starting with \"/\" are permitted. Add the prefix \"{CurrentChannelPrefix}\" to post to the current channel.",
                [Language.CN] = $"为防止误操作导致错误文本发送至公共频道，仅允许以 \"/\" 开头的指令。如需发送至当前频道，请加前缀 \"{CurrentChannelPrefix}\"。"
            },
        };
    }
}
