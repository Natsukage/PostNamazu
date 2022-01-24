using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using PostNamazu.Attributes;
using PostNamazu.Models;

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
            MarkingFunc = base.SigScanner.ScanText("48 89 5C 24 10 48 89 6C 24 18 57 48 83 EC 20 8D 42");
            LocalMarkingFunc = base.SigScanner.ScanText("E8 ?? ?? ?? ?? 4C 8B C5 8B D7");
            MarkingController = SigScanner.GetStaticAddressFromSig("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01");
        }
        //反正没人用,不如重构
        [Command("mark")]
        public void DoMarking(string command)
        {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            if (command == "")
                throw new Exception("指令为空");

            var mark=JsonConvert.DeserializeObject<Marking>(command);
            if (mark?.MarkType == null) {
                throw new Exception("标记错误");
            }
            uint actorID = 0xE000000;
            actorID = mark.ActorID ?? GetActorIDByName(mark.Name);
            DoMarkingByActorID(actorID,mark.MarkType.Value,mark.LocalOnly);
        }
        private uint GetActorIDByName(string Name)
        {
            var combatant = FFXIV_ACT_Plugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.Name != null && i.ID != 0xE0000000 && i.Name.Equals(Name));
            if (combatant == null) {
                throw new Exception($"未能找到{Name}");
            }
            return combatant.ID;
            //PluginUI.Log($"BNpcID={combatant.BNpcNameID},ActorID={combatant.ID:X},markingType={markingType}");
        }
        private void DoMarkingByActorID(uint ActorID, MarkType markingType, bool localOnly = false)
        {
            var combatant = FFXIV_ACT_Plugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.ID == ActorID);
            if (combatant == null) {
                throw new Exception($"未能找到{ActorID}");
            }
            PluginUI.Log($"ActorID={ActorID:X},markingType={(int)markingType},LocalOnly={localOnly}");
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
