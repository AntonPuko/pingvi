using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;

namespace Pingvi.TableCatchers
{
    public class VmTableCatcher
    {
        public AutomationElement AeVmDesktop;

        private DispatcherTimer _bitmapTimer;
        public RectangleF TableOnVmDesktopRect;

        public VmTableCatcher()
        {
            TableOnVmDesktopRect = new RectangleF(0, 0, 808, 586);
        }

        public event Action<Bitmap> NewTableBitmap;


        public void Start()
        {
            if (_bitmapTimer == null)
            {
                _bitmapTimer = new DispatcherTimer();
                _bitmapTimer.Interval = TimeSpan.FromMilliseconds(500);
                _bitmapTimer.Tick += MakeTableBitmap;
            }
            _bitmapTimer.Start();
        }

        public void CatchVmScreen()
        {
            var vmwareProcess = Process.GetProcessesByName("VMware");
            if (vmwareProcess.Length == 0) MessageBox.Show("VWware не запущен");
            AeVmDesktop = AutomationElement.FromHandle(vmwareProcess[0].MainWindowHandle);
            //var aeVmMainWindow = AutomationElement.FromHandle(vmwareProcess[0].MainWindowHandle);
            //_aeVmDesktop = aeVmMainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "MKSEmbedded")); 
        }

        private void MakeTableBitmap(object sender, EventArgs e)
        {
            try
            {
                //MAKE REMOTE DESKTOP BITMAP
                if (AeVmDesktop == null || !AeVmDesktop.Current.IsEnabled) return;
                var rect = (Rect) AeVmDesktop.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                //var rect = new Rect(new Point(0, 0), new Size(1000, 1000));
                var bmp = new Bitmap((int) rect.Width, (int) rect.Height);

                var tabHandler = AeVmDesktop.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
                var tabHandlerPtr = new IntPtr((int) tabHandler);
                var memoryGraphics = Graphics.FromImage(bmp);
                var dc = memoryGraphics.GetHdc();
                var success = Winapi.PrintWindow(tabHandlerPtr, dc, 0);
                memoryGraphics.ReleaseHdc(dc);
                //CROP REMOTE DESKTOP TO TABLE SIZE
                //var tableBmp = bmp.Clone(TableOnVmDesktopRect, bmp.PixelFormat);
                //FIRE EVENT
                if (NewTableBitmap != null)
                {
                    NewTableBitmap(bmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " in Method MakeTableBitmap");
            }
        }
    }
}