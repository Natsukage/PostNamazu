using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PostNamazu
{
    class WinAPI
    {
        public const uint WM_KEYUP = 0x101;
        public const uint WM_KEYDOWN = 0x100;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
