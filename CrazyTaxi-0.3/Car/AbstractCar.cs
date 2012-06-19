using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CrazyTaxi.Car
{
    abstract class AbstractCar 
    {

        public bool Right { get; set; }

        public bool Left { get; set; }

        public bool Up { get; set; }

        public bool Down{get;set;}

        public abstract void Move(Rectangle bounds);
        public abstract bool FinishMove(Rectangle bounds);
    }
}
