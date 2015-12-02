using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using Image = System.Drawing.Image;

namespace Pingvi
{
    public static class BitmapHelper
    {
        public static bool BitmapsEqualsUnmanaged(UnmanagedImage image1, UnmanagedImage image2)
        {
            if (image1.Height*image1.Width != image2.Height*image2.Width) return false;
            for (var x = 0; x < image1.Width; x++)
            {
                for (var y = 0; y < image1.Height; y++)
                {
                    if (image1.GetPixel(x, y) != image2.GetPixel(x, y)) return false;
                }
            }
            return true;
        }


        /// <summary>
        ///     Make source for Image WPF Control
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static BitmapImage MakeImage(Bitmap bmp)
        {
            if (bmp != null)
            {
                using (var memory = new MemoryStream())
                {
                    bmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            return null;
        }


        /// <summary>
        ///     Check if bitmaps are equals LockBits method
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static unsafe bool BitmapsEquals(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
                return false;
            var bmpd1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var bmpd2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var res = true;
            var p1 = (int*) bmpd1.Scan0;
            var p2 = (int*) bmpd2.Scan0;
            for (var i = 0; i < bmpd1.Height; i++)
            {
                for (var j = 0; j < bmpd2.Width; j++)
                {
                    if (*p1 != *p2)
                    {
                        res = false;
                        break;
                    }
                    p1++;
                    p2++;
                }
            }
            bmp1.UnlockBits(bmpd1);
            bmp2.UnlockBits(bmpd2);
            return res;
        }

        /// <summary>
        ///     Check if bitmaps are equals pixel by pixel
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static bool BitmapsEqualsByPixels(Bitmap bmp1, Bitmap bmp2)
        {
            {
                for (var i = 0; i < bmp1.Width; ++i)
                {
                    for (var j = 0; j < bmp1.Height; ++j)
                    {
                        if (bmp1.GetPixel(i, j) != bmp2.GetPixel(i, j))
                            return false;
                    }
                }
                return true;
            }
        }


        public static double[] ProcessUnsafeBitmapIntoDoubleArray(Bitmap procBitmap)
        {
            var res = new double[procBitmap.Size.Width*procBitmap.Size.Height];

            unsafe
            {
                var bitmapData = procBitmap.LockBits(new Rectangle(0, 0, procBitmap.Width, procBitmap.Height),
                    ImageLockMode.ReadWrite, procBitmap.PixelFormat);
                var bytesPerPixel = Image.GetPixelFormatSize(procBitmap.PixelFormat)/8;
                var heightInPixels = bitmapData.Height;
                var widthInBytes = bitmapData.Width*bytesPerPixel;
                var ptrFirstPixel = (byte*) bitmapData.Scan0;

                var c = 0;
                for (var y = 0; y < heightInPixels; y++)
                {
                    var currentLine = ptrFirstPixel + (y*bitmapData.Stride);
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int blue = currentLine[x];
                        int green = currentLine[x + 1];
                        int red = currentLine[x + 2];

                        if (blue == 255 && green == 255 && red == 255) res[c] = 0.5;
                        else res[c] = -0.5;
                        c++;
                    }
                }

                procBitmap.Dispose();
            }
            return res;
        }

        public static double[] ProcessUnsafeBitmapIntoDoubleParallel(Bitmap procBitmap)
        {
            var res = new double[procBitmap.Size.Width*procBitmap.Size.Height];

            unsafe
            {
                var bitmapData = procBitmap.LockBits(new Rectangle(0, 0, procBitmap.Width, procBitmap.Height),
                    ImageLockMode.ReadWrite, procBitmap.PixelFormat);
                var bytesPerPixel = Image.GetPixelFormatSize(procBitmap.PixelFormat)/8;
                var heightInPixels = bitmapData.Height;
                var widthInBytes = bitmapData.Width*bytesPerPixel;
                var ptrFirstPixel = (byte*) bitmapData.Scan0;

                var c = 0;
                Parallel.For(0, heightInPixels, y =>
                {
                    var currentLine = ptrFirstPixel + (y*bitmapData.Stride);
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int blue = currentLine[x];
                        int green = currentLine[x + 1];
                        int red = currentLine[x + 2];

                        if (blue == 255 && green == 255 && red == 255) res[c] = 0.5;
                        else res[c] = -0.5;
                        c++;
                    }
                });

                procBitmap.Dispose();
            }
            return res;
        }
    }
}