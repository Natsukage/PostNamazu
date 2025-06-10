using Advanced_Combat_Tracker;
using Newtonsoft.Json;
using PostNamazu.Attributes;
using PostNamazu.Common.Localization;
using PostNamazu.Models;
using System;
using System.Linq;
using System.Reflection;
#pragma warning disable CS0649 // 从未对字段赋值，字段将一直保持其默认值

namespace PostNamazu.Actions
{
    public class WayMark : NamazuModule
    {
        private WayMarks tempMarks; //暂存场地标点
        public IntPtr Waymarks;
        public IntPtr MarkingController;
        public IntPtr ExecuteCommandPtr;

        // 本地化字符串定义
        [LocalizationProvider("WayMark")]
        private static class Localizations
        {
            [Localized("Waymarks: cache restored", "场地标点: 已本地清除所有标点。")]
            public static readonly string Clear;

            [Localized("Waymarks: cache restored", "场地标点: 已公开清除所有标点。")]
            public static readonly string ClearPublic;

            [Localized("Waymarks: Failed to obtain InCombat status from OverlayPlugin: \n{0}", 
                       "从 OverlayPlugin 获取战斗状态失败：\n{0}")]
            public static readonly string GetInCombatFail;

            [Localized("Waymarks: Currently in combat, unable to place public waymarks.", 
                       "场地标点: 当前处于战斗状态，无法公开标点。")]
            public static readonly string InCombat;

            [Localized("Waymarks: cache restored", "场地标点: 已本地恢复暂存的标点。")]
            public static readonly string Load;

            [Localized("Waymarks: Local mark: \n{0}", "场地标点: 本地标记 \n{0}")]
            public static readonly string Local;

            [Localized("Waymarks: Public mark: \n{0}", "场地标点: 公开标记 \n{0}")]
            public static readonly string Public;

            [Localized("Waymarks: cache cleared", "场地标点: 已清除暂存的标点。")]
            public static readonly string Reset;

            [Localized("Waymarks: current waymarks saved to cache", "场地标点: 已暂存当前标点。")]
            public static readonly string Save;

            [Localized("Waymarks: Exception occurred when saving waymarks: \n{0}", 
                       "场地标点: 保存标记错误：\n{0}")]
            public static readonly string SaveException;
        }

        public override void GetOffsets()
        {
            base.GetOffsets();
            MarkingController = SigScanner.ScanText("48 8D 0D * * * * 4C 8B 85", nameof(MarkingController));
            Waymarks = MarkingController + 0x1E0;
            try
            {
                ExecuteCommandPtr = SigScanner.ScanText("E8 * * * * 48 83 C4 ?? C3 CC CC CC CC CC CC CC CC CC CC CC CC 48 83 EC ?? 45 0F B6 C0", nameof(ExecuteCommandPtr));
            }
            catch 
            {   // 可能和其他插件冲突，加一个备用
                ExecuteCommandPtr = SigScanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 8B E9 41 8B D9 48 8B 0D ?? ?? ?? ?? 41 8B F8 8B F2", nameof(ExecuteCommandPtr));
            }
        }

        /// <summary>
        ///     场地标点
        /// </summary>
        /// <param name="waymarks">标点合集对象</param>
        internal void DoWaymarks(WayMarks waymarks)
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
                    PluginUI.Log(L.Get("WayMark/Reset"));
                    break;
                case "clear":
                    DoWaymarks(new WayMarks { A = new Waymark(), B = new Waymark(), C = new Waymark(), D = new Waymark(), One = new Waymark(), Two = new Waymark(), Three = new Waymark(), Four = new Waymark() });
                    PluginUI.Log(L.Get("WayMark/Clear"));
                    break;
                case "public":
                    if (GetInCombat() == true)
                    {
                        Log(L.Get("WayMark/InCombat"));
                        return;
                    }
                    Public(ReadCurrentWaymarks());
                    break;
                default:
                    var waymarks = JsonConvert.DeserializeObject<WayMarks>(waymarksStr);
                    if (waymarks.LocalOnly)
                    {
                        DoWaymarks(waymarks);
                    }
                    else
                    {
                        if (GetInCombat() == true)
                        {
                            if (waymarks.Log) Log(L.Get("WayMark/InCombat"));
                            return;
                        }
                        Public(waymarks);
                    }
                    if (waymarks.Log)
                    {
                        WayMarks.SetWaymarkIds(waymarks);
                        Log(L.Get(waymarks.LocalOnly ? "WayMark/Local" : "WayMark/Public", waymarks.ToString()));
                    }
                    break;
            }
        }

        /// <summary>
        ///     暂存当前标点
        /// </summary>
        public void SaveWaymark()
        {
            tempMarks = new WayMarks();

            try {
                tempMarks = ReadCurrentWaymarks();
                PluginUI.Log(L.Get("WayMark/Save"));
            }
            catch (Exception ex) {
                throw new Exception(L.Get("WayMark/SaveException", ex.Message));
            }

        }

        public WayMarks ReadCurrentWaymarks()
        {
            CheckBeforeExecution();
            var waymarks = new WayMarks
            {
                A = ReadWaymark(Waymarks + 0x00, WaymarkID.A),
                B = ReadWaymark(Waymarks + 0x20, WaymarkID.B),
                C = ReadWaymark(Waymarks + 0x40, WaymarkID.C),
                D = ReadWaymark(Waymarks + 0x60, WaymarkID.D),
                One = ReadWaymark(Waymarks + 0x80, WaymarkID.One),
                Two = ReadWaymark(Waymarks + 0xA0, WaymarkID.Two),
                Three = ReadWaymark(Waymarks + 0xC0, WaymarkID.Three),
                Four = ReadWaymark(Waymarks + 0xE0, WaymarkID.Four)
            };
            return waymarks;

            static Waymark ReadWaymark(IntPtr addr, WaymarkID id) => new()
            {
                X = Memory.Read<float>(addr),
                Y = Memory.Read<float>(addr + 0x4),
                Z = Memory.Read<float>(addr + 0x8),
                Active = Memory.Read<byte>(addr + 0x1C) == 1,
                ID = id
            };
        }

        /// <summary>
        ///     恢复暂存标点
        /// </summary>
        public void LoadWaymark()
        {
            if (tempMarks == null)
                return;
            DoWaymarks(tempMarks);
            PluginUI.Log(L.Get("WayMark/Load"));
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

        /// <summary> 将指定标点标记为公开标点。 </summary>
        /// <param name="waymarks">标点，传入 null 时清空标点，单个标点为 null 时忽略。</param>
        public void Public(WayMarks waymarks)
        {
            ExecuteWithLock(() => 
            {
                if (waymarks == null || waymarks.All(waymark => waymark?.Active == false))
                {   // clear all
                    ExecuteCommand(313);
                    if (waymarks == null)
                    {
                        Log(L.Get("WayMark/ClearPublic"));
                    }
                }
                else
                {
                    var idx = -1;
                    foreach (var waymark in waymarks)
                    {
                        idx++;
                        if (waymark == null) continue;
                        if (waymark.Active)
                        {   // mark single
                            ExecuteCommand(317, (uint)idx, UIntEncode(waymark.X), UIntEncode(waymark.Y), UIntEncode(waymark.Z));
                        }
                        else
                        {   // clear single
                            ExecuteCommand(318, (uint)idx);
                        }
                    }
                }
            });
        }

        private uint UIntEncode(float x) => (uint)(x * 1000);

        // 统一使用 uint 调用此内部函数（参数常用于传入 id 等，uint 相比于 int 更合理）
        // 防止 GreyMagic 多次调用时参数类型不一致报错
        private void ExecuteCommand(uint command, uint a1 = 0, uint a2 = 0, uint a3 = 0, uint a4 = 0)
            => Memory.CallInjected64<IntPtr>(ExecuteCommandPtr, command, a1, a2, a3, a4);

        public bool GetInCombat()
        {
            var op = ActGlobals.oFormActMain.ActPlugins
                .FirstOrDefault(x => x.pluginObj?.GetType().ToString() == "RainbowMage.OverlayPlugin.PluginLoader")?.pluginObj;
            try
            {
                var pluginMain = op.GetType().GetField("pluginMain", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(op);
                var container = pluginMain.GetType().GetField("_container", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(pluginMain);
                var inCombatMemoryType = Type.GetType("RainbowMage.OverlayPlugin.MemoryProcessors.InCombat.IInCombatMemory, OverlayPlugin.Core");
                var resolveMethodGeneric = container.GetType().GetMethod("Resolve", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                var resolveMethodSpecific = resolveMethodGeneric.MakeGenericMethod(inCombatMemoryType);
                var inCombatMemoryManager = resolveMethodSpecific.Invoke(container, null);
                var getInCombatMethod = inCombatMemoryManager.GetType().GetMethod("GetInCombat");
                return (bool)getInCombatMethod.Invoke(inCombatMemoryManager, null);
            }
            catch (Exception ex)
            {
                Log(L.Get("WayMark/GetInCombatFail", ex.ToString()));
                return false;
            }
        }
    }
}