using System;
using System.Runtime.InteropServices;
using PostNamazu.Attributes;

namespace PostNamazu.Actions
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
                throw new Exception($"发送按键失败：{ex}");
            }
        }

        private void SendKeycode(int keycode)
        {
            SendMessageToWindow(WM_KEYDOWN, keycode, 0);
            SendMessageToWindow(WM_KEYUP, keycode, 0);
        }

        private void SendMessageToWindow(uint code, int wparam, int lparam)
        {
            IntPtr hwnd = FFXIV.MainWindowHandle;
            if (hwnd != IntPtr.Zero)
                SendMessage(hwnd, code, (IntPtr)wparam, (IntPtr)lparam);

        }

        #region WinAPI
        private const uint WM_KEYUP = 0x101;
        private const uint WM_KEYDOWN = 0x100;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion

    }
}
