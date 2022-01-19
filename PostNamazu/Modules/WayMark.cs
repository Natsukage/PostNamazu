using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PostNamazu.Attributes;
using PostNamazu.Models;

namespace PostNamazu.Modules
{
    internal class WayMark : NamazuModule
    {
        private WayMarks tempMarks; //暂存场地标点\
        private IntPtr Waymarks;
        private IntPtr MarkingController;


        public override void GetOffsets()
        {
            base.GetOffsets();
            MarkingController = SigScanner.GetStaticAddressFromSig("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01");
            Waymarks = MarkingController + 432;
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
        [Command("place"), Command("DoWaymarks")]
        public void DoWaymarks(string waymarksStr)
        {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            switch (waymarksStr.ToLower()) {
                case "save":
                case "backup":
                    SaveWaymark();
                    break;
                case "load":
                case "restore":
                    LoadWaymark();
                    break;
                default:
                    var waymarks = JsonConvert.DeserializeObject<WayMarks>(waymarksStr);
                    PluginUI.Log(waymarksStr);
                    DoWaymarks(waymarks);
                    break;
            }
        }

        /// <summary>
        ///     暂存当前标点
        /// </summary>
        public void SaveWaymark(string paylaod) {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            SaveWaymark();
        }

        public void SaveWaymark()
        {
            tempMarks = new WayMarks();

            Waymark ReadWaymark(IntPtr addr, WaymarkID id) => new Waymark
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
                PluginUI.Log("暂存当前标点");
            }
            catch (Exception ex) {
                throw new Exception("保存标记错误：" + ex.Message);
            }

        }

        /// <summary>
        ///     恢复暂存标点
        /// </summary>
        public void LoadWaymark(string paylaod)
        {
            LoadWaymark();
        }

        public void LoadWaymark()
        {
            if (tempMarks == null)
                return;
            DoWaymarks(tempMarks);
            PluginUI.Log("恢复暂存标点");
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

            var markAddr = IntPtr.Zero;
            switch (wId) {
                case (int)WaymarkID.A:
                    markAddr = Waymarks + 0x00;
                    break;
                case (int)WaymarkID.B:
                    markAddr = Waymarks + 0x20;
                    break;
                case (int)WaymarkID.C:
                    markAddr = Waymarks + 0x40;
                    break;
                case (int)WaymarkID.D:
                    markAddr = Waymarks + 0x60;
                    break;
                case (int)WaymarkID.One:
                    markAddr = Waymarks + 0x80;
                    break;
                case (int)WaymarkID.Two:
                    markAddr = Waymarks + 0xA0;
                    break;
                case (int)WaymarkID.Three:
                    markAddr = Waymarks + 0xC0;
                    break;
                case (int)WaymarkID.Four:
                    markAddr = Waymarks + 0xE0;
                    break;
            }

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

    }
}
