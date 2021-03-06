﻿ using System;
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
        public static int UPDATE_INTERVAL=30;

        private Size _Dimension;
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
        private bool playPong = false;
        private bool showFPS = false;

        //Wird Verwendet da Cursor.hide/show nicht immer bzw nicht funktioniert 
        // using System.Runtime.InteropServices;
        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

      
        
        public CT_UI()
        {
            InitializeComponent();
            Initialize();
            Backgroundfader.Start();
        }

        private void Initialize()
        {
            if (!_initialized)
            {
                Fullscreen();
                _Dimension = GetResolution();
                Size size = new Size((int)(_Dimension.Width * 1.2), (int)(_Dimension.Height * 1.2));
                img = Properties.Resources._3;
                img = CT_Helper.resizeImage(img, size);

                initGame();

                #region Menu
                int drawingPointWidth = _Dimension.Width / 2;
                int drawingPointHeight = _Dimension.Height / 2;
                Point menu = new Point(drawingPointWidth - 683 / 2, drawingPointHeight - 384 / 2);
                menuContainer = new DoubleBufferedPanel(menu);

                menuContainer.Width = _Dimension.Width;
                menuContainer.Height = _Dimension.Height;

                menuContainer.Location = new Point(0, 0);
                menuContainer.Name = "Menu";

                menuHolder = new DoubleBufferedPanel();

                string[] buttonTexts = { "Starten", "Laden", "Speichern", "Highscore","Pong spielen","Beenden" };
                int positionMultiplicator = 1;
                int offset = 10;
                foreach (string text in buttonTexts)
                {

                    VistaButton button = new VistaButton();
                    button.ButtonText = text;

                    button.Width = 160;
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

                pong=new Pong(new Size(_Dimension.Width,_Dimension.Height));

                gameBox.Size = new Size(_Dimension.Width, _Dimension.Height);

                // Resize our backbuffer
                backbuffer = new Bitmap(gameBox.Size.Width,gameBox.Size.Height, PixelFormat.Format32bppPArgb);
                backBufferGraphics = Graphics.FromImage(backbuffer);

                Updater.Interval = UPDATE_INTERVAL;

                _initialized = true;
            }
        }

        private void initGame()
        {
            game = new Game();
            game.Initialize(this, _Dimension);
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

        private Size GetResolution()
        {
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height; 
            return new Size( width, height );
        }

        #region Events
        

        public void Button_Click(object sender, EventArgs e) 
        { 
            VistaButton bt = (VistaButton)sender;

            switch (bt.ButtonText) 
            { 
                case "Starten":
                    showMenu(false);
                    bt.ButtonText = "Fortsetzen";
                    break;
                case "Fortsetzen":
                    if (Game.GameState.Failed.Equals(game.State)){
                        initGame();
                    }
                    showMenu(false);
                    break;
                case "Laden":
                    showMenu(false);
                    //erzeuge neues GamePanel
                    break;
                case "Speichern":
                    //Datenbank ...
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
                case "Pong spielen":
                    playPong = true;
                    showMenu(false);
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
                playPong = false;
                showMenu(true);
            }
            else if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            {
                showFPS = !showFPS;
            }
            else
            {

                if (playPong)
                {
                    if (!keyList.Contains(e.KeyCode))
                    {
                        keyList.Add(e.KeyCode);
                    }
                }
                else if (Game.GameState.Running.Equals(game.State) || Game.GameState.MiniMap.Equals(game.State))
                {
                    game.keyDown(e.KeyCode);
                }

            }
        }

        private void CT_UI_KeyUp(object sender, KeyEventArgs e)
        {
            if (playPong)
            {
                if (keyList.Contains(e.KeyCode))
                {
                    keyList.Remove(e.KeyCode);
                }
            }
            else if (Game.GameState.Running.Equals(game.State) || Game.GameState.MiniMap.Equals(game.State))
            {
                game.keyUp(e.KeyCode);
            }
        }

        private void showMenu(bool state)
        {
            this.Focus();
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
            int speed = (_Dimension.Width+_Dimension.Height)/1000;
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
                x = _Dimension.Width - img.Width;
                y = _Dimension.Height - img.Height;
            }  
            else if (genway == 1)
            {
                x = 0;
                y = 0;
            }
            else if (genway == 2) 
            {
                x = _Dimension.Width - img.Width;
                y = 0;
            }
            else if (genway == 3)
            {
                x = 0;
                y = _Dimension.Height - img.Height;
            }
            


            Random rand = new Random();
            Size size = new Size((int)(_Dimension.Width * 1.2), (int)(_Dimension.Height * 1.2));
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
            int difWidth = _Dimension.Width - img.Width;
            int difHeight = _Dimension.Height - img.Height;
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

            if (playPong)
            {
                long difference = (System.DateTime.Now - startTime).Milliseconds;
                pong.Update(difference, keyList);
                pong.Draw(backBufferGraphics);
            }
            else if (Game.GameState.Pause.Equals(game.State))
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
                if (showFPS)
                {
                    backBufferGraphics.DrawString("fps:" + System.Math.Round(fps).ToString(), DefaultFont, Brushes.Yellow, 10, 10);
                }
                backBufferGraphics.DrawString("Score:" + game.Score.ToString(), new Font(FontFamily.GenericSerif, 10), Brushes.Yellow, 100, 10);
                backBufferGraphics.DrawString("Level:" + game.MissionLevel.ToString(), new Font(FontFamily.GenericSerif, 10), Brushes.Yellow, 250, 10);
            }
            else if (Game.GameState.Failed.Equals(game.State))
            {
                backBufferGraphics.DrawString("Game Over \n Score=" + game.Score, new Font(FontFamily.GenericSansSerif, 50), Brushes.Crimson, _Dimension.Width / 2 -200, _Dimension.Height / 2-50);          
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