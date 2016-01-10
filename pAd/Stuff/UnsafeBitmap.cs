using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pingvi.Stuff
{
    public unsafe class UnsafeBitmap : IDisposable
    {
        private BitmapData _bitmapData;
        private byte* _pBase = null;

        // three elements used for MakeGreyUnsafe
        private int _width;

        public UnsafeBitmap(Bitmap bitmap) {
            Bitmap = new Bitmap(bitmap);
        }

        public UnsafeBitmap(int width, int height, PixelFormat pixelFormat) {
            Bitmap = new Bitmap(width, height, pixelFormat);
        }

        public Bitmap Bitmap { get; }

        private Point PixelSize
        {
            get
            {
                var unit = GraphicsUnit.Pixel;
                var bounds = Bitmap.GetBounds(ref unit);

                return new Point((int) bounds.Width, (int) bounds.Height);
            }
        }

        public void Dispose() {
            Bitmap.Dispose();
        }

        public void LockBitmap() {
            var unit = GraphicsUnit.Pixel;
            var boundsF = Bitmap.GetBounds(ref unit);
            var bounds = new Rectangle((int) boundsF.X,
                (int) boundsF.Y,
                (int) boundsF.Width,
                (int) boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            _width = (int) boundsF.Width*sizeof (PixelData);
            if (_width%4 != 0)
            {
                _width = 4*(_width/4 + 1);
            }
            _bitmapData =
                Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            _pBase = (byte*) _bitmapData.Scan0.ToPointer();
        }

        public PixelData GetPixel(int x, int y) {
            var returnValue = *PixelAt(x, y);
            return returnValue;
        }

        public void SetPixel(int x, int y, PixelData colour) {
            var pixel = PixelAt(x, y);
            *pixel = colour;
        }

        public void UnlockBitmap() {
            Bitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }

        public PixelData* PixelAt(int x, int y) {
            return (PixelData*) (_pBase + y*_width + x*sizeof (PixelData));
        }
    }

    public struct PixelData
    {
        public byte Blue;
        public byte Green;
        public byte Red;
    }
}