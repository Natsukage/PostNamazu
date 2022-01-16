using System;
using System.Collections.Generic;
using System.Text;
using PostNamazu.Attributes;

namespace PostNamazu.Modules
{
    internal class SendKey : NamazuModule
    {
        [Command("sendkey")]
        public void DoSendKey(string command)
        {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            if (command == "")
                throw new Exception("指令为空");

            Log($"收到按键：{command}");
            try {
                var keycode = int.Parse(command);
                SendKeycode(keycode);
            }
            catch (Exception ex) {
                Log($"发送按键失败：{ex}");
            }
        }

        private void SendKeycode(int keycode)
        {
            SendMessageToWindow(WinAPI.WM_KEYDOWN, keycode, 0);
            SendMessageToWindow(WinAPI.WM_KEYUP, keycode, 0);
        }

        private void SendMessageToWindow(uint code, int wparam, int lparam)
        {
            IntPtr hwnd = FFXIV.MainWindowHandle;
            if (hwnd != IntPtr.Zero) {
                IntPtr res = WinAPI.SendMessage(hwnd, code, (IntPtr)wparam, (IntPtr)lparam);
            }
        }
    }
}
