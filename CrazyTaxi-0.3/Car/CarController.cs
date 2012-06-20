using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CTMapUtils;

namespace CrazyTaxi.Car
{
    class CarController : AbstractCarController
    {

        public CarController()
        {
            rotation = 0;
            speed = 0;
            maxSpeed = 5;
            minSpeed = -3;
        }

        public CarController(Rectangle rect)
        {
            bounds = rect;
            rotation = 0;
            speed = 0;
            maxSpeed = 5;
            minSpeed = -3;
        }

        public override int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public override double[] Move(int key)
        {
            double[] movedWay;
            double x = 0;
            double y = 0;
            //Hält das auto vom drehen ab, wenn es nicht mehr fährt
            if (key == (int)keys.right) 
            {
                if (speed != 0)
                {
                    rotation += 5;
                }
            }
            else if (key == (int)keys.left) 
            {
                
                if (!speed.Equals(0))
                {
                    rotation -= 5;
                }
            }            
            else if (key==(int)keys.up)
            {
                if (speed < maxSpeed)
                {
                    speed++;
                }
                x = speed * Math.Cos((rotation) * Math.PI / 180);
                y = speed * Math.Sin((rotation) * Math.PI / 180);
            }
            else if (key == (int)keys.down) 
            {
                if (speed > minSpeed)
                {
                    speed--;
                }
                x = speed * Math.Cos((rotation) * Math.PI / 180);
                y = speed * Math.Sin((rotation) * Math.PI / 180);
            }
            movedWay = new double[]{x,y,rotation};
            return movedWay;
        }

        public override double[] FinishMoving() 
        {
            double[] movedWay;
            double x = 0;
            double y = 0;
            //Hält das auto an
            if (speed > 0)
            {
                speed--;
            }
            else if (speed < 0)
            {
                speed++;
            }
            x = speed * Math.Cos((rotation) * Math.PI / 180);
            y = speed * Math.Sin((rotation) * Math.PI / 180);
            movedWay = new double[] { x, y, rotation };
            return movedWay;
        }
    }
}
