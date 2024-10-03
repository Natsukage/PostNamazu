﻿using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using PostNamazu.Attributes;
using PostNamazu.Models;
using System.Collections.Generic;
using static PostNamazu.Common.I18n;

namespace PostNamazu.Actions
{
    internal class Mark : NamazuModule
    {
        private IntPtr MarkingFunc;
        private IntPtr LocalMarkingFunc;
        private IntPtr MarkingController;

        public override void GetOffsets() {
            base.GetOffsets();

            //char __fastcall sub_1407A6A60(__int64 g_MarkingController, __int64 MarkType, __int64 ActorID)
            MarkingFunc = base.SigScanner.ScanText("48 89 5C 24 10 48 89 6C 24 18 57 48 83 EC 20 8D 42");//正确
            LocalMarkingFunc = base.SigScanner.ScanText("E8 ?? ?? ?? ?? 4C 8B C5 8B D7 48 8B CB E8");//正确
            MarkingController = SigScanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? 4C 8B 85",3);//正确
        }
        [Command("mark")]
        public void DoMarking(string command)
        {
            CheckBeforeExecution(command);
            var mark = JsonConvert.DeserializeObject<Marking>(command);
            if (mark?.MarkType == null) {
                throw new Exception(GetLocalizedString("Exception"));
            }
            var actor = GetActor(mark.ActorID, mark.Name);
            MarkActor(actor, mark.MarkType.Value, mark.Log, mark.LocalOnly);
        }

        private FFXIV_ACT_Plugin.Common.Models.Combatant GetActor(uint? id, string name)
        {
            var combatants = FFXIV_ACT_Plugin.DataRepository.GetCombatantList().Where(i => !string.IsNullOrEmpty(i.Name) && i.ID != 0xE0000000);
            return combatants.FirstOrDefault(i => i.ID == id) 
                ?? combatants.FirstOrDefault(i => i.Name == name)
                ?? throw new Exception(GetLocalizedString("ActorNotFound", id?.ToString("X8") ?? name ?? "(null)"));
            //PluginUI.Log($"BNpcID={combatant.BNpcNameID},ActorID={combatant.ID:X},markingType={markingType}");
        }

        private void MarkActor(FFXIV_ACT_Plugin.Common.Models.Combatant actor, MarkType markingType, bool shouldLog, bool localOnly = false)
        {
            if (shouldLog)
            {
                PluginUI.Log($"Mark: Actor={actor.Name} (0x{actor.ID:X8}), Type={markingType} ({(int)markingType}), LocalOnly={localOnly}");
            }
            var assemblyLock = Memory.Executor.AssemblyLock;
            var flag = false;
            try
            {
                Monitor.Enter(assemblyLock, ref flag);
                _ = !localOnly
                    ? Memory.CallInjected64<char>(MarkingFunc, MarkingController, markingType, actor.ID) 
                    : Memory.CallInjected64<char>(LocalMarkingFunc, MarkingController, markingType - 1, actor.ID, 0); //本地标点的markingType从0开始，因此需要-1
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }

        protected override Dictionary<string, Dictionary<Language, string>> LocalizedStrings { get; } = new()
        {
            ["ActorNotFound"] = new()
            {
                [Language.EN] = "Could not find actor: {0}",
                [Language.CN] = "未能找到实体： {0}"
            },
            ["Exception"] = new()
            {
                [Language.EN] = "Invalid format for actor marker",
                [Language.CN] = "实体标点格式错误"
            },
        };
    }
}
