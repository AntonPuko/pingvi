using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using AForge.Imaging;

namespace Pingvi.TableCatchers {
    public class ScreenShotTableCatcher : ITableCatcher {

        private Bitmap _tableBitmap;
        private DispatcherTimer _bitmapTimer;
        private readonly Rectangle _tablePositionRect;
        private readonly TimeSpan _bitmapMakeInterval;
        private readonly string _screenShotsPath;
        
        public event Action<Bitmap> NewTableBitmap;
        public event Action<UnmanagedImage> NewTableImage;


        public ScreenShotTableCatcher(Rectangle tableRect, TimeSpan timeInterval, string scrShootsPath) {
            _tablePositionRect = tableRect;
            _bitmapMakeInterval = timeInterval;
            _screenShotsPath = scrShootsPath;
        }


        public void Start() {
            if (_bitmapTimer == null)
            {
                _bitmapTimer = new DispatcherTimer();
                _bitmapTimer.Tick += MakeTableBitmap;
            }
            _bitmapTimer.Interval = _bitmapMakeInterval;
            _bitmapTimer.Start();
        }

        public void Stop() {
            if (_bitmapTimer == null) return;
            _bitmapTimer.Stop();
        }

        public void MakeScreenShot() {
            if (_tableBitmap == null) return;
            try {
                _tableBitmap.Save(_screenShotsPath + DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") +
                                  ".bmp");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message + "in MakeScreenShot();");
            }
        }

        private void MakeTableBitmap(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = new Bitmap(_tablePositionRect.Width, _tablePositionRect.Height);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(Screen.AllScreens[0].Bounds.Width + _tablePositionRect.X, _tablePositionRect.Y,
                        0, 0, _tablePositionRect.Size, CopyPixelOperation.SourceCopy);
                }

                _tableBitmap = bmp;
                var _uTimage = UnmanagedImage.FromManagedImage(bmp);
                if (NewTableBitmap != null)
                {
                    NewTableBitmap(bmp);
                }

                if (NewTableImage != null) {
                    NewTableImage(_uTimage);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "in ScreenShotTableCatcher.MakeTableBitmap()");
            }
        }
    }
}