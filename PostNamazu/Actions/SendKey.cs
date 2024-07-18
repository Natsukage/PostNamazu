using PostNamazu.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static PostNamazu.Common.I18n;

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

                Log(GetLocalizedString("Success", command));
            }
            catch (Exception ex) {
                throw new Exception(GetLocalizedString("Fail", command, ex));
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
        protected override Dictionary<string, Dictionary<Language, string>> LocalizedStrings { get; } = new()
        {
            ["Success"] = new()
            {
                [Language.EN] = "Key sent: {0}",
                [Language.CN] = "已发送按键：{0}"
            },
            ["Fail"] = new()
            {
                [Language.EN] = "Failed to send key {0}: \n{1}",
                [Language.CN] = "发送按键 {0} 失败：\n{1}"
            },
        };
    }
}
