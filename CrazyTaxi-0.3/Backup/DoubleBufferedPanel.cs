using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CrazyTaxi
{
    class DoubleBufferedPanel : Panel
    {
        private Point fontPoint;
        private Point fontPoint2;
        private bool vanish;

        public DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
        }

        public DoubleBufferedPanel(Point menu)
        {
            this.DoubleBuffered = true;
            //fontPoint = new Point(menu.X,menu.Y);
            //fontPoint2 = new Point(menu.X+170, menu.Y);
            fontPoint = new Point(menu.X, menu.Y);
            fontPoint2 = new Point(menu.X + 170, menu.Y);
            this.Paint += new PaintEventHandler(Panel_Paint);          
        }

        public void vanishFont() 
        {
            vanish = true;
        }

        public void showFont()
        {
            vanish = false; ;
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            if (!vanish)
            {
                Graphics g = e.Graphics;

                // Vertical text:
                //FontFamily ff = new FontFamily("Pricedown");
                FontFamily ff = new FontFamily("Haettenschweiler");

                Font f = new Font(ff, 72, FontStyle.Regular);
                StringFormat sf = new StringFormat();

                // Local rotation of vertcal text (sf):
                GraphicsPath gp = new GraphicsPath();
                gp.AddString("Crazy ", ff, (int)FontStyle.Bold, 72, fontPoint, sf);

                GraphicsPath gp2 = new GraphicsPath();
                gp2.AddString("Taxi", ff, (int)FontStyle.Bold, 72, fontPoint2, sf);

                Matrix m = new Matrix();
                m.Rotate(-10.0f);  // clockwise
                gp.Transform(m);
                gp2.Transform(m);

                Pen thickPen = new Pen(Color.Black,3);

                g.FillPath(Brushes.Red, gp);
                g.FillPath(Brushes.Yellow, gp2);
                g.DrawPath(thickPen, gp);
                g.DrawPath(thickPen, gp2);
                //Paint a gradient over the string
                using (Brush br = new LinearGradientBrush(new Point(0, 0), new Point(3, 72),Color.FromArgb(255, Color.Black), Color.FromArgb(0, Color.Black)))
                {
                    g.FillPath(br, gp);
                }
                using (Brush br = new LinearGradientBrush(new Point(5, 0), new Point(11, 72), Color.FromArgb(255, Color.Black), Color.FromArgb(0, Color.Black)))
                {
                    g.FillPath(br, gp2);
                }
            }
        }

        public void DrawRoundRect(Graphics g, Pen p, float X, float Y, float width, float height, float radius)
        {
            Y+=5;
            X+=5;
            width -= 12;
            height -= 12;
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
            gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
            gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
            gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();
            g.FillPath(Brushes.Transparent, gp);
            gp.Dispose();


            gp = new GraphicsPath();
            gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
            gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
            gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
            gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();
            g.DrawPath(p, gp);
            gp.Dispose();

        }
    
    }
}
