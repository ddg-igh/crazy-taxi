using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CrazyTaxi.Car
{
    abstract class AbstractCarController
    {
        protected int rotation;
        protected int speed;
        protected int maxSpeed;
        protected int minSpeed;
        protected Rectangle bounds;
        public enum keys
        {
            down = 0,
            up = 1,
            left = 2,
            right = 3
        }

        public abstract int Speed{get; set;}
        public abstract double[] Move(int key);
        public abstract double[] FinishMoving();
    }
}
