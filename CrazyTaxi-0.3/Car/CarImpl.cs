using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using CTMapUtils;

namespace CrazyTaxi.Car
{
    class CarImpl : AbstractCar
    {
        private int[] dim;
        private Size gameFieldSize;
        private CollisionEntity entity;
        public CarController carCon { private set; get; }

        public CarImpl(int[] dim,Size gameFieldSize, CollisionEntity entity) 
        {
            this.dim = dim;
            this.Angle = 0;
            this.Size = new Size(23, 12);
            this.Car = CT_Helper.resizeImage(Properties.Resources.Taxi_GTA2, this.Size);
            this.entity = entity;                
            this.carCon = new CarController();
            this.Location = new Point(dim[0] / 2 - this.Car.Width / 2, dim[1] / 2 - this.Car.Height / 2);
            this.gameFieldSize = gameFieldSize;   
        }

        private void Move(int key,Rectangle bounds) 
        {
            double[] val = carCon.Move(key);
            this.Angle = (float)val[2];
            Point newPoint = new Point(Location.X+(int)val[0],Location.Y+(int)val[1]);
            if (bounds.X < newPoint.X && bounds.Y < newPoint.Y && bounds.Width > newPoint.X + this.Size.Width && bounds.Height > newPoint.Y + this.Size.Height)
            {
                this.Location = newPoint;
            }
            else 
            {
                carCon.Speed = 0;
            }
        }

        public override void Move(Rectangle bounds)
        {
           
            if (Up && Down)
            {
            }
            else if (Up)
            {
                Point cposition = new Point(Location.X + 23 / 2,Location.Y + 12 / 2);
                if (entity.Update(cposition, Convert.ToInt32(Angle + 90), true).Front)
                {
                    carCon.Speed = 0;
                }
                else
                {
                    Move((int)CarController.keys.up, bounds);
                }
            }
            else if (Down)
            {
                Point cposition = new Point(Location.X + 23 / 2, Location.Y + 12 / 2);
                if (entity.Update(cposition, Convert.ToInt32(Angle + 90), false).Back)
                {
                    carCon.Speed = 0;
                }
                else
                {
                    Move((int)CarController.keys.down, bounds);
                }
            }
            else
            {
                FinishMove(bounds);
                return;
            }

            if (Left && Right)
            {
            }
            else if (Left)
            {
                Move((int)CarController.keys.left, bounds);
            }
            else if (Right)
            {
                Move((int)CarController.keys.right, bounds);
            }
        }

        public override bool FinishMove(Rectangle bounds)
        { 
            double[] val = carCon.FinishMoving();
            int xvalue = (int)val[0];
            this.Angle = (float)val[2];
            Point newPoint = new Point(Location.X + (int)val[0], Location.Y + (int)val[1]); ;
            if (bounds.X < newPoint.X && bounds.Y < newPoint.Y && bounds.Width > newPoint.X + this.Size.Width && bounds.Height > newPoint.Y + this.Size.Height)
            {
                this.Location = newPoint;
            }
            else
            {
                carCon.Speed = 0;
            }
            
            if (xvalue == 0) 
            {
                return true;
            }
            return false;
        }


        public void draw(Graphics g)
        {
            
            int xP = calculateLocation(dim[0],gameFieldSize.Width,this.Location.X);
            int yP = calculateLocation(dim[1], gameFieldSize.Height, this.Location.Y);

           System.Drawing.Drawing2D.Matrix m = g.Transform;
            //here we do not need to translate, we rotate at the specified point
            float x = (float)(23 / 2) + (float)xP;
            float y = (float)(12 / 2) + (float)yP;
            m.RotateAt(this.Angle, new PointF(x, y), System.Drawing.Drawing2D.MatrixOrder.Append);
            g.Transform = m;
            g.DrawImageUnscaled(this.Car, new Point(xP, yP));
            g.ResetTransform();          
        }

        private int circleRadius = 2;
        public void drawMiniMap(Graphics g)
        {
            
            int x = dim[0] * Location.X / gameFieldSize.Width;
            int y = dim[1] * Location.Y / gameFieldSize.Height;

            g.FillEllipse(Brushes.Green,x - circleRadius,y - circleRadius,circleRadius * 2,circleRadius * 2);
            
            if (++circleRadius > 15)
            {
                circleRadius = 2;
            }
        }

        private int calculateLocation(int displayEdge,int gameFieldEdge,int position)
        {
            int result = displayEdge / 2;

            if (result > gameFieldEdge - position)
            {
                result = displayEdge - (gameFieldEdge - position);
            }
            else if (result > position)
            {
                result = position;
            }

            return result;
        }
        public Image Car { get; set; }

        public Point Location { get; set; }

        public Size Size { get; set; }

        public float Angle { get; set; }

       /* protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000020;
                createParams.ExStyle ^= 0x00000020;
                return base.CreateParams;
            }
        }*/
    }
}
