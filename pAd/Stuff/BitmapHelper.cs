using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Pingvi
{
    public  static class BitmapHelper
    {
        public static void GuiAsync(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            if (action == null)
                throw new ArgumentNullException("action");

            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(action, priority);
        }


        /// <summary>
        /// Make source for Image WPF Control
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static BitmapImage MakeImage(Bitmap bmp)
        {
            if (bmp != null) {
                using (MemoryStream memory = new MemoryStream())
                {
                    bmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
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
        /// Check if bitmaps are equals LockBits method
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        unsafe public static Boolean BitmapsEquals(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
                return false;
            BitmapData bmpd1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmpd2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Boolean res = true;
            Int32* p1 = (Int32*)bmpd1.Scan0;
            Int32* p2 = (Int32*)bmpd2.Scan0;
            for (Int32 i = 0; i < bmpd1.Height; i++)
            {
                for (Int32 j = 0; j < bmpd2.Width; j++)
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
        /// Check if bitmaps are equals pixel by pixel
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static bool BitmapsEqualsByPixels(Bitmap bmp1, Bitmap bmp2) {
            {
                for (int i = 0; i < bmp1.Width; ++i)
                {
                    for (int j = 0; j < bmp1.Height; ++j)
                    {
                        if (bmp1.GetPixel(i, j) != bmp2.GetPixel(i, j))
                            return false;
                    }
                }
                return true;
            }
            
        }
     
    }
}
