using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using PostNamazu.Attributes;
using PostNamazu.Models;
using PostNamazu.Common;

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
        //反正没人用,不如重构
        [Command("mark")]
        public void DoMarking(string command)
        {
            CheckBeforeExecution(command);

            var mark=JsonConvert.DeserializeObject<Marking>(command);
            if (mark?.MarkType == null) {
                throw new Exception(I18n.Translate("Mark/Exception", "实体标点格式错误。"));
            }
            uint actorID = 0xE000000;
            actorID = mark.ActorID ?? GetActorIDByName(mark.Name);
            DoMarkingByActorID(actorID,mark.MarkType.Value,mark.LocalOnly);
        }
        private uint GetActorIDByName(string Name)
        {
            var combatant = FFXIV_ACT_Plugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.Name != null && i.ID != 0xE0000000 && i.Name.Equals(Name));
            if (combatant == null) {
                throw new Exception(I18n.Translate("Mark/ActorNotFound", "未能找到实体： {0}", Name));
            }
            return combatant.ID;
            //PluginUI.Log($"BNpcID={combatant.BNpcNameID},ActorID={combatant.ID:X},markingType={markingType}");
        }
        private void DoMarkingByActorID(uint ActorID, MarkType markingType, bool localOnly = false)
        {
            var combatant = FFXIV_ACT_Plugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.ID == ActorID);
            if (ActorID != 0xE0000000 && combatant == null) {
                throw new Exception(I18n.Translate("Mark/ActorNotFound", "未能找到实体： {0}", ActorID));
            }
            PluginUI.Log($"ActorID={ActorID:X}, markingType={(int)markingType}, LocalOnly={localOnly}");
            var assemblyLock = Memory.Executor.AssemblyLock;
            var flag = false;
            try {
                Monitor.Enter(assemblyLock, ref flag);
                if (!localOnly)
                    _ = Memory.CallInjected64<char>(MarkingFunc, MarkingController, markingType, ActorID);
                else //本地标点的markingType从0开始，因此需要-1
                    _ = Memory.CallInjected64<char>(LocalMarkingFunc, MarkingController, markingType - 1, ActorID, 0);
            }
            finally {
                if (flag) Monitor.Exit(assemblyLock);
            }
        }
    }
}
