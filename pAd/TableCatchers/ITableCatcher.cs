using System;
using System.Drawing;
using AForge.Imaging;


namespace Pingvi.TableCatchers
{
    public interface ITableCatcher {

        event Action<UnmanagedImage> NewTableImage;
        void Start();
        void Stop();
        void MakeScreenShot();

    }
}
