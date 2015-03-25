using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Pingvi
{
    //TODO Refact сделать проверку для какого монитора снимать скриншот
    public class ScreenTableManager {

        public Rectangle TablePositionRect;
        public TimeSpan BitmapMakeInterval;
        public event Action<Bitmap> NewBitmap;

        private DispatcherTimer _bitmapTimer;

        public ScreenTableManager(Rectangle tableRect, TimeSpan timeInterval)
        {
            TablePositionRect = tableRect;
            BitmapMakeInterval = timeInterval;
        }
        
        public ScreenTableManager() {
            TablePositionRect = new Rectangle(550, 20, 808, 581);
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
            try {
                Bitmap bmp = new Bitmap(TablePositionRect.Width, TablePositionRect.Height);
                using (Graphics gr = Graphics.FromImage(bmp)) {
                    gr.CopyFromScreen(Screen.AllScreens[0].Bounds.Width + TablePositionRect.X, TablePositionRect.Y,
                        0, 0, TablePositionRect.Size, CopyPixelOperation.SourceCopy);
                }

                if (NewBitmap != null) {
                    NewBitmap(bmp);
                }

            } catch (Exception ex ){
                Debug.WriteLine(ex.Message + "in ScreenTableManager.MakeTableBitmap()");
           }
       }

         
    }

}

