 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using VistaButtonTest;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using CrazyTaxi.Car;
using System.IO;
using CTMapUtils;
using CTEngine;


namespace CrazyTaxi
{
    public partial class CT_UI : Form
    {
        private int[] _Dimension;
        private DoubleBufferedPanel menuContainer;
        private DoubleBufferedPanel menuHolder;
        private HighscoreControlView highscoreView;
        private Image img;

        private bool fadeIn = false;
        private int fade = 150;        
        private int x = 0;
        private int y = 0;
        private string way = null;
        private int genway=-1;
        private CarPanel carpanel;
        private bool started = false;
        private Map map;
        private bool _initialized = false;
        private CollisionEntity entity;

        //Wird Verwendet da Cursor.hide/show nicht immer bzw nicht funktioniert 
        // using System.Runtime.InteropServices;
        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

        private enum keys
        {
            down = 0,
            up = 1,
            left = 2,
            right = 3
        }
        
        public CT_UI()
        {
            InitializeComponent();
            Initilize();
            carpanel.Visible = false;
            Backgroundfader.Start();
            
        }

        private void Initilize()
        {
            if (!_initialized)
            {
                Fullscreen();
                _Dimension = GetResolution();
                Size size = new Size((int)(_Dimension[0] * 1.2), (int)(_Dimension[1] * 1.2));
                img = Properties.Resources._3;
                img = CT_Helper.resizeImage(img, size);

                #region Menu
                int drawingPointWidth = _Dimension[0] / 2;
                int drawingPointHeight = _Dimension[1] / 2;
                Point menu = new Point(drawingPointWidth - 683 / 2, drawingPointHeight - 384 / 2);
                menuContainer = new DoubleBufferedPanel(menu);



                menuContainer.Width = _Dimension[0];
                menuContainer.Height = _Dimension[1];

                menuContainer.Location = new Point(0, 0);
                menuContainer.Name = "Menu";

                menuHolder = new DoubleBufferedPanel();

                string[] buttonTexts = { "Starten", "Laden", "Speichern", "Fortsetzen", "Beenden", "Highscore" };
                int positionMultiplicator = 1;
                int offset = 10;
                foreach (string text in buttonTexts)
                {

                    VistaButton button = new VistaButton();
                    button.ButtonText = text;

                    button.Width = 136;
                    button.Height = 38;
                    button.Location = new Point(offset + button.Width / 4, (button.Height + offset) * positionMultiplicator);
                    menuHolder.Controls.Add(button);
                    positionMultiplicator++;
                    button.Click += Button_Click;
                    button.KeyDown += CT_UI_KeyDown;
                    button.KeyUp += CT_UI_KeyUp;
                    button.BackColor = Color.Transparent;
                    button.BaseColor = Color.Transparent;
                    button.ButtonColor = Color.FromArgb(155, Color.DarkSlateGray);
                    button.GlowColor = Color.LightGray;
                    button.Font = new System.Drawing.Font("Pricedown", 16.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    button.CornerRadius = 8;
                }

                this.Controls.Add(menuContainer);
                this.Controls.Add(menuHolder);
                menuHolder.BringToFront();
                menuHolder.BorderStyle = BorderStyle.Fixed3D;
                menuHolder.Width = 683;
                menuHolder.Height = 384;
                menuHolder.Location = menu;
                menuHolder.BackColor = Color.Black;
                menuContainer.BackColor = Color.FromArgb(150, Color.Black);

                #endregion

                #region car
                carpanel = new CarPanel(_Dimension);
                //this.Controls.Add(carpanel);
                #endregion

                setCursor();
                this.Focus();

                //Bilderordner angeben
                MapParser.ImagePath = string.Format(@"{0}{1}MapImages", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
                //Karte aus XML laden
                //map = MapParser.Load(@"C:\Users\Bongo\Desktop\Berufsschule\Diagramme AE\Game\CrazyTaxi\testmap.xml");
                map = MapParser.Load(20, 20);
                //Karte vergrößert sich automatisch mit GUI
                //Karte im GUI anzeigen

                //Karte an Größe des GUIs anpassen
                map.Initialize(new Size(_Dimension[0], _Dimension[1]));
                this.BackgroundImage = map.DrawImage(new Size(_Dimension[0],_Dimension[1]));
                this.TransparencyKey = Color.FromArgb(255, 255, 254);
                this.Controls.Add(carpanel);
                map.Collision.SetDebugTarget(this);
                entity = map.Collision.AddEntity(new Size(12,23));
                
                _initialized = true;
            }
        }

        private void setCursor() 
        {
                //FileStream str = new FileStream(@"C:\Users\Bongo\Desktop\Berufsschule\Diagramme AE\Game\CrazyTaxi\CrazyTaxi\Resources\BlackAngel.cur", FileMode.Open);
                //this.Cursor = new Cursor(str); 
            this.Cursor = new Cursor(Application.StartupPath + "..\\..\\..\\Custom.cur");
        }

        private void Fullscreen()
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private int[] GetResolution()
        {
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int[] Resolution = new int[2];
            Resolution[0] = width;
            Resolution[1] = height;
            int[] retVal = new int[] { width, height };
            return Resolution;
        }

        #region Events
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        

        public void Button_Click(object sender, EventArgs e) 
        { 
            VistaButton bt = (VistaButton)sender;

            switch (bt.ButtonText) 
            { 
                case "Starten":
                    if (Backgroundfader.Enabled) 
                    {
                        Backgroundfader.Stop();
                        menuFader.Start();
                        img = Properties.Resources.back3;
                        started = true;
                    }
                    else if (fade >= 150) 
                    {
                        menuFader.Start();
                    }
                    //Erzeuge neues GamePanel
                    break;
                case "Laden":
                    started = true;
                    //erzeuge neues GamePanel
                    break;
                case "Speichern":
                    //Datenbank ...
                    break;
                case "Fortsetzen":
                    if (fade >= 150)
                    {
                        menuFader.Start();
                    }
                    //zurück zu Game Panel
                    break;
                case "Beenden":
                    this.Close();
                    
                    break;
                case "Highscore":
                    if (highscoreView != null && menuHolder.Controls.Contains(highscoreView)) 
                    {
                        menuHolder.Controls.Remove(highscoreView);
                    }
                    else
                    {
                        highscoreView = new HighscoreControlView();
                        highscoreView.Size = new Size(menuHolder.Size.Width - 225, menuHolder.Size.Height-7);
                        highscoreView.Location = new Point(220, 2);
                        //highscoreView.BackColor = Color.DimGray;
                        menuHolder.Controls.Add(highscoreView);
                    }
                    break;
                default:
                    break;
            
            }
        }

        private void CT_UI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) 
            {
                menuFader.Start();
            }
            if (e.KeyCode == Keys.Up)
            {
                up = true;
                MoveMotionOne.Start();
            }
            else if (e.KeyCode == Keys.Down)
            {
                down = true;
                MoveMotionOne.Start();
            }

            if (e.KeyCode == Keys.Left)
            {
                left = true;
                MoveMotionOne.Start();
            }
            else if (e.KeyCode == Keys.Right)
            {
                right = true;
                MoveMotionOne.Start();
            }
        }

        private void CT_UI_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                up = false;
                //MoveMotionOne.Stop();
            }
            else if (e.KeyCode == Keys.Down)
            {
                down = false;
                //MoveMotionOne.Stop();
            }

            if (e.KeyCode == Keys.Left)
            {
                left = false;
                //MoveMotionTwo.Stop();
            }
            else if (e.KeyCode == Keys.Right)
            {
                right = false;
                //MoveMotionTwo.Stop();
            }
        }

        private void menuFader_Tick(object sender, EventArgs e)
        {
            menuContainer.BackColor = Color.FromArgb(fade, Color.Black);
            if (fadeIn)
            {
                //Menu anzeigen auto ausblenden
                carpanel.SendToBack();
                menuContainer.Visible = true;
                fade += 10;
                
            }
            else
            {
                menuHolder.Visible = false;
                menuContainer.vanishFont();

                fade -= 10;
            }
            if (fade == 150)
            {
                fadeIn = false;
                menuFader.Stop();
                menuContainer.Visible = true;
                menuHolder.Visible = true;
                menuContainer.showFont();
                ShowCursor(true);
                //map.Visible = false;
            }
            else if (fade == 0)
            {
                //Fading beendet auto anzeigen
                carpanel.BringToFront();
                fadeIn = true;
                menuFader.Stop();
                ShowCursor(false);
                //menuContainer.Controls.Add(map);
            }
        }

        private void Backgroundfader_Tick(object sender, EventArgs e)
        {
            imageChange();
            int speed = (_Dimension[0]+_Dimension[1])/1000;
            switch (genway)
            {
                case 0:
                    y += speed;
                    x += speed;
                    break;
                case 1:
                    y -= speed;
                    x -= speed;
                    break;
                case 2:
                    y -= speed;
                    x += speed;
                    break;
                case 3:
                    y += speed;
                    x -= speed;
                    break;
            }

            this.Invalidate();
        }

        private void ChangeImage()
        {
            /*
             * genway:
             * 0: DownRight
             * 1: TopLeft
             * 2: TopRight
             * 3: DownLeft
             */
            Random rnd = new Random();
            genway = rnd.Next(0, 4);

            if(genway==0)
            {
                x = _Dimension[0] - img.Width;
                y = _Dimension[1] - img.Height;
            }  
            else if (genway == 1)
            {
                x = 0;
                y = 0;
            }
            else if (genway == 2) 
            {
                x = _Dimension[0] - img.Width;
                y = 0;
            }
            else if (genway == 3)
            {
                x = 0;
                y = _Dimension[1] - img.Height;
            }
            


            Random rand = new Random();
            Size size = new Size((int)(_Dimension[0] * 1.2), (int)(_Dimension[1] * 1.2));
            switch (rand.Next(0, 3))
            {
                case 0:
                    img = CT_Helper.resizeImage(Properties.Resources._2, size);
                    break;
                case 1:
                    img = CT_Helper.resizeImage(Properties.Resources._3, size);
                    break;
                case 2:
                    img = CT_Helper.resizeImage(Properties.Resources._4, size);
                    break;
            }
        }

        private void imageChange()
        {
            int difWidth = _Dimension[0] - img.Width;
            int difHeight = _Dimension[1] - img.Height;
            if (genway==0)
            {
                if (x >= 0 || y >= 0)
                {
                    ChangeImage();
                }
            }
            else if (genway==1)
            {
                if (difWidth >= x || difHeight >= y)
                {
                    ChangeImage();
                }
            }
            else if (genway == 2)
            {
                if (x >= 0 || y <= difHeight)
                {
                    ChangeImage();
                }


            }
            else if (genway == 3)
            {
                if (x <= difWidth  || y >= 0)
                {
                    ChangeImage();
                }
            }
            else 
            {
                genway=1;
            }
        }

        int x1 = 0;
        int y1 = 0;
        private void CT_UI_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (!started)
            {
                g.DrawImage(img, new Point(x, y));
            }
            else 
            {

                System.Drawing.Drawing2D.Matrix m = g.Transform;
                //here we do not need to translate, we rotate at the specified point
                float x = (float)(23 / 2) + (float)carpanel.Location.X;
                float y = (float)(12 / 2) + (float)carpanel.Location.Y;
                m.RotateAt(carpanel.Angle, new PointF(x, y), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.Transform = m;
                g.DrawImage(CT_Helper.resizeImage(Properties.Resources.Taxi_GTA2,new Size(23,12)), new Point(carpanel.Location.X-11 ,carpanel.Location.Y-6));
                
                
            }
        }

        #endregion
        private void MoveMotionOne_Tick(object sender, EventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, _Dimension[0], _Dimension[1]);
            if (up)
            {
                map.Collision.ResetDebugOutput();
                if (!entity.Update(carpanel.Location, Convert.ToInt32(carpanel.Angle + 90), true).Front)
                {
                    
                    carpanel.Move((int)keys.up, bounds);
                    
                }
            }
            else if (down)
            {
                //if (!entity.Update(carpanel.Location, Convert.ToInt32(carpanel.Angle), false).Back)
                    carpanel.Move((int)keys.down, bounds);
            }
            else if (!up) 
            {
                if(carpanel.FinishMove(new Rectangle(0, 0, _Dimension[0], _Dimension[1])))
                {
                    MoveMotionOne.Stop();
                }
            }

            if (up || down)
            {
                if (right)
                {
                    //Rectangle bounds = new Rectangle(0, 0, _Dimension[0], _Dimension[1]);
                    carpanel.Move((int)keys.right, bounds);
                }
                else if (left)
                {
                    //Rectangle bounds = new Rectangle(0, 0, _Dimension[0], _Dimension[1]);
                    carpanel.Move((int)keys.left, bounds);
                }
            }
            Invalidate();
        }

        private void MoveMotionTwo_Tick(object sender, EventArgs e)
        {
            if (up || down)
            {
                if (right)
                {
                    Rectangle bounds = new Rectangle(0, 0, _Dimension[0], _Dimension[1]);
                    carpanel.Move((int)keys.right, bounds);
                }
                else if (left)
                {
                    Rectangle bounds = new Rectangle(0, 0, _Dimension[0], _Dimension[1]);
                    carpanel.Move((int)keys.left, bounds);
                }
            }
        }


    }
}