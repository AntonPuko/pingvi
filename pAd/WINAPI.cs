using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Pingvi
{
    public static class WINAPI {
  
        public static int WM_HOTKEY = 0x312;


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private static int keyId;
        public static void RegisterHotKey(Window window, uint keyVlc)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            keyId = window.GetHashCode();
            RegisterHotKey(hwnd, keyId, 0,(int) keyVlc);
        }

        public static void UnregisterHotKey(Window window)
        {
            try
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                UnregisterHotKey(hwnd, keyId); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
