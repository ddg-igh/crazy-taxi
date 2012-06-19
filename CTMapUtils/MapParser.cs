using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Xml.Serialization;
using System.IO;

namespace CTMapUtils
{
    public class MapParser
    {
        private static Dictionary<String, Image> cache = new Dictionary<string, Image>();
        private static string imagePath = "C:\\Tiles\\";
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

        public static Map Load(int width, int height)
        {
            RandomMap rndMap = new RandomMap();
            Map returnvalue = rndMap.GenerateRandomMap(width, height);

            return returnvalue;

            //List<GridElement> list = new List<GridElement>();

            //for (var i = 0; i < 16; i++)
            //{
            //    list.Add(new GridElement() { ImageId = i + "", PassableSides = i, RandomPlacableSides = i });
            //}

            //Grid grid = null;
            //try
            //{
            //    using (var fileStream = System.IO.File.Open(System.IO.Directory.GetCurrentDirectory() + "\\ElementList.xml", System.IO.FileMode.OpenOrCreate))
            //    {
            //        var serializer = new XmlSerializer(typeof(List<GridElement>));
            //        serializer.Serialize(fileStream, list);
            //    }

            //    //returnvalue = new Map(grid);
            //}
            //catch (Exception)
            //{
            //}

            //return null;
            ////GridElement[][] arr = null;

            ////var grid = new Grid();

            ////grid.GridElementCollection[0][0] = new GridElement() { ImageId
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
            Image returnvalue = null;

            try
            {
                var path = ImagePath + imageId + ".";
                if (File.Exists(path + "jpg")){
                    if (cache.ContainsKey(path + "jpg"))
                    {
                        cache.TryGetValue(path + "jpg", out returnvalue);
                    }
                    else
                    {
                        returnvalue = Image.FromFile(path + "jpg", true);
                        cache.Add(path + "jpg", returnvalue);
                    }
                }
                else if (File.Exists(path + "jpeg")){
                    if (cache.ContainsKey(path + "jpeg"))
                    {
                        cache.TryGetValue(path + "jpeg", out returnvalue);
                    }
                    else
                    {
                        returnvalue = Image.FromFile(path + "jpeg", true);
                        cache.Add(path + "jpeg", returnvalue);
                    }
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
