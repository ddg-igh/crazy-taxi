using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CrazyTaxi
{
    static class BitmapHelper
    {

        public static void Copy(Bitmap from, Bitmap to,Rectangle fromRect)
            {
                //if (from.Size != to.Size) throw new FormatException("Pictures are not Equal in Size");
                if (from.PixelFormat != PixelFormat.Format32bppPArgb) throw new FormatException("Source Picture has wrong PixelFormat");
                if (to.PixelFormat != PixelFormat.Format32bppPArgb) throw new FormatException("Target Picture has wrong PixelFormat");

                Rectangle lockRect = new Rectangle(0, 0, to.Width, to.Height);
                BitmapData toData = to.LockBits(lockRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
                BitmapData fromData = from.LockBits(fromRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);

                byte[] data = new byte[8 * 1024];

                int i = 0;
                for (; i < toData.Stride * toData.Height / data.Length; i++)
                {
                    IntPtr fromPtr = (IntPtr)(fromData.Scan0.ToInt32() + i * data.Length);
                    IntPtr toPtr = (IntPtr)(toData.Scan0.ToInt32() + i * data.Length);
                    System.Runtime.InteropServices.Marshal.Copy(fromPtr, data, 0, data.Length);
                    System.Runtime.InteropServices.Marshal.Copy(data, 0, toPtr, data.Length);
                }
                if ((toData.Stride * toData.Height) % data.Length != 0)
                {
                    int Rest = (toData.Stride * toData.Height) % data.Length;
                    IntPtr fromPtr = (IntPtr)(fromData.Scan0.ToInt32() + i * data.Length);
                    IntPtr toPtr = (IntPtr)(toData.Scan0.ToInt32() + i * data.Length);
                    System.Runtime.InteropServices.Marshal.Copy(fromPtr, data, 0, Rest);
                    System.Runtime.InteropServices.Marshal.Copy(data, 0, toPtr, Rest);
                }

                to.UnlockBits(toData);
                from.UnlockBits(fromData);            
            }

        }
    }
