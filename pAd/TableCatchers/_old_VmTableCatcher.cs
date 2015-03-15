using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Pingvi.TableCatchers {
    public class VmTableCatcher {

        public event Action<Bitmap> NewTableBitmap;
        public RectangleF TableOnVmDesktopRect;
        public AutomationElement _aeVmDesktop;

        private DispatcherTimer _bitmapTimer;

        public VmTableCatcher() {
            TableOnVmDesktopRect = new RectangleF(0,0, 808,586);
        }


        public void Start() {
            if (_bitmapTimer == null) {
                _bitmapTimer = new DispatcherTimer();
                _bitmapTimer.Interval = TimeSpan.FromMilliseconds(500);
                _bitmapTimer.Tick += MakeTableBitmap;
            }
            _bitmapTimer.Start();
        }

        public void CatchVmScreen() {
            var vmwareProcess = Process.GetProcessesByName("VMware");
            if (vmwareProcess.Length == 0) MessageBox.Show("VWware не запущен");
            _aeVmDesktop = AutomationElement.FromHandle(vmwareProcess[0].MainWindowHandle);
            //var aeVmMainWindow = AutomationElement.FromHandle(vmwareProcess[0].MainWindowHandle);
            //_aeVmDesktop = aeVmMainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "MKSEmbedded")); 

           
            
        }

        private void MakeTableBitmap(object sender, EventArgs e) {
           
            try
            {
                //MAKE REMOTE DESKTOP BITMAP
                if (_aeVmDesktop == null || !_aeVmDesktop.Current.IsEnabled) return;
                var rect = (Rect)_aeVmDesktop.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                //var rect = new Rect(new Point(0, 0), new Size(1000, 1000));
                Bitmap bmp = new Bitmap((int)rect.Width, (int)rect.Height);

                var tabHandler = _aeVmDesktop.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
                IntPtr tabHandlerPtr = new IntPtr((int)tabHandler);
                Graphics memoryGraphics = Graphics.FromImage(bmp);
                IntPtr dc = memoryGraphics.GetHdc();
                bool success = WINAPI.PrintWindow(tabHandlerPtr, dc, 0);
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
