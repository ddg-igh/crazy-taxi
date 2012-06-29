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
using System.Drawing.Imaging;


namespace CrazyTaxi
{
    public partial class CT_UI : Form
    {
        private int[] _Dimension;
        private DoubleBufferedPanel menuContainer;
        private DoubleBufferedPanel menuHolder;
        private HighscoreControlView highscoreView;
        private Image img;
        DateTime startTime = System.DateTime.Now;

        private int x = 0;
        private int y = 0;
        private int genway=-1;

        private Game game;
        private Bitmap backbuffer;  // Backbuffers bitmap
        private Graphics backBufferGraphics; // We draw on this

        private bool _initialized = false;
        private Pong pong;

        //Wird Verwendet da Cursor.hide/show nicht immer bzw nicht funktioniert 
        // using System.Runtime.InteropServices;
        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

      
        
        public CT_UI()
        {
            InitializeComponent();
            Initilize();
            Backgroundfader.Start();
        }

        private void Initilize()
        {
            if (!_initialized)
            {
                //Fullscreen();
                _Dimension = GetResolution();
                Size size = new Size((int)(_Dimension[0] * 1.2), (int)(_Dimension[1] * 1.2));
                img = Properties.Resources._3;
                img = CT_Helper.resizeImage(img, size);

                game = new Game();
                game.Initialize(this, _Dimension);

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

                setCursor();
                this.Focus();
                this.TransparencyKey = Color.FromArgb(255, 255, 254);

                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

                pong=new Pong(new Size(_Dimension[0],_Dimension[1]));

                gameBox.Size = new Size(_Dimension[0], _Dimension[1]);

                // Resize our backbuffer
                backbuffer = new Bitmap(gameBox.Size.Width,gameBox.Size.Height, PixelFormat.Format32bppPArgb);
                backBufferGraphics = Graphics.FromImage(backbuffer);

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
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height; ;
            int[] Resolution = new int[2];
            Resolution[0] = width/2;
            Resolution[1] = height/2;
            int[] retVal = new int[] { width, height };
            return Resolution;
        }

        #region Events
        

        public void Button_Click(object sender, EventArgs e) 
        { 
            VistaButton bt = (VistaButton)sender;

            switch (bt.ButtonText) 
            { 
                case "Starten":
                    showMenu(false);             
                    //Erzeuge neues GamePanel
                    break;
                case "Laden":
                    showMenu(false);
                    //erzeuge neues GamePanel
                    break;
                case "Speichern":
                    //Datenbank ...
                    break;
                case "Fortsetzen":
                    showMenu(false);
                    break;
                case "Beenden":
                    System.Environment.Exit(0);
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

        private List<Keys> keyList = new List<Keys>();

        private void CT_UI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                showMenu(true);
            }
            else
            {

                if (Game.GameState.Running.Equals(game.State) || Game.GameState.MiniMap.Equals(game.State))
                {
                    game.keyDown(e.KeyCode);
                }
                else
                {
                    if (!keyList.Contains(e.KeyCode))
                    {
                        keyList.Add(e.KeyCode);
                    }
                }
            }
        }

        private void CT_UI_KeyUp(object sender, KeyEventArgs e)
        {
            if (Game.GameState.Running.Equals(game.State) || Game.GameState.MiniMap.Equals(game.State))
            {
                game.keyUp(e.KeyCode);
            }
            else
            {
                if (keyList.Contains(e.KeyCode))
                {
                    keyList.Remove(e.KeyCode);
                }
            }
        }

        private void showMenu(bool state)
        {
            game.changeGameState(state);
            menuContainer.Visible = state;
            menuHolder.Visible = state;
            ShowCursor(state);

            if (state)
            {
                Backgroundfader.Start();
            }
            else
            {
                Backgroundfader.Stop();
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

        private void CT_UI_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.CompositingMode=CompositingMode.SourceOver;
            g.CompositingQuality=CompositingQuality.HighSpeed;
            g.PixelOffsetMode=PixelOffsetMode.None;
            g.SmoothingMode=SmoothingMode.None;
            g.InterpolationMode=InterpolationMode.Default;
            
            if (Game.GameState.Pause.Equals(game.State))
            {
                backBufferGraphics.DrawImage(img, new Point(x, y));
            }
            else if (Game.GameState.Running.Equals(game.State) || Game.GameState.MiniMap.Equals(game.State))
            {
                game.draw(backBufferGraphics);
                long difference = (System.DateTime.Now - startTime).Milliseconds;
                double fps = 0;
                if (difference != 0)
                {
                    fps = 1000 / difference;
                }
                backBufferGraphics.DrawString("fps:" + System.Math.Round(fps).ToString(), DefaultFont, Brushes.Yellow, 10, 10);
                backBufferGraphics.DrawString("Score:" + game.Score.ToString(), new Font(FontFamily.GenericSerif, 10), Brushes.Yellow, 100, 10);
            }
            else
            {
                long difference = (System.DateTime.Now - startTime).Milliseconds;
                pong.Update(difference,keyList);
                pong.Draw(backBufferGraphics);
                backBufferGraphics.DrawString("Loading", DefaultFont, Brushes.Yellow, _Dimension[0] / 2, _Dimension[1] / 2);
            }

            g.DrawImageUnscaled(backbuffer, 0, 0);

             startTime= System.DateTime.Now;
             Application.DoEvents();
             gameBox.Invalidate();
        }


        #endregion

        private void Updater_Tick(object sender, EventArgs e)
        {
            game.updateGame(Updater.Interval);   
        }


      


    }
}