using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTMapUtils
{
    public class Grid
    {

        /// <summary>
        /// Alle Elemente in diesem Grid
        /// </summary>
        public GridElement[][] GridElementCollection { get; set; }

        /// <summary>
        /// Höhe des Grids in Pixeln
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Breite des Grids in Pixeln
        /// </summary>
        public int Width { get; set; }

        public Grid()
        {
        }

       
    }
}
