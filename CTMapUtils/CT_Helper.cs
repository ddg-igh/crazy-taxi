using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CTMapUtils
{
    public class CT_Helper
    {
        public static Image resizeImage(Image imgToResize, Size size)
        {
            Bitmap b = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage((Image)b);

            g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
            g.Dispose();

            return (Image)b;
        }
    }
}
