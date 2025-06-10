using PostNamazu.Attributes;
using System;
using System.Runtime.InteropServices;
using PostNamazu.Common.Localization;
#pragma warning disable CS0649 // 从未对字段赋值，字段将一直保持其默认值

namespace PostNamazu.Actions
{
    internal class SendKey : NamazuModule
    {
        // 本地化字符串定义
        [LocalizationProvider("SendKey")]
        private static class Localizations
        {
            [Localized("Key sent: {0}", "已发送按键：{0}")]
            public static readonly string Success;

            [Localized("Failed to send key {0}: \n{1}", "发送按键 {0} 失败：\n{1}")]
            public static readonly string Fail;
        }

        [Command("sendkey")]
        public void DoSendKey(string command)
        {
            CheckBeforeExecution(command);

            try {
                var keycode = int.Parse(command);
                SendKeycode(keycode);

                Log(L.Get("SendKey/Success", command));
            }
            catch (Exception ex) {
                throw new Exception(L.Get("SendKey/Fail", command, ex));
            }
        }

        private void SendKeycode(int keycode)
        {
            SendMessageToWindow(WM_KEYDOWN, keycode, 0);
            SendMessageToWindow(WM_KEYUP, keycode, 0);
        }

        private void SendMessageToWindow(uint code, int wparam, int lparam)
        {
            var hwnd = FFXIV.MainWindowHandle;
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
