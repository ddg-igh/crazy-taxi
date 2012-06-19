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
    class CarPanel : AbstractCarPanel
    {
        private Image car;

        public Image Car
        {
            get { return car; }
            set { car = value; }
        }

        private float angle = 0;

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        private CarController carCon;

        public CarPanel(int[] dim) 
        {
            this.DoubleBuffered = true;
            car = CT_Helper.resizeImage(Properties.Resources.Taxi_GTA2, new Size(46, 24));
            this.Size = new Size(46, 46);
            this.BackColor = Color.Transparent;
            carCon = new CarController();
            this.Location = new Point(dim[0]/2-car.Width/2,dim[1]/2-car.Height/2);
            this.Paint += new PaintEventHandler(Car_Paint);
            
        }

        public override void Move(int key,Rectangle bounds) 
        {
            double[] val = carCon.Move(key);
            angle = (float)val[2];
            Point newPoint = new Point(Location.X+(int)val[0],Location.Y+(int)val[1]);
            if (bounds.X < newPoint.X && bounds.Y < newPoint.Y && bounds.Width > newPoint.X + this.Width && bounds.Height > newPoint.Y + this.Height)
            {
                this.Location = newPoint;
            }
            else 
            {
                carCon.Speed = 0;
            }
        }

        public override void Move(int key)
        {
            double[] val = carCon.Move(key);
            angle = (float)val[2];
            Point newPoint = new Point(Location.X + (int)val[0], Location.Y + (int)val[1]);
            this.Location = newPoint;
        }

        public override bool FinishMove(Rectangle bounds)
        { 
            double[] val = carCon.FinishMoving();
            int xvalue = (int)val[0];
            angle = (float)val[2];
            Point newPoint = new Point(Location.X + (int)val[0], Location.Y + (int)val[1]); ;
            if (bounds.X < newPoint.X && bounds.Y < newPoint.Y && bounds.Width > newPoint.X + this.Width && bounds.Height > newPoint.Y + this.Height)
            {
                this.Location = newPoint;
            }
            else
            {
                carCon.Speed = 0;
            }
            this.Invalidate();
            if (xvalue == 0) 
            {
                return true;
            }
            return false;
        }

        public override bool FinishMove()
        {
            double[] val = carCon.FinishMoving();
            int xvalue = (int)val[0];
            angle = (float)val[2];
            Point newPoint = new Point(Location.X + (int)val[0], Location.Y + (int)val[1]); ;
            this.Location = newPoint;
            this.Invalidate();
            if (xvalue == 0)
            {
                return true;
            }
            return false;
        }

        private void Car_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(46/2, 46/2);
            g.RotateTransform(angle);
            g.TranslateTransform(-(46/2), -(46/2));
            g.DrawImage(car, new Point(0, (46/2)-car.Height/2 ));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000020;
                createParams.ExStyle ^= 0x00000020;
                return base.CreateParams;
            }
        }
    }
}
