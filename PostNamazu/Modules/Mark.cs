using PostNamazu.Attributes;
using PostNamazu.Models;
using PostNamazu.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace PostNamazu.Modules
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
            //char __fastcall sub_1407A6A60(__int64 g_MarkingController, __int64 MarkType, __int64 ActorID)
            MarkingController = SigScanner.GetStaticAddressFromSig("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01");
        }

        [Command("mark")]
        public void DoMarking(string command)
        {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            if (command == "")
                throw new Exception("指令为空");

            var dic = ParseQueryString(command);

            PluginUI.Log(command);

            bool localOnly = dic.ContainsKey("Local") && bool.Parse(dic["Local"]);

            if (dic.ContainsKey("MarkType")) {
                var MarkTypeStr = dic["MarkType"];
                if (!Enum.TryParse<MarkingType>(MarkTypeStr, true, out var markingType)) {
                    PluginUI.Log($"未知的标记类型:{MarkTypeStr}");
                    return;
                }
                if (dic.ContainsKey("ActorID")) {
                    var ActorIDStr = dic["ActorID"];
                    var ActorID = UInt32.Parse(ActorIDStr, NumberStyles.HexNumber);
                    DoMarkingByActorID(ActorID, markingType, localOnly);
                }
                else if (dic.ContainsKey("Name")) {
                    var Name = dic["Name"];
                    GetActorIDByName(Name, markingType, localOnly);
                }
                else {
                    PluginUI.Log("错误指令");
                }
            }
            else {
                PluginUI.Log("错误指令");
            };
            return;
        }
        private void GetActorIDByName(string Name, MarkingType markingType, bool localOnly = false)
        {
            var combatant = _ffxivPlugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.Name != null && i.ID != 0xE0000000 && i.Name.Equals(Name));
            if (combatant == null) {
                PluginUI.Log($"未能找到{Name}");
                return;
            }
            //PluginUI.Log($"BNpcID={combatant.BNpcNameID},ActorID={combatant.ID:X},markingType={markingType}");
            DoMarkingByActorID(combatant.ID, markingType, localOnly);
        }
        private void DoMarkingByActorID(uint ActorID, MarkingType markingType, bool localOnly = false)
        {
            var combatant = _ffxivPlugin.DataRepository.GetCombatantList().FirstOrDefault(i => i.ID == ActorID);
            if (combatant == null) {
                PluginUI.Log($"未能找到{ActorID}");
                return;
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

        private static Dictionary<string, string> ParseQueryString(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) {
                throw new ArgumentNullException("字符串为空");
            }
            if (string.IsNullOrWhiteSpace(url)) {
                return new Dictionary<string, string>();
            }
            var dic = url
                    //2.通过&划分各个参数
                    .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                    //3.通过=划分参数key和value,且保证只分割第一个=字符
                    .Select(param => param.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                    //4.通过相同的参数key进行分组
                    .GroupBy(part => part[0], part => part.Length > 1 ? part[1] : string.Empty)
                    //5.将相同key的value以,拼接
                    .ToDictionary(group => group.Key, group => string.Join(",", group));

            return dic;
        }
    }
}
