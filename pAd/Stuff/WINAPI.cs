using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Pingvi
{
    public static class Winapi
    {
        public static int WmHotkey = 0x312;


        private static int _keyId;


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDc, uint nFlags);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void RegisterHotKey(Window window, uint keyVlc) {
            var hwnd = new WindowInteropHelper(window).Handle;
            _keyId = window.GetHashCode();
            RegisterHotKey(hwnd, _keyId, 0, (int) keyVlc);
        }

        public static void UnregisterHotKey(Window window) {
            try
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                UnregisterHotKey(hwnd, _keyId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}