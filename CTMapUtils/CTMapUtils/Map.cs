using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CTMapUtils
{
    public partial class Map : UserControl
    {
        private Grid MapGrid;

        //Höhe und Breite eines einzelnen Gridelementes
        private int gridElementWidth;
        private int gridElementHeight;

        public CollisionManager Collision
        {
            get;
            private set;
        }

        public double ScaleFactorWidth
        {
            get
            {
                return getScaleFactorWidth(this.Size.Width);
            }
        }

        public double ScaleFactorHeight
        {
            get
            {
                return getScaleFactorHeight(this.Size.Height);
            }
        }

        private double getScaleFactorWidth(double myWidth)
        {
            double uiWidth = myWidth;
            double gridWidth = MapGrid.Width;

            return uiWidth / gridWidth;
        }

        private double getScaleFactorHeight(double myHeight)
        {
            double uiHeight = myHeight;
            double gridHeight = MapGrid.Height;

            return uiHeight / gridHeight;
        }

        //Hält die Steuerelemente auf diesem UserControl
        private PictureBox[][] elementBoxes = null;

        internal Map(Grid mapGrid)
        {
            InitializeComponent();

            MapGrid = mapGrid;
        }

        public Image DrawImage(Size size)
        {
            Bitmap returnvalue = new Bitmap(size.Width, size.Height);

            var elementWidth = (int)((MapGrid.Width / MapGrid.GridElementCollection.Length) * getScaleFactorWidth(size.Width));
            var elementHeight = (int)((MapGrid.Height / MapGrid.GridElementCollection[0].Length) * getScaleFactorHeight(size.Height));

            for (var x = 0; x < MapGrid.GridElementCollection.Length; x++)
            {
                var column = MapGrid.GridElementCollection[x];
                for (var y = 0; y < column.Length; y++)
                {
                    var gridElement = column[y];

                    var image = (Bitmap)MapParser.GetImageById(gridElement.ImageId);

                    image = (Bitmap)image.GetThumbnailImage(elementWidth, elementHeight, null, IntPtr.Zero);
                    for (var bx = 0; bx < elementWidth; bx++)
                    {
                        for (var by = 0; by < elementHeight; by++)
                        {
                            var cl = image.GetPixel(bx, by);
                            returnvalue.SetPixel(bx + (elementWidth * x), by + (elementHeight * y), cl);
                        }
                    }
                }
            }

            return returnvalue;
        }

        /// <summary>
        /// Erstellt die Boxen mit Bildern
        /// </summary>
        /// <param name="uiSize">Die Größe, in der die Map angezeigt werden soll</param>
        public void Initialize()
        {
            //this.Size = uiSize;


            gridElementWidth = (int)((MapGrid.Width / MapGrid.GridElementCollection.Length) * ScaleFactorWidth);
            gridElementHeight = (int)((MapGrid.Height / MapGrid.GridElementCollection[0].Length) * ScaleFactorHeight);
            //x=breitengrad (x-achse)
            //elementBoxes = new PictureBox[MapGrid.GridElementCollection.Length][];
            //for (var x = 0; x < MapGrid.GridElementCollection.Length; x++)
            //{
            //    elementBoxes[x] = new PictureBox[MapGrid.GridElementCollection[x].Length];
            //    var column = MapGrid.GridElementCollection[x];
            //    for (var y = 0; y < column.Length; y++)
            //    {
            //        var gridElement = column[y];
            //        var pictureBox = new System.Windows.Forms.PictureBox();
            //        //pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

            //        gridElementWidth = (int)((MapGrid.Width / MapGrid.GridElementCollection.Length) * ScaleFactorWidth);
            //        gridElementHeight = (int)((MapGrid.Height / column.Length) * ScaleFactorHeight);

            //        pictureBox.Width = (int)gridElementWidth;
            //        pictureBox.Height = (int)gridElementHeight;
            //        var image = MapParser.GetImageById(gridElement.ImageId);
            //        pictureBox.Image = image;
            //        //pictureBox.BorderStyle = BorderStyle.FixedSingle;
            //        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            //        var boxX = gridElementWidth * x;
            //        var boxY = gridElementHeight * y;

            //        pictureBox.Left = (int)boxX;
            //        pictureBox.Top = (int)boxY;

            //        elementBoxes[x][y] = pictureBox;

            //        //Auskommentiert, um die Testausgaben der Kollision zu sehen
            //        this.Controls.Add(elementBoxes[x][y]);
            //    }
            //}

            Collision = new CollisionManager(MapGrid, gridElementWidth, gridElementHeight/*, this*/);
        }

        /// <summary>
        /// Immer, wenn die das UserControl größer oder kleiner wird, müssen alle Bilder skaliert und umpositioniert werden, damit die anzeige passt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_Resize(object sender, EventArgs e)
        {
            if (elementBoxes != null)
            {
                for (var x = 0; x < MapGrid.GridElementCollection.Length; x++)
                {
                    var column = MapGrid.GridElementCollection[x];
                    for (var y = 0; y < column.Length; y++)
                    {
                        gridElementWidth = (int)((MapGrid.Width / MapGrid.GridElementCollection.Length) * ScaleFactorWidth);
                        gridElementHeight = (int)(((double)MapGrid.Height / (double)column.Length) * (double)ScaleFactorHeight);

                        var pictureBox = elementBoxes[x][y];

                        pictureBox.Width = (int)gridElementWidth;
                        pictureBox.Height = (int)gridElementHeight;

                        var boxX = gridElementWidth * x;
                        var boxY = gridElementHeight * y;

                        pictureBox.Left = (int)boxX;
                        pictureBox.Top = (int)boxY;

                        Collision.updateElementDimensions(gridElementWidth, gridElementHeight);
                    }
                }
            }
        }


        private enum FrontBackDirections
        {
            Left,
            Right
        }
        private enum LeftRightDirections
        {
            Up,
            Down
        }
        private System.Windows.Vector getFrontBackVector(Point carPosition, Size carSize, FrontBackDirections facing)
        {
            return new System.Windows.Vector((facing == FrontBackDirections.Left ? -carSize.Width : carSize.Width), 0);
        }

        private System.Windows.Vector getLeftRightVector(Point carPosition, Size carSize, LeftRightDirections facing)
        {
            return new System.Windows.Vector(0, (facing == LeftRightDirections.Down ? carSize.Height : -carSize.Height));
        }
        ///// <summary>
        ///// Debugmethode: zeichnet eine Linie auf das UserControl
        ///// </summary>
        ///// <param name="l"></param>
        ///// <param name="c"></param>
        //private void drawLine(Line l, Color c)
        //{
        //    var points = l.AllPoints;
        //    for (var i = 0; i < points.Count; i++)
        //    {
        //        var point = points[i];
        //        //point.X += 50;
        //        //point.Y += 50;
        //        Controls.Add(new Label() { BackColor = c, Text = "", Location = point, Width = 5, Height = 5 });
        //    }
        //}

        //ToDo: optimieren, in Line statt mit Point mit Vector arbeiten

        //public CollisionResult CheckCollision(Point carPosition, Size carSize, double rotationDegrees, bool drivingForward)
        //{
        //    var st = DateTime.Now;
        //    //Die Bewegungsrichtung abhängig von der Rotation
        //    System.Windows.Vector corner;
        //    System.Windows.Vector middle = new System.Windows.Vector(carPosition.X, carPosition.Y);

        //    //Auf welchem Gridelement ist der Mittelpunkt des Autos (=dieses element muss nicht kontrolliert werden
        //    var carGridX = (int)(carPosition.X / this.gridElementWidth);
        //    var carGridY = (int)(carPosition.Y / this.gridElementHeight);
        //    var ctrl1 = new Label() { BackColor = Color.Blue, Location = carPosition, Width = 5, Height = 5 };
        //    Controls.Add(ctrl1);
        //    ctrl1.BringToFront();

        //    //Welche Seiten des Autos müssen kontrolliert werden?
        //    var sides = new Line[3];

        //    if (drivingForward)
        //    {
        //        //Ecke oben rechts
        //        corner = new System.Windows.Vector(carPosition.X + (carSize.Width / 2), carPosition.Y - (carSize.Height / 2));
        //        corner -= middle;
        //        corner = rotateVector(corner, rotationDegrees);
        //        corner += middle;


        //        //Obere Seite, Vektor zeigt nach rechts
        //        sides[0] = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), rotateVector(getFrontBackVector(carPosition, carSize, FrontBackDirections.Left), rotationDegrees));
        //        //Rechte Seite, Vektor zeigt nach oben
        //        sides[1] = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Down), rotationDegrees));


        //        //Ecke oben links
        //        corner = new System.Windows.Vector(carPosition.X - (carSize.Width / 2), carPosition.Y - (carSize.Height / 2));
        //        corner -= middle;
        //        corner = rotateVector(corner, rotationDegrees);
        //        corner += middle;

        //        //Linke Seite, Vektor zeigt nach oben
        //        sides[2] = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Down), rotationDegrees));
        //    }
        //    else
        //    {
        //        //Ecke unten rechts
        //        corner = new System.Windows.Vector(carPosition.X + (carSize.Width / 2), carPosition.Y + (carSize.Height / 2));
        //        corner -= middle;
        //        corner = rotateVector(corner, rotationDegrees);
        //        corner += middle;


        //        //Obere Seite, Vektor zeigt nach rechts
        //        sides[0] = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), rotateVector(getFrontBackVector(carPosition, carSize, FrontBackDirections.Left), rotationDegrees));
        //        //Rechte Seite, Vektor zeigt nach oben
        //        sides[1] = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Up), rotationDegrees));


        //        //Ecke unten links
        //        corner = new System.Windows.Vector(carPosition.X - (carSize.Width / 2), carPosition.Y + (carSize.Height / 2));
        //        corner -= middle;
        //        corner = rotateVector(corner, rotationDegrees);
        //        corner += middle;

        //        //Linke Seite, Vektor zeigt nach oben
        //        sides[2] = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Up), rotationDegrees));
        //    }

        //    var returnvalue = new CollisionResult();

        //    var currentCarElement = this.MapGrid.GridElementCollection[carGridX][carGridY];
        //    for (int i1 = 0; i1 < 3; i1++)
        //    {
        //        List<Point> points = sides[i1].AllPoints;
        //        for (var i = 0; i < points.Count; i++)
        //        {
        //            var point = points[i];

        //            var px = (point.X / gridElementWidth);
        //            var py = (point.Y / gridElementHeight);
        //            var ctrl = new Label() { BackColor = Color.Green, Text = "", Location = point, Width = 5, Height = 5 };
        //            if (px != carGridX || py != carGridY)
        //            {
        //                if (MapGrid.GridElementCollection[px][py].PassableSides == 0)
        //                {
        //                    if (i1 == 0)
        //                    {
        //                        if (drivingForward)
        //                            returnvalue.Front = true;
        //                        else
        //                            returnvalue.Back = true;
        //                    }
        //                    else if (i1 == 1)
        //                    {
        //                        returnvalue.Right = true;
        //                    }
        //                    else
        //                        returnvalue.Left = true;

        //                    ctrl.BackColor = Color.Red;
        //                }
        //            }
        //            this.Controls.Add(ctrl);
        //            ctrl.BringToFront();
        //        }
        //    }

        //    var diff = DateTime.Now - st;
        //    MessageBox.Show(diff.TotalMilliseconds + "");
        //    return returnvalue;
        //}


        //ToDo: Start und Endzeit entfernen, Zeichnen der Punkte entfernen
        /// <summary>
        /// </summary>
        /// <param name="carPosition">Position (Mittelpunkt) des Autos</param>
        /// <param name="carSize">Größe des Autos</param>
        /// <param name="rotationDegrees">Rotation des Rechtecks (0 = Auto ist nach oben gerichtet, 90 = Auto ist nach rechts gerichtet, ...</param>
        /// <param name="movementDegrees">Richtung, in die das Auto sich bewegt (Auf der Form, nicht abhängig von der Rotation => Bei 0 bewegt sich das Auto nach norden, egal wie es rotiert ist)</param>
        /// <returns></returns>
        /*public CollisionResult CheckCollision(Point carPosition, Size carSize, double rotationDegrees, double movementDegrees)
        {
            var st = DateTime.Now;
            //Die Bewegungsrichtung abhängig von der Rotation
            var relativeMovement = Math.Abs((rotationDegrees - 360) - movementDegrees) % 360;
            System.Windows.Vector corner;
            System.Windows.Vector middle = new System.Windows.Vector(carPosition.X, carPosition.Y);

            //Auf welchem Gridelement ist der Mittelpunkt des Autos (=dieses element muss nicht kontrolliert werden
            var carGridX = (int)(carPosition.X / this.gridElementWidth);
            var carGridY = (int)(carPosition.Y / this.gridElementHeight);
            var ctrl1 = new Label() { BackColor = Color.Blue, Location = carPosition, Width = 5, Height = 5 };
            Controls.Add(ctrl1);
            ctrl1.BringToFront();

            //Welche Seiten des Autos müssen kontrolliert werden?
            var sides = new System.Windows.Vector[2];

            if (relativeMovement < 270)
            {
                //Das Auto rutscht NICHT nach links oder oben links
                if (relativeMovement < 180)
                {
                    //Das Auto rutscht NICHT nach unten oder unten links
                    if (relativeMovement < 90)
                    {
                        //Das Auto rutscht NICHT nach rechts oder unten rechts
                        //=> Das Auto bewegt sich auf jeden Fall nach oben
                        corner = new System.Windows.Vector(carPosition.X + (carSize.Width / 2), carPosition.Y - (carSize.Height / 2));
                        corner -= middle;
                        corner = rotateVector(corner, rotationDegrees);
                        corner += middle;

                        //Obere Seite, Vektor zeigt nach links
                        sides[0] = rotateVector(getFrontBackVector(carPosition, carSize, FrontBackDirections.Left), rotationDegrees);

                        //Bewegt sich das Auto außerdem nach rechts?
                        if (relativeMovement != 0)
                        {
                            //Das Auto rutscht nach oben rechts

                            //Rechte Seite, Vektor zeigt nach unten
                            sides[1] = rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Down), rotationDegrees);
                        }
                    }
                    else
                    {
                        //Das Auto rutscht nach rechts und evtl nach unten
                        //Ecke unten rechts
                        corner = new System.Windows.Vector(carPosition.X + (carSize.Width / 2), carPosition.Y + (carSize.Height / 2));
                        corner -= middle;
                        corner = rotateVector(corner, rotationDegrees);
                        corner += middle;

                        //Rechte Seite, Vektor zeigt nach oben
                        sides[0] = rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Up), rotationDegrees);
                        if (relativeMovement > 90)
                        {
                            //Das Auto rutscht nach unten rechts

                            //Untere Seite, Vektor zeigt nach links
                            sides[1] = rotateVector(getFrontBackVector(carPosition, carSize, FrontBackDirections.Left), rotationDegrees);
                        }
                    }
                }
                else
                {
                    //Das Auto bewegt sich nach unten und eventuell nach links
                    //Ecke unten links
                    corner = new System.Windows.Vector(carPosition.X - (carSize.Width / 2), carPosition.Y + (carSize.Height / 2));
                    corner -= middle;
                    corner = rotateVector(corner, rotationDegrees);
                    corner += middle;

                    //Untere Seite, Vektor zeigt nach rechts
                    sides[0] = rotateVector(getFrontBackVector(carPosition, carSize, FrontBackDirections.Right), rotationDegrees);
                    if (relativeMovement > 180)
                    {
                        //Das Auto rutscht nach unten links
                        //linke Seite, Vektor zeigt nach oben
                        sides[1] = rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Up), rotationDegrees);
                    }
                }
            }
            else
            {
                //Das Auto rutscht nach links und evtl nach oben
                //Ecke oben links
                corner = new System.Windows.Vector(carPosition.X - (carSize.Width / 2), carPosition.Y - (carSize.Height / 2));
                corner -= middle;
                corner = rotateVector(corner, rotationDegrees);
                corner += middle;

                //Linke Seite, Vektor zeigt nach unten
                sides[0] = rotateVector(getLeftRightVector(carPosition, carSize, LeftRightDirections.Down), rotationDegrees);
                if (relativeMovement > 270)
                {
                    //Das Auto rutscht nach oben links
                    //Obere Seite, Vektor zeigt nach rechts
                    sides[1] = rotateVector(getFrontBackVector(carPosition, carSize, FrontBackDirections.Right), rotationDegrees);
                }
            }

            var currentCarElement = this.MapGrid.GridElementCollection[carGridX][carGridY];
            //MessageBox.Show("Y: " + y + ", X: " + x);
            if (sides[0] != null)
            {
                var line = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), sides[0]);
                List<Point> points = line.AllPoints;
                for (var i = 0; i < points.Count; i++)
                {
                    var point = points[i];

                    var px = (point.X / gridElementWidth);
                    var py = (point.Y / gridElementHeight);
                    var ctrl = new Label() { BackColor = Color.Green, Text = "", Location = point, Width = 5, Height = 5 };
                    if (px != carGridX || py != carGridY)
                    {
                        if (MapGrid.GridElementCollection[px][py].PassableSides == 0)
                            ctrl.BackColor = Color.Red;
                    }
                    this.Controls.Add(ctrl);
                    ctrl.BringToFront();
                }

                if (sides[1] != null)
                {
                    line = Line.FromCarSide(new Point((int)corner.X, (int)corner.Y), sides[1]);
                    points = line.AllPoints;
                    for (var i = 0; i < points.Count; i++)
                    {
                        var point = points[i];

                        var px =(point.X / gridElementWidth);
                        var py = (point.Y / gridElementHeight);
                        var ctrl = new Label() { BackColor = Color.Green, Text = "", Location = point, Width = 5, Height = 5 };
                        if (px != carGridX || py != carGridY)
                        {
                            if (MapGrid.GridElementCollection[px][py].PassableSides == 0)
                                ctrl.BackColor = Color.Red;
                        }
                        this.Controls.Add(ctrl);
                        ctrl.BringToFront();
                    }
                }
            }

            var diff = DateTime.Now - st;
            return null;
        }*/
    }
}
