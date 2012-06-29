using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CTMapUtils;

namespace CrazyTaxi.Car
{
    class CarController
    {

        public enum keys
        {
            down = 0,
            up = 1,
            left = 2,
            right = 3
        }

        public static int MAX_SPEED = 7;
        public static int MIN_SPEED = -4;
        public static double ACCELERATION = 0.1;
        public static int ROTATION_SPEED = 7;

        public int Rotation { set; get; }
        public double Speed { set; get; }

        public CarController()
        {
            Rotation = 0;
            Speed = 0;
        }

        public  double[] Move(int key)
        {
            double[] movedWay;
            double x = 0;
            double y = 0;
            //Hält das auto vom drehen ab, wenn es nicht mehr fährt
            if (key == (int)keys.right) 
            {
                if (Speed != 0)
                {
                    Rotation += ROTATION_SPEED;
                }
            }
            else if (key == (int)keys.left) 
            {
                
                if (!Speed.Equals(0))
                {
                    Rotation -= ROTATION_SPEED;
                }
            }            
            else if (key==(int)keys.up)
            {
                if (Speed < MAX_SPEED)
                {
                    Speed+=ACCELERATION;
                }
                x = Speed * Math.Cos((Rotation) * Math.PI / 180);
                y = Speed * Math.Sin((Rotation) * Math.PI / 180);
            }
            else if (key == (int)keys.down) 
            {
                if (Speed > MIN_SPEED)
                {
                    Speed-=ACCELERATION;
                }
                x = Speed * Math.Cos((Rotation) * Math.PI / 180);
                y = Speed * Math.Sin((Rotation) * Math.PI / 180);
            }
            movedWay = new double[]{x,y,Rotation};
            return movedWay;
        }

        public double[] FinishMoving() 
        {
            double[] movedWay;
            double x = 0;
            double y = 0;
            //Hält das auto an
            if (Speed > 0)
            {
                Speed--;
            }
            else if (Speed < 0)
            {
                Speed++;
            }
            x = Speed * Math.Cos((Rotation) * Math.PI / 180);
            y = Speed * Math.Sin((Rotation) * Math.PI / 180);
            movedWay = new double[] { x, y, Rotation };
            return movedWay;
        }
    }
}
