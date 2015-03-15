using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Pingvi
{
    //TODO Refact сделать проверку для какого монитора снимать скриншот
    public class ScreenTableManager {

        public RectangleF TablePositionRect;
        public TimeSpan BitmapMakeInterval;
        public event Action<Bitmap> NewBitmap;

        private DispatcherTimer _bitmapTimer;

        public ScreenTableManager(RectangleF tableRectF, TimeSpan timeInterval) {
            TablePositionRect = tableRectF;
            BitmapMakeInterval = timeInterval;
        }
        
        public ScreenTableManager() {
            TablePositionRect = new RectangleF(550,20,808,581);
            BitmapMakeInterval = TimeSpan.FromMilliseconds(150);
        }

        public void Start() {
            if (_bitmapTimer == null) {
                _bitmapTimer = new DispatcherTimer();
                _bitmapTimer.Tick += MakeTableBitmap;
            }
            _bitmapTimer.Interval = BitmapMakeInterval;
            _bitmapTimer.Start();
        }

        public void Stop() {
            if (_bitmapTimer == null) return;
            _bitmapTimer.Stop();
        }

        private void MakeTableBitmap(object sender, EventArgs e) {
          //  try {
                //capture second monitor screen
                Bitmap bmp = new Bitmap(Screen.AllScreens[1].Bounds.Width, Screen.AllScreens[1].Bounds.Height);
                using (Graphics gr = Graphics.FromImage(bmp)) {
                    gr.CopyFromScreen(Screen.AllScreens[1].Bounds.X, Screen.AllScreens[1].Bounds.Y,
                        0, 0, Screen.AllScreens[1].Bounds.Size, CopyPixelOperation.SourceCopy);
                }

                //Crop ScreenShot to table size and position
                bmp = bmp.Clone(TablePositionRect, bmp.PixelFormat);

                if (NewBitmap != null) {
                    NewBitmap(bmp);
                }

         //   } catch (Exception ex ){
         //       Debug.WriteLine(ex.Message + "in ScreenTableManager.MakeTableBitmap()");
         //  }
       }

    }

}

