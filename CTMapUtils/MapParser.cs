using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Imaging;

namespace CTMapUtils
{
    public class MapParser
    {
        public enum SpecialMapElement
        {
            None,
            CentralPark,
            RiverCrossing
        }

        private static Dictionary<String, Bitmap> imageCache = new Dictionary<string, Bitmap>();
        private static string imagePath = "C:\\Tiles\\";
        private static Size bitmapSize;

        public static void Initialize(int gridElementWidth,int gridElementHeight){
            bitmapSize = new Size(gridElementWidth, gridElementHeight);
        }

        public static string ImagePath
        {
            get
            {
                if (imagePath == null)
                {
                    imagePath = "C:\\Tiles\\";
                }

                if (!imagePath.EndsWith("\\"))
                    imagePath += "\\";

                return imagePath;
            }
            set
            {
                imagePath = value;
            }
        }

        public static Map Load(int width, int height, SpecialMapElement specialMapElement)
        {
            RandomMap rndMap = new RandomMap();
            Map returnvalue = rndMap.GenerateRandomMap(width, height, specialMapElement);

            return returnvalue;
        }

        public static Map Load(string path)
        {
            Map returnvalue = null;

            Grid grid = null;
            try
            {
                using (var fileStream = System.IO.File.Open(path, System.IO.FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(Grid));
                    grid = (Grid)serializer.Deserialize(fileStream);
                }

                returnvalue = new Map(grid);
            }
            catch (Exception)
            {
            }

            return returnvalue;
        }

        public static Image GetImageById(string imageId)
        {
            
            Bitmap returnvalue = null;

            try
            {
                var path = ImagePath + imageId + ".";
                if (!imageCache.TryGetValue(path, out returnvalue))
                {
                    if (File.Exists(path + "jpg"))
                        returnvalue = (Bitmap)CT_Helper.resizeImage(Image.FromFile(path + "jpg", true), bitmapSize);
                    else if (File.Exists(path + "jpeg"))
                        returnvalue = (Bitmap)CT_Helper.resizeImage(Image.FromFile(path + "jpeg", true), bitmapSize);

                    imageCache.Add(path, returnvalue);
                }
            }
            catch (Exception)
            {
            }

            return returnvalue;
        }

        private static void temp()
        {
            //Grid gr = new Grid();
            //gr.Height = 500;
            //gr.Width = 500;
            //gr.GridElementCollection = new GridElement[3][];
            //for (var i = 0; i < gr.GridElementCollection.Length; i++)
            //{
            //    gr.GridElementCollection[i] = new GridElement[3];
            //    var column = gr.GridElementCollection[i];

            //    for (var rowIndex = 0; rowIndex < column.Length; rowIndex++)
            //    {
            //        column[rowIndex] = new GridElement();
            //        column[rowIndex].ImageId = 0 + "";
            //        column[rowIndex].ElementPassable = true;
            //    }
            //}

            //XmlSerializer serializer = new XmlSerializer(typeof(Grid));
            //using (var fileStream = System.IO.File.Open("C:\\test.xml", System.IO.FileMode.OpenOrCreate))
            //{
            //    serializer.Serialize(fileStream, gr);
            //}
        }
    }
}
