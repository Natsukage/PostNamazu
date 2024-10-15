﻿using Advanced_Combat_Tracker;
using Newtonsoft.Json;
using PostNamazu.Attributes;
using PostNamazu.Common;
using PostNamazu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using static PostNamazu.Common.I18n;

namespace PostNamazu.Actions
{
    public class WayMark : NamazuModule
    {
        private WayMarks tempMarks; //暂存场地标点
        public IntPtr Waymarks;
        public IntPtr MarkingController;
        public IntPtr ExecuteCommandPtr;

        public override void GetOffsets()
        {
            base.GetOffsets();
            MarkingController = SigScanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? 4C 8B 85", 3);
            // 41 D1 C0 88 81 ? ? ? ? 8B 42 04 
            Waymarks = MarkingController + 0x1E0;
            ExecuteCommandPtr = SigScanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 8B E9 41 8B D9 48 8B 0D ?? ?? ?? ?? 41 8B F8 8B F2");
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
                    PluginUI.Log(GetLocalizedString("Reset"));
                    break;
                case "clear":
                    DoWaymarks(new WayMarks { A = new Waymark(), B = new Waymark(), C = new Waymark(), D = new Waymark(), One = new Waymark(), Two = new Waymark(), Three = new Waymark(), Four = new Waymark() });
                    PluginUI.Log(GetLocalizedString("Clear"));
                    break;
                case "public":
                    if (GetInCombat() == true)
                    {
                        Log(GetLocalizedString("InCombat"));
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
                            if (waymarks.Log) Log(GetLocalizedString("InCombat"));
                            return;
                        }
                        Public(waymarks);
                    }
                    if (waymarks.Log)
                    {
                        WayMarks.SetWaymarkIds(waymarks);
                        Log(GetLocalizedString(waymarks.LocalOnly ? "Local" : "Public", waymarks.ToString()));
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
                PluginUI.Log(GetLocalizedString("Save"));
            }
            catch (Exception ex) {
                throw new Exception(GetLocalizedString("SaveException", ex.Message));
            }

        }

        public WayMarks ReadCurrentWaymarks()
        {
            CheckBeforeExecution();
            Waymark ReadWaymark(IntPtr addr, WaymarkID id) => new()
            {
                X = Memory.Read<float>(addr),
                Y = Memory.Read<float>(addr + 0x4),
                Z = Memory.Read<float>(addr + 0x8),
                Active = Memory.Read<byte>(addr + 0x1C) == 1,
                ID = id
            };
            var waymarks = new WayMarks();
            waymarks.A = ReadWaymark(Waymarks + 0x00, WaymarkID.A);
            waymarks.B = ReadWaymark(Waymarks + 0x20, WaymarkID.B);
            waymarks.C = ReadWaymark(Waymarks + 0x40, WaymarkID.C);
            waymarks.D = ReadWaymark(Waymarks + 0x60, WaymarkID.D);
            waymarks.One = ReadWaymark(Waymarks + 0x80, WaymarkID.One);
            waymarks.Two = ReadWaymark(Waymarks + 0xA0, WaymarkID.Two);
            waymarks.Three = ReadWaymark(Waymarks + 0xC0, WaymarkID.Three);
            waymarks.Four = ReadWaymark(Waymarks + 0xE0, WaymarkID.Four);
            return waymarks;
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

        /// <summary> 将指定标点标记为公开标点。 </summary>
        /// <param name="waymarks">标点，传入 null 时清空标点，单个标点为 null 时忽略。</param>
        public void Public(WayMarks waymarks)
        {
            var assemblyLock = Memory.Executor.AssemblyLock;
            var flag = false;
            try
            {
                Monitor.Enter(assemblyLock, ref flag);
                if (waymarks?.FirstOrDefault(waymark => waymark?.Active != false) == null) 
                {   // clear all
                    Memory.CallInjected64<IntPtr>(ExecuteCommandPtr, 313, 0, 0, 0, 0);
                    if (waymarks == null)
                    {
                        Log(GetLocalizedString("ClearPublic"));
                    }
                }
                else
                {
                    int idx = 0;
                    foreach (var waymark in waymarks)
                    {
                        if (waymark == null) continue;
                        if (waymark.Active)
                        {   // mark single
                            Memory.CallInjected64<IntPtr>(ExecuteCommandPtr, 317, idx, (int)(waymark.X * 1000), (int)(waymark.Y * 1000), (int)(waymark.Z * 1000));
                            
                        }
                        else
                        {   // clear single
                            Memory.CallInjected64<IntPtr>(ExecuteCommandPtr, 318, idx, 0, 0, 0);
                        }
                        idx++;
                    }
                }
            }
            finally
            {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }

        public bool GetInCombat()
        {
            var op = ActGlobals.oFormActMain.ActPlugins
                .FirstOrDefault(x => x.pluginObj?.GetType().ToString() == "RainbowMage.OverlayPlugin.PluginLoader")?.pluginObj;
            try
            {
                object pluginMain = op.GetType().GetField("pluginMain", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(op);
                object container = pluginMain.GetType().GetField("_container", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(pluginMain);
                Type inCombatMemoryType = Type.GetType("RainbowMage.OverlayPlugin.MemoryProcessors.InCombat.IInCombatMemory, OverlayPlugin.Core");
                MethodInfo resolveMethodGeneric = container.GetType().GetMethod("Resolve", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                MethodInfo resolveMethodSpecific = resolveMethodGeneric.MakeGenericMethod(inCombatMemoryType);
                object inCombatMemoryManager = resolveMethodSpecific.Invoke(container, null);
                MethodInfo getInCombatMethod = inCombatMemoryManager.GetType().GetMethod("GetInCombat");
                return (bool)getInCombatMethod.Invoke(inCombatMemoryManager, null);
            }
            catch (Exception ex)
            {
                Log(GetLocalizedString("GetInCombatFail", ex.ToString()));
                return false;
            }
        }

        protected override Dictionary<string, Dictionary<Language, string>> LocalizedStrings { get; } = new()
        {
            ["Clear"] = new()            
            {
                [Language.EN] = "Waymarks: cache restored",
                [Language.CN] = "场地标点: 已本地清除所有标点。"
            },
            ["ClearPublic"] = new()
            {
                [Language.EN] = "Waymarks: cache restored",
                [Language.CN] = "场地标点: 已公开清除所有标点。"
            },
            ["GetInCombatFail"] = new()
            {
                [Language.EN] = "Waymarks: Failed to obtain InCombat status from OverlayPlugin: \n{0}",
                [Language.CN] = "从 OverlayPlugin 获取战斗状态失败：\n{0}"
            },
            ["InCombat"] = new()
            {
                [Language.EN] = "Waymarks: Currently in combat, unable to place public waymarks.",
                [Language.CN] = "场地标点: 当前处于战斗状态，无法公开标点。"
            },
            ["Load"] = new()
            {
                [Language.EN] = "Waymarks: cache restored",
                [Language.CN] = "场地标点: 已本地恢复暂存的标点。"
            },
            ["Local"] = new()
            {
                [Language.EN] = "Waymarks: Local mark: \n{0}",
                [Language.CN] = "场地标点: 本地标记 \n{0}"
            },
            ["Public"] = new()
            {
                [Language.EN] = "Waymarks: Public mark: \n{0}",
                [Language.CN] = "场地标点: 公开标记 \n{0}"
            },
            ["Reset"] = new()
            {
                [Language.EN] = "Waymarks: cache cleared",
                [Language.CN] = "场地标点: 已清除暂存的标点。"
            },
            ["Save"] = new()
            {
                [Language.EN] = "Waymarks: current waymarks saved to cache",
                [Language.CN] = "场地标点: 已暂存当前标点。"
            },
            ["SaveException"] = new()
            {
                [Language.EN] = "Waymarks: Exception occurred when saving waymarks: \n{0}",
                [Language.CN] = "场地标点: 保存标记错误：\n{0}"
            },
        };
    }
}