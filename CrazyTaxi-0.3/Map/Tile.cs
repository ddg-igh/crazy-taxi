using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CrazyTaxi.Map
{
    class Tile : DoubleBufferedPanel
    {
        private int _ID;
        private bool _collide;
        private Image _img;

        public Tile(int ID, bool collide, Image img) 
        {
            _ID = ID;
            _collide = collide;
            _img = img;
        }

        public Tile(int ID, bool collide, Color bgColor)
        {
            _ID = ID;
            _collide = collide;
            this.BackColor = bgColor;

        }

        public int ID
        {
            get { return _ID; }
        }

        public bool Collide
        {
            get { return _collide; }
        }

    }
}
