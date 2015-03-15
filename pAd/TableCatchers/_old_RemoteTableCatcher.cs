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

namespace Pingvi
{
    class RemoteTableCatcher {
        private event Action NewRemoteDesktop;
        private AutomationElement _aeRemoteDesktop;




        public RectangleF TableOnRemoteDesktopRect;
        public event Action<Bitmap> NewTableBitmap;
        private DispatcherTimer _bitmapMakerTimer;
        
        


        
        public RemoteTableCatcher() {
            TableOnRemoteDesktopRect = new RectangleF(144,35,808,586);
            
        }

        public void Start() {
            NewRemoteDesktop += StartMakingBitmaps;
            CatchRemoteTable();
        }
        public void CatchRemoteTable() {
            var rutView =  Process.GetProcessesByName("rutview");
            if (rutView.Length == 0) {
                MessageBox.Show("RutView не запущен!");
                return;
            }
            var aeRutView = AutomationElement.FromHandle(rutView[0].MainWindowHandle);
            var aeDesktop = AutomationElement.RootElement;

            _aeRemoteDesktop = aeDesktop.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "TfmRender"));

            
   
            if (_aeRemoteDesktop == null) {
                MessageBox.Show("удаленный desktop не найден");
                return;
            }

            if (NewRemoteDesktop != null && _aeRemoteDesktop != null) {
                NewRemoteDesktop();
            }

        }


        public void  StartMakingBitmaps() {
            if (_bitmapMakerTimer == null) {
                _bitmapMakerTimer = new DispatcherTimer();
                _bitmapMakerTimer.Interval = TimeSpan.FromMilliseconds(500);
                _bitmapMakerTimer.Tick += MakeTableBitmap;
            }
            _bitmapMakerTimer.Start();
        }






        private void MakeTableBitmap(object sender, EventArgs e) {
            try {
                //MAKE REMOTE DESKTOP BITMAP
                if (_aeRemoteDesktop == null || !_aeRemoteDesktop.Current.IsEnabled) return;
                //var rect = (Rect)_aeRemoteDesktop.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                var rect = new Rect(new Point(0,0), new Size(1000,1000));
                Bitmap bmp = new Bitmap((int)rect.Width, (int)rect.Height);
                
                var tabHandler = _aeRemoteDesktop.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
                IntPtr tabHandlerPtr = new IntPtr((int)tabHandler);
                Graphics memoryGraphics = Graphics.FromImage(bmp);
                IntPtr dc = memoryGraphics.GetHdc();
                bool success = WINAPI.PrintWindow(tabHandlerPtr, dc, 0);
                memoryGraphics.ReleaseHdc(dc);
                //CROP REMOTE DESKTOP TO TABLE SIZE
                var tableBmp = bmp.Clone(TableOnRemoteDesktopRect, bmp.PixelFormat);
                //FIRE EVENT
                if (NewTableBitmap != null) {
                    NewTableBitmap(tableBmp);
                }
            } catch (Exception ex) {
                    MessageBox.Show(ex.Message + " in Method MakeTableBitmap");
                } 
        }
    }
}
