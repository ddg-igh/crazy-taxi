using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CTMapUtils
{
    public class GridElement
    {
        public enum Sides
        {
            North = 1,
            East = 2,
            South = 4,
            West = 8
        }

        /// <summary>
        /// Das Bild dieses GridElements
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// Seiten, von denen das Element vom Auto passierbar ist
        /// </summary>
        public int PassableSides { get; set; }
        /// <summary>
        /// Seiten, an denen das Element vom Zufallsgenerator mit anderen Elementen verbunden werden muss
        /// </summary>
        public int RandomPlacableSides { get; set; }

        public GridElement()
        {
        }
    }
}
