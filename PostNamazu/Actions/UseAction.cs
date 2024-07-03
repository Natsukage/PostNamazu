using System;
using Newtonsoft.Json;
using PostNamazu.Attributes;

namespace PostNamazu.Actions
{
    internal class UseAction : NamazuModule
    {
        private IntPtr ActionManagerPtr;
        private IntPtr ActionManagerPtrPtr;
        private IntPtr UseActionLocPtr;
        private IntPtr UseActionPtr;

        public override void GetOffsets()
        {
            base.GetOffsets();
            ActionManagerPtr =
                SigScanner.ScanText("48 8D 0D ?? ?? ?? ?? F3 0F 10 13");
            ActionManagerPtrPtr = ActionManagerPtr + 7 + Memory.Read<int>(ActionManagerPtr + 3);
            UseActionLocPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 41 3A C5 0F 85 ?? ?? ?? ??");
            UseActionPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? B0 01 EB B6");
        }

        [Command("UseAction")]
        public void DoUseAction(string command)
        {
            if (!isReady)
                throw new Exception("没有对应的游戏进程");

            if (command == "")
                throw new Exception("指令为空");

            PluginUI.Log(command);

            var action = JsonConvert.DeserializeObject<Models.UseAction.Action>(command);

            if (action.ActionType == 2)
            {
                Memory.ClearCallCache();
                Memory.CallInjected64<char>(UseActionPtr, (long)ActionManagerPtrPtr, 2, action.ActionID, 3758096384,
                    0xFFFF, 0, 0, 0);
                Memory.ClearCallCache();
            }
            else
            {
                if (action.IsGroundTarget)
                {
                    Memory.ClearCallCache();
                    action.X ??= 0;
                    action.Y ??= 0;
                    action.Z ??= 0;
                    var vec = new Vec3 { X = action.X.Value, Y = action.Y.Value, Z = action.Z.Value };
                    var posptr = Memory.AllocateMemory(0x10);
                    Memory.Write(posptr, vec);
                    Memory.CallInjected64<char>(UseActionLocPtr, (long)ActionManagerPtrPtr, action.ActionType,
                        action.ActionID, 3758096384,
                        (long)posptr, 0);
                    Memory.FreeMemory(posptr);
                    Memory.ClearCallCache();
                }
                else
                {
                    Memory.ClearCallCache();
                    Memory.CallInjected64<char>(UseActionPtr, (long)ActionManagerPtrPtr, action.ActionType,
                        action.ActionID, action.TargetID, 0, 0, 0, 0);
                    Memory.ClearCallCache();
                }
            }
        }

        public struct Vec3
        {
            public float X;
            public float Y;
            public float Z;
        }
    }
}