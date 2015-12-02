using System;
using System.Diagnostics;
using System.Drawing;
using AForge.Imaging;
using AForge.Video;

namespace Pingvi.TableCatchers
{
    internal class AForgeTableCatcher : ITableCatcher
    {
        private readonly Rectangle _tablePositionRect;
        private readonly string _screenShotsPath;
        private ScreenCaptureStream _stream;

        private UnmanagedImage _tbUnmanaged;
        private readonly int _timeFrameInterfal;


        public AForgeTableCatcher(Rectangle tableRect, int timeInterval, string scrShootsPath)
        {
            _timeFrameInterfal = timeInterval;
            _tablePositionRect = tableRect;
            _screenShotsPath = scrShootsPath;
        }

        public event Action<UnmanagedImage> NewTableImage;

        public void Start()
        {
            CreateStream();
        }

        public void Stop()
        {
            _stream.Stop();
        }

        public void MakeScreenShot()
        {
            if (_tbUnmanaged == null) return;
            try
            {
                var bmp = _tbUnmanaged.ToManagedImage();
                bmp.Save(_screenShotsPath + DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") +
                         ".bmp");
                bmp.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "in MakeScreenShot();");
            }
        }


        public event Action<Bitmap> NewTableBitmap;

        private void CreateStream()
        {
            _stream = new ScreenCaptureStream(_tablePositionRect);


            _stream.NewFrame += OnStreamNewFrame;
            _stream.FrameInterval = _timeFrameInterfal;
            _stream.Start();
        }

        private void OnStreamNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                _tbUnmanaged = UnmanagedImage.FromManagedImage(eventArgs.Frame);


                if (NewTableImage != null)
                {
                    NewTableImage(_tbUnmanaged);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " in OnStreamNewFrame()");
            }
        }
    }
}