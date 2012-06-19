using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CrazyTaxi.Car;
using System.IO;
using CTMapUtils;
using CTEngine;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

namespace CrazyTaxi
{
    class Game 
    {

        public enum GameState
        {
            Running = 0,
            Pause = 1,
            Loading = 2,
        }

        private Image gameField=new Bitmap(1,1,PixelFormat.Format32bppPArgb);
        private Map map;
        private Size size;
        private CarImpl car;
        private CollisionEntity entity;
        private bool initialized=false;
        private volatile bool loaded = false;
        public GameState State{private set; get;}
        private Mission mission;

        public Game()
        {
            State = GameState.Pause;
        }

        public void Initialize(Form parent,int[] dim)
        {
            if (!initialized)
            {
                initialized = true;
                size = new Size(dim[0], dim[1]);
                //map.Collision.SetDebugTarget(parent);
                Thread thread = new Thread(this.loadMap);
                thread.Start();
            }
        }

        private void loadMap()
        {
            DateTime start;
            //Bilderordner angebe     
            MapParser.ImagePath = string.Format(@"{0}{1}MapImages", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
            start = DateTime.Now;
            map = MapParser.Load(100, 100);
            Console.WriteLine("MapParser.Load:" + (System.DateTime.Now - start).ToString());
            //Karte vergrößert sich automatisch mit GUI
            //Karte im GUI anzeigen
            start = DateTime.Now;
            map.Initialize(size);
            Console.WriteLine("MapParser.init:" + (System.DateTime.Now - start).ToString());
            //Karte an Größe des GUIs anpassen
            start = DateTime.Now;
            gameField = map.DrawImage(new Size(size.Width * 10, size.Height * 10));
            Console.WriteLine("MapParser.draw:" + (System.DateTime.Now - start).ToString());

            entity = map.Collision.AddEntity(new Size(12, 23));
            car = new CarImpl(new int[]{size.Width,size.Height}, entity);

            loaded = true;
            this.State=GameState.Running;
            mission = new Mission(100,100,gameField.Width-100,gameField.Height-100,50000,car,gameField.Height,gameField.Width);
        }

        private void updateGame()
        {
            if (GameState.Running.Equals(State))
            {
                Rectangle bounds = new Rectangle(0, 0, gameField.Width, gameField.Height);
                car.Move(bounds);
                mission.update(15);
            }
        }


        public void draw(Graphics g)
        {
            updateGame();

            if (GameState.Loading.Equals(State))
            {
                return;
            }
            int x = size.Width / 2 - car.Location.X;
            int y = size.Height / 2 - car.Location.Y;

            if (x > 0)
            {
                x = 0;
            }
            else if (x < size.Width - gameField.Width)
            {
                x = size.Width - gameField.Width;
            }

            if (y > 0)
            {
                y = 0;
            }
            else if (y < size.Height - gameField.Height)
            {
                y = size.Height - gameField.Height;
            }

            //g.DrawImage(gameField,new Rectangle(0,0,_Dimension[0], _Dimension[1]),new Rectangle(x,y,_Dimension[0], _Dimension[1]),GraphicsUnit.Pixel);
            g.DrawImageUnscaled(gameField, x, y);
            car.draw(g, gameField.Width, gameField.Height);
            mission.draw(g,x,y);
            

           //gameScreen.Invalidate();
        }


        public bool changeGameState(bool pause)
        {

            if (pause)
            {
                this.State = GameState.Pause;
            }
            else if (loaded)
            {
                this.State = GameState.Running;
            }
            else
            {
                this.State = GameState.Loading;
            }

            return true;
        }

        public void keyDown(Keys key)
        {
            if (GameState.Loading.Equals(this.State))
            {
                return;
            }

            if (key == Keys.Up)
            {
                car.Up = true;
            }
            else if (key == Keys.Down)
            {
                car.Down = true;
            }
            else if (key == Keys.Left)
            {
                car.Left = true;
            }
            else if (key == Keys.Right)
            {
                car.Right = true;
            }
        }

        public void keyUp(Keys key)
        {
            if (GameState.Loading.Equals(this.State))
            {
                return;
            }

            if (key == Keys.Up)
            {
                car.Up = false;
            }
            else if (key == Keys.Down)
            {
                car.Down = false;
            }
            else if (key == Keys.Left)
            {
                car.Left = false;
            }
            else if (key == Keys.Right)
            {
                car.Right = false;
            }
        }

    }
}
