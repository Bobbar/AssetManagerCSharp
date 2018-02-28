using System;
using System.Runtime.InteropServices;

namespace AssetManager.Tools
{
    /// <summary>
    /// Provides flashing window and taskbar support.
    /// </summary>
    public static class TaskBarNotify
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO fwInfo);

        private enum FlashWindowFlags : UInt32
        {
            FLASHW_STOP = 0,
            FLASHW_CAPTION = 1,
            FLASHW_TRAY = 2,
            FLASHW_ALL = 3,
            FLASHW_TIMER = 4,
            FLASHW_TIMERNOFG = 12
        }

        private struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public FlashWindowFlags dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        public static bool FlashWindow(IntPtr handle, bool flashTitleBar, bool flashTray, int flashCount)
        {
            if (handle == null)
            {
                return false;
            }

            try
            {
                FLASHWINFO fwi = new FLASHWINFO();
                fwi.hwnd = handle;
                if (flashTitleBar)
                {
                    fwi.dwFlags = fwi.dwFlags | FlashWindowFlags.FLASHW_CAPTION;
                }

                if (flashTray)
                {
                    fwi.dwFlags = fwi.dwFlags | FlashWindowFlags.FLASHW_TRAY;
                }

                fwi.uCount = (uint)flashCount;

                if (flashCount == 0)
                {
                    fwi.dwFlags = fwi.dwFlags | FlashWindowFlags.FLASHW_TIMERNOFG;
                }

                fwi.dwTimeout = 0;
                fwi.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(fwi);
                return FlashWindowEx(ref fwi);
            }
            catch
            {
                return false;
            }
        }
    }
}