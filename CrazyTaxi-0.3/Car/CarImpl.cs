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
    class CarImpl
    {
        private static int MAX_DMG = 100;
        private static int COLLISION_MAX_DMG = 10;

        private Size dim;
        private Size gameFieldSize;
        private int dmg=0;
        private CollisionEntity entity;
        public CarController carCon { private set; get; }

        private Image destroyedCar;
        public Image Car { get; set; }

        public Point Location { get; set; }

        public Size Size { get; set; }

        public float Angle { get; set; }

        public bool Destroyed { get; private set; }

        public bool Right { get; set; }

        public bool Left { get; set; }

        public bool Up { get; set; }

        public bool Down { get; set; }

        public CarImpl(Size dim,Size gameFieldSize, CollisionEntity entity,Size size) 
        {
            this.dim = dim;
            this.Angle = 0;
            this.Size = size;
            this.Car = CT_Helper.resizeImage(Properties.Resources.Taxi_GTA2, this.Size);
            this.destroyedCar=CT_Helper.resizeImage(Properties.Resources.GTA2_CAR_71,this.Size);
            this.entity = entity;                
            this.carCon = new CarController();
            this.Location = new Point(dim.Width / 2 - this.Car.Width / 2, dim.Height / 2 - this.Car.Height / 2);
            this.gameFieldSize = gameFieldSize;
            this.Destroyed = false;
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

        public  void Move(Rectangle bounds)
        {
            if (Destroyed)
            {
                return;
            }

            Point cPosition = new Point(Location.X + Size.Width / 2, Location.Y + Size.Height / 2);
           
            if (Up && Down)
            {
                if (entity.Update(cPosition, Convert.ToInt32(Angle + 90), true).Front || entity.Update(cPosition, Convert.ToInt32(Angle + 90), false).Back)
                {
                    collide();
                }
                FinishMove(bounds);

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
            else if (Up)
            {

                if (entity.Update(cPosition, Convert.ToInt32(Angle + 90), true).Front)
                {
                    collide();
                }
                else
                {
                    Move((int)CarController.keys.up, bounds);
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
            else if (Down)
            {
                if (entity.Update(cPosition, Convert.ToInt32(Angle + 90), false).Back)
                {
                    collide();
                }
                else
                {
                    Move((int)CarController.keys.down, bounds);
                }

                if (Left && Right)
                {
                }
                else if (Left)
                {
                    Move((int)CarController.keys.right, bounds); //Wenn man rückwärts fährt ist die Drehung umgekehrt
                }
                else if (Right)
                {
                    Move((int)CarController.keys.left, bounds);
                }
            }
            else
            {
                if (entity.Update(cPosition, Convert.ToInt32(Angle + 90), true).Front || entity.Update(cPosition, Convert.ToInt32(Angle + 90), false).Back)
                {
                    collide();
                }
                FinishMove(bounds);

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

        }

        private void collide()
        {
            if (dmg >= MAX_DMG)
            {
                Destroyed = true;
                dmg = MAX_DMG;
            }
            else
            {
                dmg += (int)(COLLISION_MAX_DMG * (Math.Abs(carCon.Speed) / CarController.MAX_SPEED));
            }
            carCon.Speed = 0;
        }

        public  bool FinishMove(Rectangle bounds)
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
            
            int xP = calculateLocation(dim.Width,gameFieldSize.Width,this.Location.X);
            int yP = calculateLocation(dim.Height, gameFieldSize.Height, this.Location.Y);

           System.Drawing.Drawing2D.Matrix m = g.Transform;
            //here we do not need to translate, we rotate at the specified point
            float x = (float)(Size.Width / 2) + (float)xP;
            float y = (float)(Size.Height / 2) + (float)yP;
            m.RotateAt(this.Angle, new PointF(x, y), System.Drawing.Drawing2D.MatrixOrder.Append);
            g.Transform = m;
            if (Destroyed)
            {
                g.DrawImageUnscaled(this.destroyedCar, new Point(xP, yP));
            }
            else
            {
                g.DrawImageUnscaled(this.Car, new Point(xP, yP));
            }
            g.ResetTransform();


            Brush brush = Brushes.Green;
            if (dmg > 50 && dmg <=80)
            {
                brush = Brushes.Orange;
            }
            else if (dmg > 80)
            {
                brush = Brushes.Red;
            }

            g.DrawString("Schaden:" + dmg.ToString() + " %", new Font(FontFamily.GenericSansSerif, 10), brush, dim.Width/2, 10);
        }

        private double circleRadius = 2;
        public void drawMiniMap(Graphics g)
        {
            
            int x = (int)Math.Ceiling((double)(dim.Width * Location.X / gameFieldSize.Width));
            int y = (int)Math.Ceiling((double)(dim.Height * Location.Y / gameFieldSize.Height));

            g.FillEllipse(Brushes.Green,x-(int)circleRadius ,y-(int)circleRadius,(int)(circleRadius * 2),(int)(circleRadius * 2));

            circleRadius += 0.5;
            if (circleRadius > 15)
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
