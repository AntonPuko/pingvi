using System;
using System.Drawing;


namespace Pingvi.TableCatchers
{
    public interface ITableCatcher {
        
        
        event Action<Bitmap> NewTableBitmap;
        void Start();
        void Stop();
        void MakeScreenShot();

    }
}
