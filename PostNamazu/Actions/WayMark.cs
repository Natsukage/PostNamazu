using System;
using PostNamazu.Attributes;
using PostNamazu.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using static PostNamazu.Common.I18n;

namespace PostNamazu.Actions
{
    internal class WayMark : NamazuModule
    {
        private WayMarks tempMarks; //暂存场地标点
        private IntPtr Waymarks;
        private IntPtr MarkingController;


        public override void GetOffsets()
        {
            base.GetOffsets();
            MarkingController = SigScanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? 4C 8B 85",3);
            // 41 D1 C0 88 81 ? ? ? ? 8B 42 04 
            Waymarks = MarkingController + 0x1E0;
        }

        /// <summary>
        ///     场地标点
        /// </summary>
        /// <param name="waymarks">标点合集对象</param>
        private void DoWaymarks(WayMarks waymarks)
        {
            WriteWaymark(waymarks.A, 0);
            WriteWaymark(waymarks.B, 1);
            WriteWaymark(waymarks.C, 2);
            WriteWaymark(waymarks.D, 3);
            WriteWaymark(waymarks.One, 4);
            WriteWaymark(waymarks.Two, 5);
            WriteWaymark(waymarks.Three, 6);
            WriteWaymark(waymarks.Four, 7);
        }
        /// <summary>
        ///     场地标点
        /// </summary>
        /// <param name="waymarksStr">标点合集序列化Json字符串</param>
        [Command("place")] [Command("DoWaymarks")]
        public void DoWaymarks(string waymarksStr)
        {
            CheckBeforeExecution(waymarksStr);

            switch (waymarksStr.ToLower().Trim()) {
                case "save":
                case "backup":
                    SaveWaymark();
                    break;
                case "load":
                case "restore":
                    LoadWaymark();
                    break;
                case "reset":
                    tempMarks = null;
                    PluginUI.Log(GetLocalizedString("Reset"));
                    break;
                case "clear":
                    DoWaymarks(new WayMarks { A = new Waymark(), B = new Waymark(), C = new Waymark(), D = new Waymark(), One = new Waymark(), Two = new Waymark(), Three = new Waymark(), Four = new Waymark() });
                    PluginUI.Log("Waymarks: clear");
                    break;
                default:
                    var waymarks = JsonConvert.DeserializeObject<WayMarks>(waymarksStr);
                    if (waymarks.Log)
                    {
                        WayMarks.SetWaymarkIds(waymarks);
                        PluginUI.Log("Waymarks: " + waymarks.ToString());
                    }
                    DoWaymarks(waymarks);
                    break;
            }
        }

        /// <summary>
        ///     暂存当前标点
        /// </summary>
        public void SaveWaymark()
        {
            tempMarks = new WayMarks();

            Waymark ReadWaymark(IntPtr addr, WaymarkID id) => new()
            {
                X = Memory.Read<float>(addr),
                Y = Memory.Read<float>(addr + 0x4),
                Z = Memory.Read<float>(addr + 0x8),
                Active = Memory.Read<byte>(addr + 0x1C) == 1,
                ID = id
            };

            try {
                tempMarks.A = ReadWaymark(Waymarks + 0x00, WaymarkID.A);
                tempMarks.B = ReadWaymark(Waymarks + 0x20, WaymarkID.B);
                tempMarks.C = ReadWaymark(Waymarks + 0x40, WaymarkID.C);
                tempMarks.D = ReadWaymark(Waymarks + 0x60, WaymarkID.D);
                tempMarks.One = ReadWaymark(Waymarks + 0x80, WaymarkID.One);
                tempMarks.Two = ReadWaymark(Waymarks + 0xA0, WaymarkID.Two);
                tempMarks.Three = ReadWaymark(Waymarks + 0xC0, WaymarkID.Three);
                tempMarks.Four = ReadWaymark(Waymarks + 0xE0, WaymarkID.Four);
                PluginUI.Log(GetLocalizedString("Save"));
            }
            catch (Exception ex) {
                throw new Exception(GetLocalizedString("SaveException", ex.Message));
            }

        }

        /// <summary>
        ///     恢复暂存标点
        /// </summary>
        public void LoadWaymark()
        {
            if (tempMarks == null)
                return;
            DoWaymarks(tempMarks);
            PluginUI.Log(GetLocalizedString("Load"));
        }

        /// <summary>
        ///     写入指定标点
        /// </summary>
        /// <param name="waymark">标点</param>
        /// <param name="id">ID</param>
        private void WriteWaymark(Waymark waymark, int id = -1)
        {
            if (waymark == null)
                return;

            var wId = id == -1 ? (byte)waymark.ID : id;

            var markAddr = wId switch
            {
                (int)WaymarkID.A => Waymarks + 0x00,
                (int)WaymarkID.B => Waymarks + 0x20,
                (int)WaymarkID.C => Waymarks + 0x40,
                (int)WaymarkID.D => Waymarks + 0x60,
                (int)WaymarkID.One => Waymarks + 0x80,
                (int)WaymarkID.Two => Waymarks + 0xA0,
                (int)WaymarkID.Three => Waymarks + 0xC0,
                (int)WaymarkID.Four => Waymarks + 0xE0,
                _ => IntPtr.Zero
            };

            // Write the X, Y and Z coordinates
            Memory.Write(markAddr, waymark.X);
            Memory.Write(markAddr + 0x4, waymark.Y);
            Memory.Write(markAddr + 0x8, waymark.Z);

            Memory.Write(markAddr + 0x10, (int)(waymark.X * 1000));
            Memory.Write(markAddr + 0x14, (int)(waymark.Y * 1000));
            Memory.Write(markAddr + 0x18, (int)(waymark.Z * 1000));

            // Write the active state
            Memory.Write(markAddr + 0x1C, (byte)(waymark.Active ? 1 : 0));
        }
        protected override Dictionary<string, Dictionary<Language, string>> LocalizedStrings { get; } = new()
        {
            ["Load"] = new()
            {
                [Language.EN] = "Waymarks: cache restored",
                [Language.CN] = "Waymarks: 已恢复暂存的标点"
            },
            ["Reset"] = new()
            {
                [Language.EN] = "Waymarks: cache cleared",
                [Language.CN] = "Waymarks: 已清除暂存的标点"
            },
            ["Save"] = new()
            {
                [Language.EN] = "Waymarks: current waymarks saved to cache",
                [Language.CN] = "Waymarks: 已暂存当前标点。"
            },
            ["SaveException"] = new()
            {
                [Language.EN] = "Waymarks: Exception occurred when saving waymarks: \n{0}",
                [Language.CN] = "Waymarks: 保存标记错误：\n{0}"
            },
        };
    }
}
