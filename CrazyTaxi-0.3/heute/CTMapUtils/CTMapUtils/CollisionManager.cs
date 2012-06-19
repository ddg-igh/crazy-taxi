using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace CTMapUtils
{
    public class CollisionManager
    {
        private Grid grid;
        internal int elementWidth;
        internal int elementHeight;

        private List<CollisionEntity> entities = new List<CollisionEntity>();

        //private Map debugMAP;
        internal CollisionManager(Grid mapGrid, int gridElementWidth, int gridElementHeight/*, Map DEBUGMAP*/)
        {
            grid = mapGrid;
            elementWidth = gridElementWidth;
            elementHeight = gridElementHeight;

            //debugMAP = DEBUGMAP;
        }

        internal void updateElementDimensions(int width, int height)
        {
            elementWidth = width;
            elementHeight = height;
        }

        public CollisionEntity AddEntity(System.Drawing.Size size)
        {
            var returnvalue = new CollisionEntity(this, size, 4);

            entities.Add(returnvalue);

            return returnvalue;
        }

        private List<Control> ctrls = new List<Control>();
        internal bool Collides(Vector[] points, int carX, int carY/*, bool remove*/)
        {
            //if (remove)
            //{
            //    foreach (var ctrl in ctrls)
            //    {
            //        debugMAP.Controls.Remove(ctrl);
            //    }
            //}

            var returnvalue = false;

            var carGridX = (int)(carX  / elementWidth);
            var carGridY = (int)(carY / elementHeight);
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];

                var px = (int)(point.X / elementWidth);
                var py = (int)(point.Y / elementHeight);
                //var ctrl = new Label() { BackColor = Color.Green, Text = "", Location = new System.Drawing.Point((int)point.X, (int)point.Y), Width = 5, Height = 5 };
                if (px != carGridX || py != carGridY)
                {
                    if (px < grid.GridElementCollection.Length && px >= 0)
                    {
                        var side = grid.GridElementCollection[px];
                        if (py < side.Length && py >= 0)
                        {
                            if (side[py].PassableSides == 0)
                            {
                                returnvalue = true;
                                //ctrl.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            returnvalue = true;
                            //ctrl.BackColor = Color.Yellow;
                        }
                    }
                    else
                    {
                        returnvalue = true;
                        //ctrl.BackColor = Color.Yellow;
                    }
                }
                //ctrls.Add(ctrl);
                //debugMAP.Controls.Add(ctrl);
                //ctrl.BringToFront();
            }

            return returnvalue;
        }
    }
}
