using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using AForge.Imaging;

namespace Pingvi.TableCatchers {
    public class ScreenShotTableCatcher : ITableCatcher {

        private UnmanagedImage _tbUnmanaged;
        private DispatcherTimer _bitmapTimer;
        private readonly Rectangle _tablePositionRect;
        private readonly TimeSpan _bitmapMakeInterval;
        private readonly string _screenShotsPath;

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
            if (_tbUnmanaged == null) return;
            try {
                var bmp = _tbUnmanaged.ToManagedImage();
                bmp.Save(_screenShotsPath + DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") +
                                  ".bmp");
                bmp.Dispose();
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

                _tbUnmanaged = UnmanagedImage.FromManagedImage(bmp);
                if (NewTableImage != null) {
                    NewTableImage(_tbUnmanaged);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "in ScreenShotTableCatcher.MakeTableBitmap()");
            }
        }
    }
}