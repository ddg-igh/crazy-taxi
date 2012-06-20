using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTMapUtils
{
    public class CentralParkCreator
    {
        public int[] TileUpperLeftEnd = new int[2];
        public int[] TileLowerRightEnd = new int[2];

        public CentralParkCreator(int MapWidth, int MapHeight)
        {
            GenerateCentralParkArea(MapWidth, MapHeight);
        }

        private void GenerateCentralParkArea(int MapWidth, int MapHeight)
        {
            //Richtwert der Parklänge ermitteln (jeweils 1/3)
            int theParkWidth = MapWidth / 3;
            int theParkHeight = MapHeight / 3;

            Random rnd = new Random();
            theParkWidth += rnd.Next(-theParkWidth / 2, theParkWidth / 2 + 1);
            theParkHeight += rnd.Next(-theParkHeight / 2, theParkHeight / 2 + 1);

            Console.WriteLine("Parkwidth: {0}, Parkheight: {1}", theParkWidth, theParkHeight);

            int tempPosLinks = (MapWidth - theParkWidth) / 2;
            tempPosLinks += rnd.Next(-1 / 2);

            int tempPosOben = (MapHeight - theParkHeight) / 2;
            tempPosOben += rnd.Next(-1 / 2);

            TileUpperLeftEnd[0] = tempPosLinks;
            TileUpperLeftEnd[1] = tempPosOben;

            TileLowerRightEnd[0] = tempPosLinks + theParkWidth - 1;
            TileLowerRightEnd[1] = tempPosOben + theParkHeight - 1;

            Console.WriteLine("StartLinksOben: {0},{1}", TileUpperLeftEnd[0], TileUpperLeftEnd[1]);
            Console.WriteLine("EndeRechtsUnten: {0},{1}", TileLowerRightEnd[0], TileLowerRightEnd[1]);
        }

        public bool IsInsideParkArea(int x, int y)
        {
            //Breitenposition überprüfen:
            if (x >= TileUpperLeftEnd[0] && x <= TileLowerRightEnd[0])
            {
                if (y >= TileUpperLeftEnd[1] && y <= TileLowerRightEnd[1])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
