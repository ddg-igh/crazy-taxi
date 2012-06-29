using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace CTMapUtils
{
    public partial class Map : UserControl
    {
        private Grid MapGrid;

        //Höhe und Breite eines einzelnen Gridelementes
        private int gridElementWidth;
        private int gridElementHeight;

        private Random rnd = new Random();

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
            Dictionary<String,Bitmap> cache=new Dictionary<string,Bitmap>();
            Bitmap returnvalue = new Bitmap(size.Width, size.Height,PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(returnvalue);

            for (var x = 0; x < MapGrid.GridElementCollection.Length; x++)
            {
                var row = MapGrid.GridElementCollection[x];
                for (var y = 0; y < row.Length; y++)
                {
                    Bitmap image;
                    var gridElement = row[y];

                    var elementWidth = (int)(Math.Ceiling((MapGrid.Width / row.Length) * getScaleFactorWidth(size.Width)));
                    var elementHeight = (int)(Math.Ceiling((MapGrid.Height / MapGrid.GridElementCollection.Length) * getScaleFactorHeight(size.Height)));

                    
                    if (!cache.TryGetValue(gridElement.ImageId,out image)){
                        image= (Bitmap)MapParser.GetImageById(gridElement.ImageId);
                        image=(Bitmap)CT_Helper.resizeImage(image,new Size(elementWidth,elementHeight));
                        cache.Add(gridElement.ImageId, image);
                    }


                    //image = (Bitmap)image.GetThumbnailImage(elementWidth, elementHeight, null, IntPtr.Zero);

                    g.DrawImageUnscaled(image, elementWidth * x, elementHeight * y);
                 /*   for (var bx = 0; bx < elementWidth; bx++)
                    {
                        for (var by = 0; by < elementHeight; by++)
                        {
                            var cl = image.GetPixel(bx, by);
                            returnvalue.SetPixel(bx + (elementWidth * x), by + (elementHeight * y), cl);
                        }
                    }  */
                }
            }

            return returnvalue;
        }

        public void Draw(Graphics g,Size screenSize,int x, int y)
        {
            int elementX = (int)Math.Floor(Math.Abs((double)(x / gridElementWidth)));
            int elementY = (int)Math.Floor(Math.Abs((double)(y / gridElementHeight)));
            int endElementX = elementX + (int)Math.Floor((double)(screenSize.Width / gridElementWidth));
            int endElementY = elementY + (int)Math.Floor((double)(screenSize.Height / gridElementHeight));

            int drawPositionX = x + elementX * gridElementWidth;
            int drawPositionY = y + elementY * gridElementHeight;

            if (endElementX >= MapGrid.GridElementCollection.Length-1)
            {
                endElementX = MapGrid.GridElementCollection.Length-1;
            }

            for (int amountX = 0; amountX + elementX <= endElementX; amountX++)
            {
                var row = MapGrid.GridElementCollection[amountX + elementX];
               
                if (endElementY >= row.Length-1)
                {
                    endElementY = row.Length-1;
                }


                for (var amountY =0; amountY+elementY <= endElementY; amountY++)
                {
                    var gridElement = row[amountY + elementY];           

                    var image = (Bitmap)MapParser.GetImageById(gridElement.ImageId);

                    //image = (Bitmap)image.GetThumbnailImage(elementWidth, elementHeight, null, IntPtr.Zero);

                    g.DrawImageUnscaled(image, drawPositionX + amountX * gridElementWidth, drawPositionY + amountY * gridElementHeight);
                    /*   for (var bx = 0; bx < elementWidth; bx++)
                       {
                           for (var by = 0; by < elementHeight; by++)
                           {
                               var cl = image.GetPixel(bx, by);
                               returnvalue.SetPixel(bx + (elementWidth * x), by + (elementHeight * y), cl);
                           }
                       }  */
                }
            }
        }

      

        /// <summary>
        /// Erstellt die Boxen mit Bildern
        /// </summary>
        /// <param name="uiSize">Die Größe, in der die Map angezeigt werden soll</param>
        public void Initialize(Size uiSize)
        {
            this.Size = uiSize;

            //x=breitengrad (x-achse)
            elementBoxes = new PictureBox[MapGrid.GridElementCollection.Length][];
            for (var x = 0; x < MapGrid.GridElementCollection.Length; x++)
            {
                elementBoxes[x] = new PictureBox[MapGrid.GridElementCollection[x].Length];
                var column = MapGrid.GridElementCollection[x];
                for (var y = 0; y < column.Length; y++)
                {
                    var gridElement = column[y];
                    var pictureBox = new System.Windows.Forms.PictureBox();
                    //pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                    gridElementWidth = (int)Math.Ceiling((MapGrid.Width / MapGrid.GridElementCollection.Length) * ScaleFactorWidth);
                    gridElementHeight = (int)Math.Ceiling((MapGrid.Height / column.Length) * ScaleFactorHeight);

                    MapParser.Initialize(gridElementWidth, gridElementHeight);

                    pictureBox.Width = (int)gridElementWidth;
                    pictureBox.Height = (int)gridElementHeight;
                    var image = MapParser.GetImageById(gridElement.ImageId);
                    pictureBox.Image = image;
                    //pictureBox.BorderStyle = BorderStyle.FixedSingle;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                    var boxX = gridElementWidth * x;
                    var boxY = gridElementHeight * y;

                    pictureBox.Left = (int)boxX;
                    pictureBox.Top = (int)boxY;

                    elementBoxes[x][y] = pictureBox;

                    //Auskommentiert, um die Testausgaben der Kollision zu sehen
                    //this.Controls.Add(elementBoxes[x][y]);
                }
            }

            Collision = new CollisionManager(MapGrid, gridElementWidth, gridElementHeight);
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


        //public Point GetStartPoint(Map _Map)

        public Point GetRandomTilePosition(bool _bIsPassable, System.Drawing.Size size)
        {
            Point returnPoint = new Point();

            int basePointX = 0;
            int basePointY = 0;

            while (true)
            {
                int posX = rnd.Next(0,MapGrid.GridElementCollection.Length);
                int posY = rnd.Next(0, MapGrid.GridElementCollection[0].Length);

                if (_bIsPassable)
                {
                    if (MapGrid.GridElementCollection[posX][posY].PassableSides != 0)
                    {
                        basePointX = posX;
                        basePointY = posY;
                        break;
                    }
                }

                else
                {                  
                        basePointX = posX;
                        basePointY = posY;
                        break;
                }
            }



            int worldPosX = basePointX * gridElementWidth + Convert.ToInt32(gridElementWidth*0.5);
            int worldPosY = basePointY * gridElementHeight + Convert.ToInt32(gridElementHeight*0.5);

            returnPoint = new Point(worldPosX, worldPosY);

            return returnPoint;
        } 
    }
}
