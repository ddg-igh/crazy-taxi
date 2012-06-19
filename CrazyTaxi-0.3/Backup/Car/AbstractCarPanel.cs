using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CrazyTaxi.Car
{
    abstract class AbstractCarPanel : DoubleBufferedPanel 
    {
        public abstract void Move(int key, Rectangle bounds);
        public abstract void Move(int key);
        public abstract bool FinishMove(Rectangle bounds);
        public abstract bool FinishMove();
    }
}
