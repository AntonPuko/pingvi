using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;

namespace Pingvi.TableCatchers
{
    class AForgeTableCatcher : ITableCatcher{
  
        private UnmanagedImage _tbUnmanaged;
        private readonly Rectangle _tablePositionRect;
        private ScreenCaptureStream _stream;
        private int _timeFrameInterfal;
        private string _screenShotsPath;



        public event Action<Bitmap> NewTableBitmap;

        public event Action<UnmanagedImage> NewTableImage;


        public AForgeTableCatcher(Rectangle tableRect, int timeInterval, string scrShootsPath) {
            _timeFrameInterfal = timeInterval;
            _tablePositionRect = tableRect;
            _screenShotsPath = scrShootsPath;
        }

        public void Start() {
            CreateStream();
           
        }

        private void CreateStream() {
            _stream = new ScreenCaptureStream(_tablePositionRect);
            
            
            _stream.NewFrame += OnStreamNewFrame;
            _stream.FrameInterval = _timeFrameInterfal;
            _stream.Start();
        }

        public void Stop() {
            _stream.Stop();
        }

        public void MakeScreenShot() {
            if (_tbUnmanaged == null) return;
            try {
                var bmp = _tbUnmanaged.ToManagedImage();
                bmp.Save(_screenShotsPath + DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") +
                                  ".bmp");
                bmp.Dispose();
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message + "in MakeScreenShot();");
            }
        }

        private void OnStreamNewFrame(object sender, NewFrameEventArgs eventArgs) {
            try {
                _tbUnmanaged = UnmanagedImage.FromManagedImage(eventArgs.Frame);

           

                if (NewTableImage != null) {
                    NewTableImage(_tbUnmanaged);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message + " in OnStreamNewFrame()");
            }
            
        
        }
    }
}
