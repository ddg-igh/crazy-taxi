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
        const bool B_DEBUG_ASSIGNDETAIL_ID = false;

        // zentrale Properties für die Kartenkonstruktion:
        // tileWidth: Breite eines Kachelelements in Pixel
        // tileHeight: Höhe eines Kachelelements in Pixel
        // buildingFactor: voreingestellte Wahrscheinlichkeit für die Platzierung von Gebäuden bzw. nicht passierbaren Kacheln (s.u.) 
        int tileWidth = 0;
        int tileHeight = 0;
        int buildingFactor = 0;

        int tileCountX = 0;
        int tileCountY = 0;

        //Zentraler Zufallsgenerator für Verzweigungen unten.
        Random rnd = new Random();

        //Die Liste aller verfügbaren Elemente:
        List<GridElement> GridElementPossibilitiesList = new List<GridElement>();
        List<GridElement> ParkGridElementPossibilitiesList = new List<GridElement>();

        public MapParser.SpecialMapElement theSpecialMapElement;
        CentralParkCreator theCentralParkCreator;
        RiverCrossingCreator theRiverCrossingCreator;

        bool bRiverAssignmentValid = true;

        public Map GenerateRandomMap(int width, int height, MapParser.SpecialMapElement specialMapElement)
        {
            //Map deklarieren
            Map returnvalue = null;
            //Zugehöriges Grid erzeugen:
            Grid tempGrid = new Grid();

            tileCountX = width;
            tileCountY = height;

            theSpecialMapElement = specialMapElement;
            if (theSpecialMapElement == MapParser.SpecialMapElement.CentralPark)
            {
                theCentralParkCreator = new CentralParkCreator(tileCountX, tileCountY);
                ParkGridElementPossibilitiesList = GetListFromXML(@"ParkElementList.xml");
            }

            else if (theSpecialMapElement == MapParser.SpecialMapElement.RiverCrossing)
            {
                theRiverCrossingCreator = new RiverCrossingCreator(tileCountX, tileCountY);
            }

            //Die Elementliste aus der Xml "ElementList.xml" ziehen:
            GridElementPossibilitiesList = GetListFromXML(@"ElementList.xml");

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
                    if (theSpecialMapElement == MapParser.SpecialMapElement.CentralPark)
                    {
                        if (theCentralParkCreator.IsInsideParkArea(x, y))
                        {
                            GetParkFromXml(gridElementCollection[x][y]);
                        }

                        else
                        {
                            GetTileFromXml(gridElementCollection[x][y]);
                        }
                    }

                    else if (theSpecialMapElement == MapParser.SpecialMapElement.RiverCrossing)
                    {
                        if (theRiverCrossingCreator.IsInsideArea(x, y))
                        {
                            string imageId = "river";
                            int passableSides = 0;
                            int placeableSides = 0;


                            if (theRiverCrossingCreator.BridgePositionsHorizontal != null)
                            {
                                foreach (int Position in theRiverCrossingCreator.BridgePositionsHorizontal)
                                {
                                    if (x == Position)
                                    {
                                        imageId = "bridgevert";
                                        passableSides = 5;
                                        placeableSides = 5;
                                    }
                                }
                            }

                            if (theRiverCrossingCreator.BridgePositionsVertical != null)
                            {
                                foreach (int Position in theRiverCrossingCreator.BridgePositionsVertical)
                                {
                                    if (y == Position)
                                    {
                                        imageId = "bridgehori";
                                        passableSides = 10;
                                        placeableSides = 10;
                                    }
                                }
                            }

                            gridElementCollection[x][y].ImageId = imageId;
                            gridElementCollection[x][y].PassableSides = passableSides;
                            gridElementCollection[x][y].RandomPlacableSides = placeableSides;
                        }

                        else
                        {
                            GetTileFromXml(gridElementCollection[x][y]);
                        }
                    }

                    else
                    {
                        GetTileFromXml(gridElementCollection[x][y]);
                    }
                }

            }
        }

        private void GetParkFromXml(GridElement gridElement)
        {

            int fundIndex = rnd.Next(0, ParkGridElementPossibilitiesList.Count);

            gridElement.ImageId = ParkGridElementPossibilitiesList[fundIndex].ImageId;
            gridElement.PassableSides = ParkGridElementPossibilitiesList[fundIndex].PassableSides;
            gridElement.RandomPlacableSides = ParkGridElementPossibilitiesList[fundIndex].RandomPlacableSides;
        }

        private void GetTileFromXml(GridElement gridElement)
        {
            //Durchlaufen, und nach passable Sites durchsuchen:
            int fundIndex = 0;

            //Alle Kandidaten-Positionen der Liste in eine temporäre Liste schreiben:
            List<int> templist = new List<int>();

            for (int i = 0; i < GridElementPossibilitiesList.Count; i++)
            {
                if (GridElementPossibilitiesList[i].PassableSides == Convert.ToInt32(gridElement.ImageId))
                {
                    templist.Add(i);
                }
            }

            fundIndex = templist[rnd.Next(0, templist.Count)];

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


            //River-Eigenschaften feststellen:
            bool bRiverDirectlyAbove = false;
            bool bRiverDirectlyBeneath = false;
            bool bRiverDirectlyLeft = false;
            bool bRiverDirectlyRight = false;

            bool bBridgeAbove = false;
            bool bBridgeBeneath = false;
            bool bBridgeLeft = false;
            bool bBridgeRight = false;

            if (theSpecialMapElement == MapParser.SpecialMapElement.RiverCrossing)
            {
                if (theRiverCrossingCreator.TheRiverDirection == RiverCrossingCreator.RiverDirection.horizontal)
                {
                    if (theRiverCrossingCreator.IsInsideArea(posX, posY - 1)) { bRiverDirectlyAbove = true; }
                    if (theRiverCrossingCreator.IsInsideArea(posX, posY + 1)) { bRiverDirectlyBeneath = true; }

                    //Brücken feststellen:
                    if (bRiverDirectlyAbove)
                    { if (theRiverCrossingCreator.BridgeOnPosition(posX, RiverCrossingCreator.RiverDirection.horizontal)) { bBridgeAbove = true; } }
                    if (bRiverDirectlyBeneath)
                    { if (theRiverCrossingCreator.BridgeOnPosition(posX, RiverCrossingCreator.RiverDirection.horizontal)) { bBridgeBeneath = true; } }
                }
                if (theRiverCrossingCreator.TheRiverDirection == RiverCrossingCreator.RiverDirection.vertical)
                {
                    if (theRiverCrossingCreator.IsInsideArea(posX - 1, posY)) { bRiverDirectlyLeft = true; }
                    if (theRiverCrossingCreator.IsInsideArea(posX + 1, posY)) { bRiverDirectlyRight = true; }

                    //Brücken feststellen:
                    if (bRiverDirectlyLeft)
                    { if (theRiverCrossingCreator.BridgeOnPosition(posY, RiverCrossingCreator.RiverDirection.vertical)) { bBridgeLeft = true; } }
                    if (bRiverDirectlyRight)
                    { if (theRiverCrossingCreator.BridgeOnPosition(posY, RiverCrossingCreator.RiverDirection.vertical)) { bBridgeRight = true; } }
                }
            }


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

            if ((posX == 0 && posY == 0) || (posX == 0 && bRiverDirectlyAbove) || (posY == 0 && bRiverDirectlyLeft))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 1: Links Oben---"); }

                //if (tileWidth > 2 && tileHeight > 2)
                //{
                //    if (PlaceBuilding())
                //    {
                //        return "0"; //TODO: aus XML Laden
                //    }
                //    return "6"; //TODO: aus XML Laden
                //}

                if (bBridgeLeft)
                {
                    int[] bridgePossibleTiles = new int[] { 12, 12, 10, 14 };
                    return findRandomFromTilePossibilities(bridgePossibleTiles);                        
                }

                if (bBridgeAbove)
                {
                    int[] bridgePossibleTiles = new int[] { 5, 5, 5, 5, 3, 7 };
                    return findRandomFromTilePossibilities(bridgePossibleTiles);
                }

                int[] possibleTiles = new int[] { 0, 6 };

                return findRandomFromTilePossibilities(possibleTiles);
            }

            //Fall 2: Platzierung Oben, Mitte
            //  OXXXO
            //  OOOOO
            //  OOOOO
            //  OOOOO

            else if ((posY == 0 && posX != 0 && posX != tileWidth - 1 && !bRiverDirectlyRight) || (posX != 0 && posX != tileWidth-1 && bRiverDirectlyAbove))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 2: Obere Reihe, Mitte---"); }

                //NEU:
                //Wenn ein Gebäude links
                if (gridElement[posX - 1][posY].ImageId == "0")
                {
                    //wenn 3 Gebäude in Reihe, dann immer Straßenstück setzen
                    if (posX > 2 && posX < tileWidth - 2 && gridElement[posX - 2][posY].ImageId == "0" && gridElement[posX - 3][posY].ImageId == "0")
                    {
                        if (bBridgeAbove)
                        {
                            int[] bridgePossibleTiles = new int[] { 5, 5, 5, 5, 3, 7 };
                            return findRandomFromTilePossibilities(bridgePossibleTiles);    
                        }
                        if (B_DEBUG_ASSIGNDETAIL_ID) { Console.WriteLine("---FALL 2: TEIL1: 3 Gebäude in Reihe"); }
                        return "6";
                    }
                    else
                    {
                        if (bBridgeAbove)
                        {
                            int[] bridgePossibleTiles = new int[] { 5, 5, 5, 5, 3, 7 };
                            return findRandomFromTilePossibilities(bridgePossibleTiles);
                        }
                        if (B_DEBUG_ASSIGNDETAIL_ID) { Console.WriteLine("---FALL 2: TEIL2: Gebäude links, keine 3 in Reihe"); }
                        int[] possibleTiles = new int[] { 0, 6 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }
                }
                //Wenn links eine Straße, aber keine Verbindung nach rechts:
                else if (!CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2))
                {
                    if (bBridgeAbove)
                    {
                        int[] bridgePossibleTiles = new int[] { 5, 5, 5, 5, 5, 5, 3, 7 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }
                    if (B_DEBUG_ASSIGNDETAIL_ID) { Console.WriteLine("---FALL 2: TEIL4: Links Straße, nicht verbunden"); }
                    //Wahrscheinlichkeit 10 / 90 % Gebäude / Straße Typ 6:
                    //int percentValue = rnd.Next(0, 10);
                    //if (percentValue == 0)
                    //{ return "6"; }
                    //else
                    //{ return "0"; }
                    return "0";
                }
                //Wenn links eine Straße und Verbindung nach rechts:
                else
                {
                    //wenn links eine Verbindung nach unten vorliegt immer gerade Straße Typ 10:                    
                    if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 4))
                    {
                        if (bBridgeAbove)
                        {
                            int[] bridgePossibleTiles = new int[] {9,9,9,9,9,9,11,11,13,15};
                            return findRandomFromTilePossibilities(bridgePossibleTiles);
                        }
                        if (B_DEBUG_ASSIGNDETAIL_ID) { Console.WriteLine("---FALL 2: TEIL5: Verbindung von links, Kreuzung links"); }
                        return "10";
                    }
                    // Ansonsten 50/50 Kurve 12 oder T-Kreuzung 14:
                    else
                    {
                        if (bBridgeAbove)
                        {
                            int[] bridgePossibleTiles = new int[] { 9, 9, 9, 9, 11, 11, 13, 15 };
                            return findRandomFromTilePossibilities(bridgePossibleTiles);
                        }
                        if (B_DEBUG_ASSIGNDETAIL_ID) { Console.WriteLine("---FALL 2: TEIL5: Verbindung von links, Gerade links"); }
                        int[] possibleTiles = new int[] { 10, 10, 12, 14 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }
                }
            }

            //Fall 3: Oben Rechts
            //  OOOOX
            //  OOOOO
            //  OOOOO
            //  OOOOO
            else if ((posY == 0 && posX == tileWidth - 1) || (posX == tileWidth -1 && bRiverDirectlyAbove) || (posY == 0 && bRiverDirectlyRight))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 3: Oben Rechts---"); }
                //Wenn links keine Straßenanbindung:
                if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2) == false)
                {
                    if (bBridgeRight)
                    {
                        return "6";                        
                    }
                    if (bBridgeAbove)
                    {
                        return "5";                        
                    }
                    return "0";
                }
                if (bBridgeRight)
                {
                    int[] bridgePossibleTiles = new int[] { 10, 10, 14 };
                    return findRandomFromTilePossibilities(bridgePossibleTiles);                      
                }
                if (bBridgeAbove)
                {
                    int[] bridgePossibleTiles = new int[] { 9, 9, 13};
                    return findRandomFromTilePossibilities(bridgePossibleTiles);   
                }
                return "12";
            }


            //Fall 4: Ganz links, mittig
            //  OOOOO
            //  XOOOO
            //  XOOOO
            //  OOOOO
            else if ((posX == 0 && posY != 0 && posY != tileHeight - 1 && !bRiverDirectlyBeneath) || (bRiverDirectlyLeft && posY != 0 && posY != tileHeight-1))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 4: Ganz links, mittig---"); }

                //Wenn oben Building, dann 80% Rechtskurve, Ansonsten Building:
                if (gridElement[posX][posY - 1].ImageId == "0")
                {
                    if (bBridgeLeft)
                    {
                        return "14";    
                    }
                    int[] possibleTiles = new int[] { 0, 6, 6, 6, 6 };
                    return findRandomFromTilePossibilities(possibleTiles);
                }

                //Wenn oben Straßenanbindung, aber nur Gerade: 40% T Kreuzung 7, 30% Gerade 5, 30% Kurve 3:
                else if (gridElement[posX][posY - 1].ImageId == "5")
                {
                    if (bBridgeLeft)
                    {
                        int[] bridgePossibleTiles = new int[] { 9,9,9,11,15 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);                           
                    }
                    int[] possibleTiles = new int[] { 7, 7, 7, 7, 5, 5, 5, 3, 3, 3 };
                    return findRandomFromTilePossibilities(possibleTiles);
                }
                //Wenn oben Kurve 6 oder T-Kreuzung 7, immer Gerade 5:
                else if (gridElement[posX][posY - 1].ImageId == "6" || gridElement[posX][posY - 1].ImageId == "7" || gridElement[posX][posY - 1].ImageId == "14" || gridElement[posX][posY - 1].ImageId == "15" || gridElement[posX][posY - 1].ImageId == "12")
                {
                    if (bBridgeLeft)
                    {
                        int[] bridgePossibleTiles = new int[] { 9, 9, 9, 11, 15 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }
                    return "5";
                }

                //Wenn oben Straße, aber keine Anbindung nach unten, 90% Gebäude 10% Kurve 6:
                else if (gridElement[posX][posY - 1].ImageId == "3")
                {
                    if (bBridgeLeft)
                    {
                        int[] bridgePossibleTiles = new int[] { 12, 12, 14, 10, 10 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }

                    int[] possibleTiles = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 };
                    return findRandomFromTilePossibilities(possibleTiles);
                }
            }


            //Fall 5: Unten links
            //  OOOOO
            //  OOOOO
            //  OOOOO
            //  XOOOO
            else if ((posY == tileHeight - 1 && posX == 0) || (posX == 0 && bRiverDirectlyBeneath) || (bRiverDirectlyLeft && posY == tileHeight-1))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 5: Unten links---"); }

                //wenn von oben keine Straßenanbindung:
                if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4) == false)
                {
                    if (bBridgeBeneath)
                    {
                        return "6";                        
                    }
                    if (bBridgeLeft)
                    {
                        return "10";                        
                    }
                    return "0";

                }
                if (bBridgeLeft)
                {
                    int[] bridgePossibleTiles = new int[] { 9, 9, 9, 11 };
                    return findRandomFromTilePossibilities(bridgePossibleTiles);                        
                }
                if (bBridgeBeneath)
                {
                    int[] bridgePossibleTiles = new int[] { 5,5,5,7 };
                    return findRandomFromTilePossibilities(bridgePossibleTiles);                    
                }

                return "3";
            }

            //Fall 6: Mittig, kein RandTile
            //  OOOOO
            //  OXXXO
            //  OXXXO
            //  OOOOO
            else if (posY != 0 && posX != 0 && posY != tileHeight - 1 && posX != tileWidth - 1 && !bRiverDirectlyBeneath && !bRiverDirectlyRight)
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 6: Mittig, kein RandTile---"); }

                //SpecialElement - Überprüfung:
                bool bCentralParkDirectlyAbove = false;
                bool bCentralParkDirectlyBeneath = false;
                bool bCentralParkDirectlyLeft = false;
                bool bCentralParkDirectlyRight = false;

                if (theSpecialMapElement == MapParser.SpecialMapElement.CentralPark)
                {
                    if (theCentralParkCreator.IsInsideParkArea(posX, posY - 1)) { bCentralParkDirectlyAbove = true; }
                    if (theCentralParkCreator.IsInsideParkArea(posX, posY + 1)) { bCentralParkDirectlyBeneath = true; }
                    if (theCentralParkCreator.IsInsideParkArea(posX - 1, posY)) { bCentralParkDirectlyLeft = true; }
                    if (theCentralParkCreator.IsInsideParkArea(posX + 1, posY)) { bCentralParkDirectlyRight = true; }
                }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);
                bool buildingFromAbove = false;
                bool buildingFromLeft = false;

                if (gridElement[posX][posY - 1].ImageId == "0") { buildingFromAbove = true; }
                if (gridElement[posX - 1][posY].ImageId == "0") { buildingFromLeft = true; }

                //Hier zur behebung des Bugs Typ 9 rechts mit linker Anbindung gebaut
                if (theSpecialMapElement == MapParser.SpecialMapElement.RiverCrossing && theRiverCrossingCreator.BridgePositionsVertical != null && gridElement[posX-1][posY].ImageId == "9")
                {
                    if (!streetFromAbove)
                    {
                        int[] bridgePossibleTiles = new int[] { 0, 0, 0, 0, 0, 6 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }
                    else
                    {
                        int[] bridgePossibleTiles = new int[] { 3,7 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);                       
                    }
                }

                //Wenn links und oben Straßenanbindung 40% T-Kreuzung 11 60% Kreuzung 15
                if (streetFromAbove && streetFromLeft)
                {
                    if (bCentralParkDirectlyLeft)
                    {
                        int[] possibleTiles = new int[] { 5, 5, 5, 3, 7 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }

                    if (bCentralParkDirectlyAbove)
                    {
                        int[] possibleTiles = new int[] { 10, 10, 10, 12, 14 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }

                    //wenn oben nach rechts geht, nur Kurve 9:
                    //Wenn oben nach rechts abzweigt, keine Straße setzen, die selbst nach rechts geht:
                    if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 2))
                    {
                        return "9";
                    }

                    if (bCentralParkDirectlyRight) { return "9"; }

                    else
                    {
                        if (bCentralParkDirectlyBeneath)
                        {

                            return "11";
                        }

                        int[] possibleTiles = new int[] { 11, 11, 11, 15, 15 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }
                }

                //Wenn links und oben Gebäude, dann 80% Gebäude 20% Kurve 6:
                else if (buildingFromAbove && buildingFromLeft)
                {
                    if (bCentralParkDirectlyBeneath)
                    {

                        return "0";
                    }
                    if (bCentralParkDirectlyRight) { return "0"; }

                    int[] possibleTiles = new int[] { 0, 0, 0, 0, 6 };
                    return findRandomFromTilePossibilities(possibleTiles);
                }

                //für alle mit Gebäude oben:
                else if (buildingFromAbove && !buildingFromLeft)
                {
                    if (bCentralParkDirectlyLeft)
                    {
                        int[] possibleTiles = new int[] { 0, 6 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }

                    //Wenn links gerade Straße, dann 30% Gerade 10 40% T-Kreuzung 14 30% Kurve 12
                    if (gridElement[posX - 1][posY].ImageId == "9")
                    {
                        if (bCentralParkDirectlyBeneath)
                        {

                            return "10";
                        }
                        if (bCentralParkDirectlyRight) { return "12"; }

                        int[] possibleTiles = new int[] { 10, 10, 10, 14, 12 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }

                    //Wenn links keine Straßenanbindung, dann 10% Kurve 6 90% Gebäude
                    else if (!streetFromLeft)
                    {
                        if (bCentralParkDirectlyBeneath)
                        {

                            return "0";
                        }
                        if (bCentralParkDirectlyRight) { return "0"; }

                        int[] possibleTiles = new int[] { 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }

                    //Ansonsten 60% Gerade 10 20% T-Kreuzung 14 20% Kurve 12
                    else
                    {
                        //Wenn links eine Straße nach unten geht, dann nur 10
                        if (bCentralParkDirectlyRight) { return "12"; }

                        if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 4))
                        {
                            return "10";
                        }

                        else
                        {
                            if (bCentralParkDirectlyBeneath)
                            {

                                return "10";
                            }

                            int[] possibleTiles = new int[] { 10, 10, 10, 14, 12 };
                            return findRandomFromTilePossibilities(possibleTiles);
                        }
                    }
                }
                //für alle mit Gebäude links:
                else if (!buildingFromAbove && buildingFromLeft)
                {
                    if (bCentralParkDirectlyAbove)
                    {
                        int[] possibleTiles = new int[] { 0, 6 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }

                    //Wenn oben gerade Straße 5, dann 30% Gerade 5 40% T-Kreuzung 7 30% Kurve 3
                    if (gridElement[posX][posY - 1].ImageId == "5")
                    {
                        if (bCentralParkDirectlyBeneath)
                        {

                            return "3";
                        }
                        if (bCentralParkDirectlyRight) { return "5"; }

                        int[] possibleTiles = new int[] { 5, 5, 5, 7, 3 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }
                    //Wenn oben keine Straßenanbindung, dann 90% Gebäude 10% Kurve 6
                    else if (!streetFromAbove)
                    {
                        return "0";
                    }
                    //Ansonsten 70% Gerade 5 20% T-Kreuzung 7 10% Kurve 3
                    else
                    {
                        //Wenn oben nach rechts abzweigt, keine Straße setzen, die selbst nach rechts geht:
                        if (CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 2))
                        {
                            if (bCentralParkDirectlyBeneath)
                            {

                                return "3";
                            }
                            return "5";
                        }

                        else
                        {
                            if (bCentralParkDirectlyBeneath)
                            {

                                return "3";
                            }
                            if (bCentralParkDirectlyRight) { return "5"; }

                            int[] possibleTiles = new int[] { 5, 5, 5, 5, 5, 5, 5, 7, 3 };
                            return findRandomFromTilePossibilities(possibleTiles);
                        }
                    }
                }

                //für den Fall, dass oben und links keine Gebäude sind (oben oder unten oder beide nicht connected):
                else
                {
                    //Wenn beide nicht connected, dann Gebäude setzen 90%, 10% Kurve 6:
                    if (!streetFromAbove && !streetFromLeft)
                    {
                        //int[] possibleTiles = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 };
                        //return findRandomFromTilePossibilities(possibleTiles);                            
                        return "0";
                    }

                    //Wenn oben connected, dann 60% Kurve 3 20% T-Kreuzung 7 20% Gerade 5
                    else if (streetFromAbove)
                    {
                        if (bCentralParkDirectlyAbove && streetFromLeft)
                        {
                            int[] possibletTiles = new int[] { 10, 10, 10, 12, 14 };
                            return findRandomFromTilePossibilities(possibletTiles);
                        }
                        else if (bCentralParkDirectlyAbove)
                        {
                            int[] possibletTiles = new int[] { 0, 6 };
                            return findRandomFromTilePossibilities(possibletTiles);
                        }

                        if (bCentralParkDirectlyBeneath)
                        {

                            return "3";
                        }
                        if (bCentralParkDirectlyRight) { return "5"; }

                        int[] possibleTiles = new int[] { 3, 7, 5 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }
                    //Wenn links connected dann 60% Kurve 12 20% T-Kreuzung 14 20% Gerade 10
                    else
                    {
                        if (bCentralParkDirectlyLeft)
                        {
                            int[] ppossibleTiles = new int[] { 3, 7, 5 };
                            return findRandomFromTilePossibilities(ppossibleTiles);
                        }

                        if (bCentralParkDirectlyBeneath)
                        {

                            return "10";
                        }
                        if (bCentralParkDirectlyRight) { return "12"; }

                        int[] possibleTiles = new int[] { 12, 12, 12, 14, 10, 10, 10 };
                        return findRandomFromTilePossibilities(possibleTiles);
                    }
                }
            }

            //Fall 7: Rechts, mittig
            //  OOOOO
            //  OOOOX
            //  OOOOX
            //  OOOOO
            else if ((posY != 0 && posX == tileWidth - 1 && posY != tileHeight - 1 && !bRiverDirectlyBeneath) || (bRiverDirectlyRight && posX != 0 && posX != tileWidth -1 && posY != 0 && posY != tileHeight -1))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 7: Rechts, mittig---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                if (streetFromAbove == false && streetFromLeft == false)
                {
                    if (bBridgeRight)
                    {
                        return "6";                        
                    }
                    //Console.WriteLine("Type: oben nicht links nicht");
                    return "0";
                }

                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    //Console.WriteLine("Type: oben links");
                    if (bBridgeRight)
                    {
                        int[] bridgePossibleTiles = new int[] { 11, 11, 11, 15 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);                        
                    }

                    int[] possibleTiles = new int[] { 9, 13 };
                    return findRandomFromTilePossibilities(possibleTiles);
                }

                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    //Console.WriteLine("Type: oben nicht links");
                    if (bBridgeRight)
                    {
                        int[] bridgePossibleTiles = new int[] { 10, 10, 10, 14 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }

                    return "12";
                }
                else if (streetFromLeft == false && streetFromAbove == true)
                {
                    if (bBridgeRight)
                    {
                        int[] bridgePossibleTiles = new int[] { 3, 3, 3, 7 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }
                    //Console.WriteLine("Type: oben links nicht");
                    return "5";
                }
            }

            //Fall 8: Unten, mittig
            //  OOOOO
            //  OOOOO
            //  OOOOO
            //  OXXXO
            else if ((posX != 0 && posY == tileHeight - 1 && posX != tileWidth - 1 && !bRiverDirectlyRight) || (bRiverDirectlyBeneath && posX != 0 && posX != tileWidth -1))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 8: Unten, mittig---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                //links und oben keine Anbindung:
                if (streetFromAbove == false && streetFromLeft == false)
                {
                    if (bBridgeBeneath)
                    {
                        return "6";                        
                    }
                    //Console.WriteLine("Type: oben nicht links nicht");
                    return "0";
                }
                //links und oben straßenanbindung:
                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    if (bBridgeBeneath)
                    {
                        int[] bridgePossibleTiles = new int[] { 13,13,13,15 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }

                    int[] possibleTiles = new int[] { 9, 9, 9, 9, 11 };

                    return findRandomFromTilePossibilities(possibleTiles);
                }
                //links anbindung, oben nicht
                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    if (bBridgeBeneath)
                    {
                        int[] bridgePossibleTiles = new int[] { 12, 12, 12, 14 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }
                    return "10";
                }
                //oben anbindung, links nicht
                else if (streetFromLeft == false && streetFromAbove == true)
                {
                    if (bBridgeBeneath)
                    {
                        int[] bridgePossibleTiles = new int[] { 5, 5, 5, 7 };
                        return findRandomFromTilePossibilities(bridgePossibleTiles);
                    }

                    return "3";
                }
            }


            //Fall 9: Unten, rechts
            //  OOOOO
            //  OOOOO
            //  OOOOO
            //  OOOOX
            else if ((posY == tileHeight - 1 && posX == tileWidth - 1) || (bRiverDirectlyBeneath && posX == tileWidth - 1) || (bRiverDirectlyRight && posY == tileHeight - 1))
            {
                if (B_DEBUG_ASSIGN_ID) { Console.WriteLine("---FALL 9: Unten, rechts---"); }

                bool streetFromAbove = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX][posY - 1].ImageId), 4);
                bool streetFromLeft = CheckIfBitIsIncluded(Convert.ToInt32(gridElement[posX - 1][posY].ImageId), 2);

                if (streetFromAbove == false && streetFromLeft == false)
                {
                    if (bBridgeBeneath)
                    {
                        bRiverAssignmentValid = false;                        
                    }
                    if (bBridgeRight)
                    {
                        bRiverAssignmentValid = false;                        
                    }

                    return "0";
                }

                else if (streetFromLeft == true && streetFromAbove == true)
                {
                    if (bBridgeBeneath)
                    {
                        return "13";                        
                    }

                    if (bBridgeRight)
                    {
                        return "11";                        
                    }

                    return "9";
                }

                else if (streetFromLeft == true && streetFromAbove == false)
                {
                    if (bBridgeBeneath)
                    {
                        return "12";                        
                    }
                    if (bBridgeRight)
                    {
                        return "10";                        
                    }

                    if (bRiverDirectlyRight)
                    {
                        bRiverAssignmentValid = false;                        
                    }
                    if (bRiverDirectlyBeneath)
                    {
                        bRiverAssignmentValid = false;                        
                    }

                    return "10";
                }
                else if (streetFromLeft == false && streetFromAbove == true)
                {
                    if (bBridgeBeneath)
                    {
                        return "5";                        
                    }
                    if (bBridgeRight)
                    {
                        return "3";                        
                    }

                    if (bRiverDirectlyRight)
                    {
                        bRiverAssignmentValid = false;                        
                    }

                    if (bRiverDirectlyBeneath)
                    {
                        bRiverAssignmentValid = false;                        
                    }

                    return "3";
                }
            }

            return Convert.ToString(returnvalue); //Default-Return
        }

        private GridElement GetTileLeft(GridElement[][] gridElement, int posX, int posY)
        {
            return gridElement[posX - 1][posY];
        }


        /***************************************************************************************
         *      Hilfsfunktionen
         **************************************************************************************/
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
            int lengthX = tileCountX;
            int lengthY = tileCountY;

            bool bUpperElementConnected = false;
            bool bLeftElementConnected = false;

            if (!bRiverAssignmentValid)
            {
                bRiverAssignmentValid = true;
                return false;
            }

            //GridElement an Position über unterem Eckelement rechts auf Anbindung nach unten überprüfen.
            if (CheckIfBitIsIncluded(Convert.ToInt32(gridElementCollectionTemp[lengthX - 1][lengthY - 2].ImageId), 4))
            {bUpperElementConnected = true;}

            //GridElement an Position links vom unterem Eckelement rechts auf Anbindung nach unten überprüfen.
            if (CheckIfBitIsIncluded(Convert.ToInt32(gridElementCollectionTemp[lengthX - 2][lengthY - 1].ImageId), 2))
            {bLeftElementConnected = true;}

            //Zuweisung korrekt, wenn die Booleans entweder beide true oder beide false (Straßennetz geschlossen):
            if ((bLeftElementConnected && bUpperElementConnected) || (bLeftElementConnected && bUpperElementConnected))
            {return true;}

            //Wenn die beiden Booleans unterschiedlich sind, mit 'false' raus (Zuweisung fehlerhaft):
            return false;
        }

        public bool CheckIfBitIsIncluded(int sourceValue, int bitValue)
        {
            int startPotency = 1;
            while (sourceValue > startPotency)
            {startPotency *= 2;}
            startPotency /= 2;

            while (startPotency > 0)
            {
                if (sourceValue >= startPotency)
                {                    
                    if (startPotency == bitValue)
                    {return true;}
                    sourceValue -= startPotency;
                }
                startPotency /= 2;

                if (startPotency == 1)
                { startPotency = 0; }
            }
            return false;
        }

        private string findRandomFromTilePossibilities(int[] possibleTiles)
        {
            int value = rnd.Next(0, possibleTiles.Length);
            return Convert.ToString(possibleTiles[value]);
        }

        private List<GridElement> GetListFromXML(string filename)
        {
            List<GridElement> TempList = new List<GridElement>();

            XmlSerializer ser = new XmlSerializer(typeof(List<GridElement>));
            StreamReader sr = new StreamReader(filename);

            TempList = (List<GridElement>)ser.Deserialize(sr);
            sr.Close();

            return TempList;
        }
    }
}