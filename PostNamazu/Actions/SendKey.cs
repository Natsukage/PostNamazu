using System;
using System.Runtime.InteropServices;
using PostNamazu.Attributes;
using PostNamazu.Common;

namespace PostNamazu.Actions
{
    internal class SendKey : NamazuModule
    {
        [Command("sendkey")]
        public void DoSendKey(string command)
        {
            CheckBeforeExecution(command);

            try {
                var keycode = int.Parse(command);
                SendKeycode(keycode);
                Log(I18n.Translate("SendKey/Success", "已发送按键：{0}", command));
            }
            catch (Exception ex) {
                throw new Exception(I18n.Translate("SendKey/Fail", "发送按键 {0} 失败：\n{1}", command, ex));
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
