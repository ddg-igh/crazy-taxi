using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CrazyTaxi.Map
{
    class TileMap : DoubleBufferedPanel
    {
        private Tile[,] map;
        private Size _mapSize;

        public TileMap(Size size) 
        {
            _mapSize = size;

            List<Tile> tiles = new List<Tile>();
            tiles.Add(new Tile(1, false, Color.Red)); /*TopRight*/
            tiles.Add(new Tile(2, false, Color.White)); /*TopLeft*/
            tiles.Add(new Tile(3, false, Color.Blue)); /*Left and right*/
            tiles.Add(new Tile(4, false, Color.Brown)); /*Top and down*/
            tiles.Add(new Tile(5, false, Color.Yellow)); /*DownRight*/
            tiles.Add(new Tile(6, false, Color.Gray)); /*DownLeft*/
            tiles.Add(new Tile(7, true, Color.Black)); /*Block*/

            map = new Tile[3,3];
            for (int i = 0; i < size.Height; i++ )
            {
                Tile[] row = new Tile[size.Height];   
                for (int k = 0; k < size.Width; k++) 
                {
                    map[i,k] = tiles[(k+1)*i];
                }
            }
        }

        public Size MapSize
        {
            get { return _mapSize; }
        }

        internal Tile[,] Map
        {
            get { return map; }
        }

        public Tile[] getRow(int row, int column) 
        {

            return null;
        }

        public Tile[] getColumn(int row, int column)
        {
            return null;
        }
    }
}
