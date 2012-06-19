using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;


///////////////////////////////////////////////////////////////////////////////////////////////////////
// RandomMap.cs                                                                                     
// A. Hausen
///////////////////////////////////////////////////////////////////////////////////////////////////////

// Erzeugt anhand der Map Klasse die zugehörige Konfiguration einer Karte incl. Kacheln und Dimensionen.
// Zentrales Element ist die Methode "private string AssignTileId()". In dieser wird durch einen Zufallsalgorithmus die
// Karte mit zusammenhängendem Straßenverlauf konstruiert.

namespace CTMapUtils
{
    public class RandomMap
    {
        //Debugmöglichkeit:
        const bool B_DEBUG_ASSIGN_ID = false;

        // zentrale Properties für die Kartenkonstruktion:
        // tileWidth: Breite eines Kachelelements in Pixel
        // tileHeight: Höhe eines Kachelelements in Pixel
        // buildingFactor: voreingestellte Wahrscheinlichkeit für die Platzierung von Gebäuden bzw. nicht passierbaren Kacheln (s.u.) 
        int tileWidth = 0;
        int tileHeight = 0;
        int buildingFactor = 0;

        //Zentraler Zufallsgenerator für Verzweigungen unten.
        Random rnd = new Random();

        //Die Liste aller verfügbaren Elemente:
        List<GridElement> GridElementPossibilitiesList = new List<GridElement>();

        public Map GenerateRandomMap(int width, int height)
        {
            //Map deklarieren
            Map returnvalue = null;
            //Zugehöriges erzeugen:
            Grid tempGrid = new Grid();

            //Die Elementliste aus der Xml "ElementList.xml" ziehen:
            GridElementPossibilitiesList = GetListFromXML();

            //Im Folgenden das Grid in der in den Parametern zugewiesenen Dimension als Jagged Array erzeugen.
            tempGrid.GridElementCollection = new GridElement[height][];

            for (int i = 0; i < height; i++)
            {
                tempGrid.GridElementCollection[i] = new GridElement[width];
            }

            tileWidth = width;
            tileHeight = height;

            SetMapProperties(width, height);
            tempGrid = SetMapBoundaries(width, height);

            SetTileIds(tempGrid.GridElementCollection);

            returnvalue = new Map(tempGrid);

            return returnvalue;
        }

        private Grid SetMapBoundaries(int width, int height)
        {
            Grid grid = null;
            grid = new Grid();

            grid.Height = 500;
            grid.Width = 500;


            //For Schleife neu strukturieren
            grid.GridElementCollection = new GridElement[width][];
            for (var i = 0; i < grid.GridElementCollection.Length; i++)
            {
                grid.GridElementCollection[i] = new GridElement[width];

                //Initialisierung iterieren:
                for (int j = 0; j < grid.GridElementCollection[i].Length; j++)
                {
                    //Console.WriteLine("{0} / {1}", i, j);
                    grid.GridElementCollection[i][j] = new GridElement();
                    grid.GridElementCollection[i][j].ImageId = "n/a";
                    grid.GridElementCollection[i][j].PassableSides = 0;
                    grid.GridElementCollection[i][j].RandomPlacableSides = 0;
                }
            }

            return grid;

        }// private void SetMapBoundaries(int width, int height)

        private void SetMapProperties(int width, int height)
        {
            int buildingProbability = 70; //testweise hier hardcoded

            SetBuildingFactor(buildingProbability);
        }

        private void SetBuildingFactor(int buildingProbability)
        {
            if (buildingProbability > 0)
            {
                buildingFactor = buildingProbability / 10;
            }
        }

        /// <summary>
        /// Das gesamte Grid wird Kachel für Kachel durchlaufen, auf nachbarschaftliche Verhältnisse überprüft; jeweils jedes Kachelelement bekommt eine TileID, die auf eine Grafikdatei verweist.
        /// </summary>
        /// <param name="gridElementCollection"></param>
        private void SetTileIds(GridElement[][] gridElementCollection)
        {
            int counter = 1;
            bool bAssignedCorrect = false;

            while (!bAssignedCorrect)
            {

                for (int y = 0; y < tileHeight; y++)
                {
                    for (int x = 0; x < tileWidth; x++)
                    {
                        gridElementCollection[x][y].ImageId = AssignTileId(x, y, gridElementCollection);

                        if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("zugewiesen: {0}", gridElementCollection[x][y].ImageId); }

                        if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("Durchlauf: {0}, Position: {1}/{2}", counter, x, y); }
                        counter++;
                    }

                }

                bAssignedCorrect = CheckForCorrectAssignment(gridElementCollection);

            }


            //Eine beliebiges Zutreffendes Tile aus der XML laden:
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    GetTileFromXml(gridElementCollection[x][y]);

                }

            }
        }

        private void GetTileFromXml(GridElement gridElement)
        {
            //Durchlaufen, und nach passable Sites durchsuchen:
            int fundIndex = 0;

            for (int i = 0; i < GridElementPossibilitiesList.Count; i++)
            {
                if (GridElementPossibilitiesList[i].PassableSides == Convert.ToInt32(gridElement.ImageId))
                {
                    fundIndex = i;
                }
            }

            gridElement.ImageId = GridElementPossibilitiesList[fundIndex].ImageId;
            gridElement.PassableSides = GridElementPossibilitiesList[fundIndex].PassableSides;
            gridElement.RandomPlacableSides = GridElementPossibilitiesList[fundIndex].RandomPlacableSides;
        }

        /// <summary>
        /// Hauptalgorithmus, Zuweisung der Tile-IDs, abhängig von den Nachbarn. Die Funktion besteht aus folgenden Schritten:
        /// 1.: Feststellung der relativen Position der aktuellen Kachel; hieraus ergeben sich unterschiedliche Verfahren der ID-Zuweisung.
        /// 2.: Sofern auf der aktuellen Kachel die Möglichkeit besteht, ein nicht passierbares Objekt (z.B. Gebäude etc.) zu platzieren, wird per Zufall unter Einfluss der Wahrscheinlichkeit ermittelt, ob dieses platziert wird.
        /// 3.: Für die jeweilige Position werden alternativ die Möglichkeiten ermittelt. Gibt es mehrere, wird per Zufall eine ID ermittelt.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="gridElement"></param>
        /// <returns></returns>
        private string AssignTileId(int posX, int posY, GridElement[][] gridElement)
        {
            if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("Zuweisung für Position: {0}/{1}", posX, posY); };

            int returnvalue = 0;

            //Fall 1: Platzierung Links Oben
            //  XOOOO
            //  OOOOO
            //  OOOOO
            //  OOOOO

            //Kurzdokumentation der jeweiligen TileIDs:
            // 0 = Gebäude;
            // 3 = Kurve        oben - rechts           
            // 5 = Gerade       oben - unten
            // 6 = Kurve        rechts - unten
            // 7 = T-Kreuzung   oben - rechts - unten
            // 9 = Kurve        oben -rechts
            // usw.

            if (posX == 0 && posY == 0)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 1: Links Oben---"); }

                if (tileWidth > 2 && tileHeight > 2)
                {
                    if (PlaceBuilding())
                    {
                        return "0"; //TODO: aus XML Laden
                    }
                    return "6"; //TODO: aus XML Laden
                }
            }

            //Fall 2: Platzierung Oben, Mitte
            //  OXXXO
            //  OOOOO
            //  OOOOO
            //  OOOOO

            else if (posY == 0 && posX != 0 && posX != tileWidth - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 2: Obere Reihe, Mitte---"); }

                if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2) == false)
                {
                    if (PlaceBuilding())
                    {
                        return "0"; //TODO: aus XML Laden
                    }
                    return "6"; //TODO: aus XML Laden

                }
                else
                {
                    int[] possibleTiles = new int[] { 10, 14, 12 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }
            }

            //Fall 3: Oben Rechts
            //  OOOOX
            //  OOOOO
            //  OOOOO
            //  OOOOO
            else if (posY == 0 && posX == tileWidth - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 3: Oben Rechts---"); }
                if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2) == false)
                {
                    return "0";
                }
                return "12";
            }


            //Fall 4: Ganz links, mittig
            //  OOOOO
            //  XOOOO
            //  XOOOO
            //  OOOOO
            else if (posX == 0 && posY != 0 && posY != tileHeight - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 4: Ganz links, mittig---"); }

                if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4) == false)
                {
                    if (PlaceBuilding())
                    {
                        return "0";
                    }
                    return "6";
                }
                else
                {
                    int[] possibleTiles = new int[] { 3, 5, 7 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }
            }


            //Fall 5: Unten links
            //  OOOOO
            //  OOOOO
            //  OOOOO
            //  XOOOO
            else if (posY == tileHeight - 1 && posX == 0)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 5: Unten links---"); }

                //Console.WriteLine(Convert.ToInt32(gridElement[posX - 1][posY].ImageId));

                //if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 4) == false)
                if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4) == false)
                {
                    //if (PlaceBuilding())
                    //{
                    return "0";
                    //}
                }
                return "3";
            }

            //Fall 6: Mittig, kein RandTile
            //  OOOOO
            //  OXXXO
            //  OXXXO
            //  OOOOO
            else if (posY != 0 && posX != 0 && posY != tileHeight - 1 && posX != tileWidth - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 6: Mittig, kein RandTile---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                if (streetFromAbove == false && streetFromLeft == false)
                {
                    //Console.WriteLine("Type: oben nicht links nicht");
                    if (PlaceBuilding())
                    {
                        return "0";
                    }
                    else
                    {
                        return "6";
                    }
                }

                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links");

                    int[] possibleTiles = new int[] { 9, 13, 15 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }

                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    //Console.WriteLine("Type: oben nicht links");
                    int[] possibleTiles = new int[] { 10, 12, 14 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }
                else if (streetFromLeft == false && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links nicht");
                    int[] possibleTiles = new int[] { 3, 5, 7 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }
            }

            //Fall 7: Rechts, mittig
            //  OOOOO
            //  OOOOX
            //  OOOOX
            //  OOOOO
            else if (posY != 0 && posX == tileWidth - 1 && posY != tileHeight - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 7: Rechts, mittig---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                if (streetFromAbove == false && streetFromLeft == false)
                {
                    //Console.WriteLine("Type: oben nicht links nicht");
                    return "0";
                }

                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links");
                    int[] possibleTiles = new int[] { 9, 13 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }

                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    //Console.WriteLine("Type: oben nicht links");
                    return "12";
                }
                else if (streetFromLeft == false && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links nicht");
                    return "5";
                }
            }

            //Fall 8: Unten, mittig
            //  OOOOO
            //  OOOOO
            //  OOOOO
            //  OXXXO
            else if (posX != 0 && posY == tileHeight - 1 && posX != tileWidth - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 8: Unten, mittig---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                if (streetFromAbove == false && streetFromLeft == false)
                {
                    //Console.WriteLine("Type: oben nicht links nicht");
                    return "0";
                }

                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    Console.WriteLine("Type: oben links");

                    int[] possibleTiles = new int[] { 9, 11 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }

                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    //Console.WriteLine("Type: oben nicht links");

                    return "10";
                }
                else if (streetFromLeft == false && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links nicht");

                    return "3";
                }
            }


            //Fall 9: Unten, rechts
            //  OOOOO
            //  OOOOO
            //  OOOOO
            //  OOOOX
            else if (posY == tileHeight - 1 && posX == tileWidth - 1)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 9: Unten, rechts---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                if (streetFromAbove == false && streetFromLeft == false)
                {
                    //Console.WriteLine("Type: oben nicht links nicht");
                    return "0";
                }

                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links");

                    return "9";
                }

                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    //Console.WriteLine("Type: oben nicht links");


                    //SetTileIds(theMapGrid); //hier Eigentlich die Berechnung neu starten

                    return "10";
                }
                else if (streetFromLeft == false && streetFromAbove == true)
                {

                    //SetTileIds(theMapGrid); //hier Eigentlich die Berechnung neu starten

                    return "3";
                }
            }






            return Convert.ToString(returnvalue); //Default-Return
        }




        // Hilfsfunktionen

        private bool PlaceBuilding()
        {
            if (buildingFactor > 0)
            {
                int value = rnd.Next(1, 11);

                if (value <= buildingFactor)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Überprüft, ob der vorherige ID-Zuweisungsdurchlauf erfolgreich war, und gibt das Resultat zurück.
        /// </summary>
        /// <param name="gridElementCollection">GridElementCollection in der Größe der zu erzeugenden Map</param>
        /// <returns></returns>
        private bool CheckForCorrectAssignment(GridElement[][] gridElementCollectionTemp)
        {
            int lengthX = gridElementCollectionTemp.GetLength(0);
            int lengthY = gridElementCollectionTemp[0].Length;

            bool bUpperElementConnected = false;
            bool bLeftElementConnected = false;

            //GridElement an Position über unterem Eckelement rechts auf Anbindung nach unten überprüfen.
            if (CheckIfBitIsIncluded(Convert.ToInt32(gridElementCollectionTemp[lengthX - 1][lengthY - 2].ImageId), 4))
            {
                bUpperElementConnected = true;
            }

            //GridElement an Position links vom unterem Eckelement rechts auf Anbindung nach unten überprüfen.
            if (CheckIfBitIsIncluded(Convert.ToInt32(gridElementCollectionTemp[lengthX - 2][lengthY - 1].ImageId), 2))
            {
                bLeftElementConnected = true;
            }

            //Zuweisung korrekt, wenn die Booleans entweder beide true oder beide false (Straßennetz geschlossen):
            if ((bLeftElementConnected && bUpperElementConnected) || (bLeftElementConnected && bUpperElementConnected))
            {
                return true;
            }

            //Wenn die beiden Booleans unterschiedlich sind, mit 'false' raus (Zuweisung fehlerhaft):
            return false;
        }

        public bool CheckIfBitIsIncluded(int sourceValue, int bitValue)
        {
            int startPotency = 1;

            while (sourceValue > startPotency)
            {
                startPotency *= 2;
            }
            startPotency /= 2;
            //Console.WriteLine("Startpotenz: " + startPotency);

            //source = 37
            //bit = 2
            //startpotency = 32

            while (startPotency > 0)
            {
                //Console.WriteLine("Aktuelle Potenz: "+startPotency);
                if (sourceValue >= startPotency)
                {
                    //Console.WriteLine("Source ist größer als die gegenwärtige Potenz: "+sourceValue+"/"+startPotency);
                    //check, ob der gegenwärtige startPotency-Wert dem bitValue entspricht
                    //Beispiel: startPotency = 32, bit = 2 -> Also: der gesuchte Bitwert wird hier (noch) nicht gefunden
                    if (startPotency == bitValue)
                    {
                        //Console.WriteLine("StartPotenz = Bitwert");
                        return true; //Bitwert enthalten!
                    }

                    //Den sourceValue - die gegenwärtige 2erPotenz:
                    //Beispiel: 37-32 = 5
                    sourceValue -= startPotency;

                }

                //bitwert um die Hälfte verringern:
                //Beispiel: Runde1: /2 = 16
                //usw...
                startPotency /= 2;

                if (startPotency == 1)
                { startPotency = 0; }
            }

            //Console.WriteLine("StartPotenz:" + startPotency);

            // Bit nicht gefunden!
            return false;
        }

        private string findRandomFromTilePossibilities(int[] possibleTiles)
        {
            int value = rnd.Next(0, possibleTiles.Length);
            return Convert.ToString(possibleTiles[value]);
        }

        private List<GridElement> GetListFromXML()
        {
            List<GridElement> TempList = new List<GridElement>();

            XmlSerializer ser = new XmlSerializer(typeof(List<GridElement>));
            StreamReader sr = new StreamReader(@"ElementList.xml");

            TempList = (List<GridElement>)ser.Deserialize(sr);
            sr.Close();

            return TempList;
        }

    }
}
