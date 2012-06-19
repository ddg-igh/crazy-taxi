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

        private Control debugMAP = null;
        internal CollisionManager(Grid mapGrid, int gridElementWidth, int gridElementHeight)
        {
            grid = mapGrid;
            elementWidth = gridElementWidth;
            elementHeight = gridElementHeight;
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

        public void SetDebugTarget(Control target)
        {
            debugMAP = target;
        }

        public void ResetDebugOutput()
        {
            if (debugMAP != null)
            {
                foreach (var ctrl in ctrls)
                {
                    debugMAP.Controls.Remove(ctrl);
                }
            }
        }

        private List<Control> ctrls = new List<Control>();
        internal bool Collides(Vector[] points, int carX, int carY)
        {
            var returnvalue = false;

            var carGridX = (int)(carX  / elementWidth);
            var carGridY = (int)(carY / elementHeight);
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];

                var px = (int)(point.X / elementWidth);
                var py = (int)(point.Y / elementHeight);
                var ctrl = new Label() { BackColor = Color.Green, Text = "", Location = new System.Drawing.Point((int)point.X, (int)point.Y), Width = 5, Height = 5 };
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
                                ctrl.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            returnvalue = true;
                            ctrl.BackColor = Color.Yellow;
                        }
                    }
                    else
                    {
                        returnvalue = true;
                        ctrl.BackColor = Color.Yellow;
                    }
                }
#if (DEBUG)
                if (debugMAP != null)
                {
                    ctrls.Add(ctrl);
                    debugMAP.Controls.Add(ctrl);
                    ctrl.BringToFront();
                }
#endif
            }

            return returnvalue;
        }
    }
}
