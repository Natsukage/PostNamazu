using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PostNamazu.Attributes;
using PostNamazu.Common;
using PostNamazu.Common.Localization;
using PostNamazu.Models;
#pragma warning disable CS0649 // 从未对字段赋值，字段将一直保持其默认值

namespace PostNamazu.Actions
{
    internal class Preset : NamazuModule
    {
        public IntPtr GetUIModulePtr;
        private IntPtr UIModulePtr => Memory.CallInjected64<IntPtr>(GetUIModulePtr, PostNamazu.FrameworkPtr);
        private Int32 WayMarkSlotOffset;

        // 模组使用的是 ContentFinderCondition 而非 Map 或 Territory 的 Id
        public IntPtr ContentFinderConditionIdPtr;

        // FFXIVClientStructs/FFXIV/Client/UI/Misc/FieldMarkerModule
        // [FieldOffset(0x48), FixedSizeArray] internal FixedSizeArray30<FieldMarkerPreset> _presets
        int WaymarkDataOffset = 0x48;

        // FFXIVClientStructs/FFXIV/Client/UI/Misc/FieldMarkerModule
        // [StructLayout(LayoutKind.Explicit, Size = 0x68)] public partial struct FieldMarkerPreset
        int FieldMarkerPresetSize = 0x68;

        // 本地化字符串定义
        [LocalizationProvider("Preset")]
        private static class Localizations
        {
            [Localized("Preset and current map ID are both invalid, loading preset failed.", "预设与当前的地图 ID 均不合法，加载预设失败。")]
            public static readonly string MapIdIllegal;
        }

        public override void GetOffsets()
        {
            base.GetOffsets();
            GetUIModulePtr = SigScanner.ScanText("E8 * * * * 80 7B 1D 01", nameof(GetUIModulePtr));

            // FFXIVClientStructs/FFXIV/Client/Game/GameMain.cs
            IntPtr gameMainPtr;
            gameMainPtr = SigScanner.ScanText("48 8D 0D * * * * 0F B6 55 ?? 24", nameof(gameMainPtr));

            // GameMain.CurrentContentFinderConditionId
            // 7.2/7.3: offset = 0x40AC
            // 进出副本时写入 id：
            // 66 89 81 AC 40 00 00     mov [rcx+40ACh], ax
            ushort contentFinderConditionIdOffset;
            contentFinderConditionIdOffset = SigScanner.Read<ushort>(SigScanner.ScanText("66 89 81 ? ? ? ? 66 85 C0 74 ?", nameof(contentFinderConditionIdOffset)) + 3);

            ContentFinderConditionIdPtr = gameMainPtr + contentFinderConditionIdOffset;
        }

        private void GetWayMarkSlotOffset()
        {
            var UIModuleSwitch = new SwitchParser(SigScanner, "8B 94 98 ?? ?? ?? ?? 48 03 D0 FF E2 49 8B 10", 3);

            //.text:000000014063AE36                        loc_14063AE36:; CODE XREF: sub_14063ACD0 + 32↑j
            //.text:000000014063AE36
            //.text:000000014063AE36 49 8B 00                               mov rax, [r8]; jumptable 000000014063AD02 case 17
            //.text:000000014063AE39 49 8B C8                               mov rcx, r8
            //.text:000000014063AE3C 48 83 C4 20                            add rsp, 20h
            //.text:000000014063AE40 5B                                     pop     rbx
            //.text:000000014063AE41 48 FF A0 70 01 00 00                   jmp qword ptr[rax + 170h]
            var Case0x11 = UIModuleSwitch.Case(0x11);
            var offset = SigScanner.Read<int>(Case0x11 + 14);

            var uiModule = UIModulePtr;
            var FastCallAddressPtr = SigScanner.Read<IntPtr>(uiModule) + offset;

            var FastCallAddress = SigScanner.Read<IntPtr>(FastCallAddressPtr);
            //.text:00000001405BA9A0
            //.text:00000001405BA9A0                        sub_1405BA9A0   proc near; DATA XREF: .rdata: 0000000141649E10↓o
            //.text:00000001405BA9A0 48 8D 81 30 05 09 00                   lea rax, [rcx + 90530h]
            //.text:00000001405BA9A7 C3                                     retn
            //.text:00000001405BA9A7                        sub_1405BA9A0   endp
            WayMarkSlotOffset = SigScanner.Read<Int32>(FastCallAddress + 3);
        }

        public IntPtr GetWaymarkDataPointerForSlot(uint slotNum)
        {
            var uiModulePtr = UIModulePtr;
            var wayMarkSlotPtr = uiModulePtr + WayMarkSlotOffset;
            var waymarkDataPtr = wayMarkSlotPtr + WaymarkDataOffset + (int)(FieldMarkerPresetSize * (slotNum - 1));
            return waymarkDataPtr;
        }

        /// <summary>
        ///     写入预设
        /// </summary>
        /// <param name="waymarks">标点合集对象</param>
        private void DoInsertPreset(WayMarks waymarks)
        {
            if (waymarks.MapID is > 2000 or 0) 
                waymarks.MapID = SigScanner.Read<ushort>(ContentFinderConditionIdPtr);
            if (waymarks.MapID == 0)
            {
                Log(L.Get("Preset/MapIdIllegal"));
                return;
            }
            GetWayMarkSlotOffset();
            
            IntPtr SlotOffset;

            var pattern = @"^Slot\s*(\d+)$";
            var match = Regex.Match(waymarks.Name.Trim(), pattern,RegexOptions.IgnoreCase);

            if (match.Success && uint.TryParse(match.Groups[1].Value, out var slotNum) && slotNum is > 0 and <= 30)
                SlotOffset = GetWaymarkDataPointerForSlot(slotNum);
            else
                SlotOffset = GetWaymarkDataPointerForSlot(1);

            var importdata = ConstructGamePreset(waymarks);
            Memory.WriteBytes(SlotOffset, importdata);
            
        }
        /// <summary>
        ///     写入预设
        /// </summary>
        /// <param name="waymarksStr">标点合集序列化Json字符串</param>
        [Command("preset")] [Command("DoInsertPreset")]
        public void DoInsertPreset(string waymarksStr)
        {
            CheckBeforeExecution(waymarksStr);

            switch (waymarksStr.ToLower()) {
                default:
                    var waymarks = JsonConvert.DeserializeObject<WayMarks>(waymarksStr);
                    if (waymarks.Log)
                    {
                        WayMarks.SetWaymarkIds(waymarks);
                        PluginUI.Log("Preset: " + waymarks.ToString());
                    }
                    DoInsertPreset(waymarks);
                    break;
            }
        }

        /// <summary>
        ///     构造预设结构，从0号头子的PPR抄来的
        /// </summary>
        /// <param name="waymark">标点</param>
        /// <returns>byte[]预设结构</returns>
        public byte[] ConstructGamePreset(WayMarks waymarks)
        {
            /*
            [StructLayout(LayoutKind.Explicit, Size = 0x68)]
            public partial struct FieldMarkerPreset
            {
                [FieldOffset(0x00), FixedSizeArray] internal FixedSizeArray8<GamePresetPoint> _markers;
                [FieldOffset(0x60)] public byte ActiveMarkers;
                [FieldOffset(0x62)] public ushort ContentFinderConditionId;
                [FieldOffset(0x64)] public int Timestamp;
            }
            */

            //	List is easy because we can just push data on to it.
            var byteData = new List<byte>();
            byte activeMask = 0x00;
            foreach (var twaymark in waymarks)
            {
                var waymark = twaymark ?? new Waymark();
                byteData.AddRange(BitConverter.GetBytes(waymark.Active ? (Int32)(waymark.X * 1000.0) : 0));
                byteData.AddRange(BitConverter.GetBytes(waymark.Active ? (Int32)(waymark.Y * 1000.0) : 0));
                byteData.AddRange(BitConverter.GetBytes(waymark.Active ? (Int32)(waymark.Z * 1000.0) : 0));
                activeMask >>= 1;
                if (waymark.Active) activeMask |= 0b10000000;
            }

            // 0x60
            byteData.Add(activeMask);

            // 0x61 Reserved byte.
            byteData.Add((byte)0x00);

            // 0x62 ContentFinderCondition ID.
            byteData.AddRange(BitConverter.GetBytes(waymarks.MapID));

            // 0x64 Time last modified.
            var Time = new DateTimeOffset(DateTimeOffset.Now.UtcDateTime);
            byteData.AddRange(BitConverter.GetBytes((Int32)Time.ToUnixTimeSeconds()));

            //	Shouldn't ever come up with the wrong length, but just in case...
            if (byteData.Count != FieldMarkerPresetSize)
            {
                throw new Exception($"Error in {nameof(ConstructGamePreset)}(): Constructed byte array was of an unexpected length ({byteData.Count}).");
            }

            //	Send it out.
            return byteData.ToArray();
        }
    }
}
