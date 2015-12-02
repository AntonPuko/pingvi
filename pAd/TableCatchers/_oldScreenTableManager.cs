using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using AForge.Video;

namespace Pingvi
{
    //TODO Refact сделать проверку для какого монитора снимать скриншот
    public class ScreenTableManager
    {
        private DispatcherTimer _bitmapTimer;
        private Bitmap _tableBitmap;
        public TimeSpan BitmapMakeInterval;

        public Rectangle TablePositionRect;

        public ScreenTableManager(Rectangle tableRect, TimeSpan timeInterval)
        {
            TablePositionRect = tableRect;
            BitmapMakeInterval = timeInterval;
        }

        public ScreenTableManager()
        {
            //TablePositionRect = new Rectangle(230, 20, 800, 574);
            TablePositionRect = new Rectangle(230 + 1920, 20, 800, 574);
            BitmapMakeInterval = TimeSpan.FromMilliseconds(100);
        }

        public event Action<Bitmap> NewTableBitmap;

        public void Start()
        {
            // CreateStream();

            if (_bitmapTimer == null)
            {
                _bitmapTimer = new DispatcherTimer();
                _bitmapTimer.Tick += MakeTableBitmap;
            }
            _bitmapTimer.Interval = BitmapMakeInterval;
            _bitmapTimer.Start();
        }

        public void Stop()
        {
            if (_bitmapTimer == null) return;
            _bitmapTimer.Stop();
        }

        public void MakeScreenShot()
        {
            if (_tableBitmap == null) return;
            const string path = @"P:\screens\";

            _tableBitmap.Save(path + DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + ".bmp");
        }

        private void MakeTableBitmap(object sender, EventArgs e)
        {
            try
            {
                var bmp = new Bitmap(TablePositionRect.Width, TablePositionRect.Height);
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(Screen.AllScreens[0].Bounds.Width + TablePositionRect.X, TablePositionRect.Y,
                        0, 0, TablePositionRect.Size, CopyPixelOperation.SourceCopy);
                }

                _tableBitmap = bmp;
                if (NewTableBitmap != null)
                {
                    NewTableBitmap(bmp);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "in ScreenTableManager.MakeTableBitmap()");
            }
        }

        public void CreateStream()
        {
            var stream = new ScreenCaptureStream(TablePositionRect);
            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }

        private void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            _tableBitmap = eventArgs.Frame;

            // _tableBitmap = Grayscale.CommonAlgorithms.BT709.Apply(_tableBitmap);
            //   Threshold filter = new Threshold();
            //     _tableBitmap = filter.Apply(_tableBitmap);

            //       MakeScreenShot();
            if (NewTableBitmap != null)
            {
                NewTableBitmap(_tableBitmap);
            }
        }
    }
}