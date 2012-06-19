using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CTEngine
{
    public class Sprite
    {
        private Image _image;
        public float _angle;
        private Size _size;
        private Point _position;

        public Sprite() 
        { 
        
        }

        public Graphics getSprite(Graphics img) 
        {
            _image = Image.FromFile(@"C:\Users\Bongo\Desktop\Berufsschule\AE\CrazyTaxi-0.3\Resources\Taxi-GTA2.png");
            Graphics g = img;
            System.Drawing.Drawing2D.Matrix m = g.Transform;
            //here we do not need to translate, we rotate at the specified point
            float x = (float)_size.Width + (float)_position.X;
            float y = (float)_size.Height + (float)_position.Y;
            m.RotateAt(_angle, new PointF(_size.Width, _size.Height), System.Drawing.Drawing2D.MatrixOrder.Append);
            g.Transform = m;
           
            return g;
        }

    }
}
