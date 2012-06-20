using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTMapUtils
{
    public class RiverCrossingCreator
    {
        public int[] TileUpperLeftEnd = new int[2];
        public int[] TileLowerRightEnd = new int[2];

        public RiverDirection TheRiverDirection;

        public int[] BridgePositionsVertical;
        public int[] BridgePositionsHorizontal;

        public enum RiverDirection
        {
            horizontal,
            vertical
        }

        public RiverCrossingCreator(int MapWidth, int MapHeight)
        {
            TheRiverDirection = SetRiverDirection(MapWidth, MapHeight);

            //Console.WriteLine("Richtung: {0}", TheRiverDirection.ToString());
            SetRiverDimensions(MapWidth, MapHeight);

            if (TheRiverDirection == RiverDirection.horizontal)
            {
                SetBridges(TheRiverDirection, MapWidth);
            }

            else
            {
                SetBridges(TheRiverDirection, MapHeight);
            }

        }

        public bool BridgeAtThisPosition(int currentPos, int[] PosList)
        {
            foreach (int pos in PosList)
            {
                if (pos == currentPos)
                {
                    return true;
                }
            }

            return false;
        }

        private RiverDirection SetRiverDirection(int MapWidth, int MapHeight)
        {
            //die Flussrichtung wählen; Abgreifen, wenn eine Seite besonders schmal ist:

            //Wenn die Breite mind 1,5x so groß wie die Höhe ist, vertikalen Fluss setzen:
            if (Convert.ToDouble(MapWidth) / Convert.ToDouble(MapHeight) >= 1.5)
            { return RiverDirection.vertical; }
            //Wenn umgekehrt bei der Höhe:
            if (Convert.ToDouble(MapHeight) / Convert.ToDouble(MapWidth) >= 1.5)
            { return RiverDirection.horizontal; }
            //Ansonsten zufällig wählen:
            else
            {
                Random rnd = new Random();
                if (rnd.Next(0, 2) == 0)
                {
                    return RiverDirection.vertical;
                }
                else
                {
                    return RiverDirection.horizontal;
                }
            }

            //return RiverDirection.horizontal;
        }

        private void SetRiverDimensions(int MapWidth, int MapHeight)
        {
            int startX = 0;
            int endX = 0;
            int startY = 0;
            int endY = 0;

            int riverthickness;

            //bei horizontalem Flusslauf ("London Style"):
            if (TheRiverDirection == RiverDirection.horizontal)
            {
                startX = 0;
                endX = MapWidth - 1;

                riverthickness = GetRiverThickness(MapHeight);

                SetRiverPosition(MapHeight, ref startY, ref endY, riverthickness);


            }

            //bei vertikalem Flusslauf ("New York Style"):
            else
            {
                startY = 0;
                endY = MapWidth - 1;

                riverthickness = GetRiverThickness(MapWidth);

                SetRiverPosition(MapHeight, ref startX, ref endX, riverthickness);
            }

            //Console.WriteLine("Dicke: " + riverthickness);

            TileUpperLeftEnd[0] = startX;
            TileUpperLeftEnd[1] = startY;

            TileLowerRightEnd[0] = endX;
            TileLowerRightEnd[1] = endY;

            //Console.WriteLine("LO: {0},{1} | RU: {2},{3}",TileUpperLeftEnd[0],TileUpperLeftEnd[1],TileLowerRightEnd[0],TileLowerRightEnd[1]);
        }

        private int GetRiverThickness(int totalLength)
        {
            if (totalLength < 15)
            {
                return 2;
            }

            else
            {
                Random rnd = new Random();
                int[] values = new int[] { 2, 3, 3, 3, 4, 4 };

                return values[rnd.Next(0, values.Length)];
            }
        }

        private void SetRiverPosition(int MapHeight, ref int start, ref int end, int riverthickness)
        {
            // das optimale Maß für eine Uferseite finden:
            int standardShoreDist = Convert.ToInt32((MapHeight - riverthickness) / 2);

            Random rnd = new Random();

            if (standardShoreDist < 7)
            {
                //schmalere Toleranz setzen, wenn die Uferseiten von Natur aus schmal sind:
                standardShoreDist += rnd.Next(-1, 2);

                start = standardShoreDist;
                end = standardShoreDist + riverthickness - 1;
            }
            else
            {
                //...hier mehr Spielraum möglich:
                standardShoreDist += rnd.Next(-3, 4);

                start = standardShoreDist;
                end = standardShoreDist + riverthickness - 1;
            }

        }

        public bool IsInsideArea(int x, int y)
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

        private void SetBridges(RiverDirection TheRiverDirection, int Distance)
        {
            int bridgeAmount;

            if (Distance <= 15)
            {
                bridgeAmount = 2;
            }
            else
            {
                Random rnd = new Random();
                int[] values = new int[] { 2, 3, 3, 4 };

                bridgeAmount = values[rnd.Next(0, values.Length)];
            }

            int counter = 0;

            if (TheRiverDirection == RiverDirection.horizontal)
            {
                BridgePositionsHorizontal = new int[bridgeAmount];
                for (int i = 0; i < bridgeAmount; i++)
                {
                    BridgePositionsHorizontal[i] = -10;
                }

                Random rnd = new Random();
                bool wrongassigned = false;

                while (true)
                {
                    int tempValue = rnd.Next(0, Distance);

                    foreach (int compvalue in BridgePositionsHorizontal)
                    {
                        if (compvalue == tempValue - 1 || compvalue == tempValue + 1 || compvalue == tempValue)
                        {
                            wrongassigned = true;
                        }
                    }

                    if (!wrongassigned)
                    {
                        BridgePositionsHorizontal[counter] = tempValue;
                        counter++;
                    }
                    wrongassigned = false;

                    if (counter == bridgeAmount)
                    {
                        break;
                    }
                }
            }

            else
            {
                BridgePositionsVertical = new int[bridgeAmount];
                for (int i = 0; i < bridgeAmount; i++)
                {
                    BridgePositionsVertical[i] = -10;
                }
                Random rnd = new Random();
                bool wrongassigned = false;

                while (true)
                {
                    int tempValue = rnd.Next(0, Distance);

                    foreach (int compvalue in BridgePositionsVertical)
                    {
                        if (compvalue == tempValue - 1 || compvalue == tempValue + 1 || compvalue == tempValue)
                        {
                            wrongassigned = true;
                        }
                    }

                    if (!wrongassigned)
                    {
                        BridgePositionsVertical[counter] = tempValue;
                        counter++;
                    }
                    wrongassigned = false;

                    if (counter == bridgeAmount)
                    {
                        break;
                    }
                }
            }
        }

        public bool BridgeOnPosition(int position, RiverDirection riverDirection)
        {
            if (riverDirection == RiverDirection.horizontal)
            {
                foreach (int bridgeposition in BridgePositionsHorizontal)
                {
                    if (bridgeposition == position)
                    {
                        return true;                        
                    }                    
                }
            }

            else if (riverDirection == RiverDirection.vertical)
            {
                foreach (int bridgeposition in BridgePositionsVertical)
                {
                    if (bridgeposition == position)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
