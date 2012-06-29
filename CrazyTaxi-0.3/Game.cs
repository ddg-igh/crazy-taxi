using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CrazyTaxi.Car;
using System.IO;
using CTMapUtils;
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
            Failed = 3,
            MiniMap = 4
        }

        public GameState State { private set; get; }
        public long Score { private set; get; }
        
        private Image miniMap;
        private Map map;
        private Size size;
        private CarImpl car;
        private CollisionEntity entity;
        private bool initialized=false;
        private volatile bool loaded = false;
        private Mission mission;
        private Size gameSize;

        public int MissionLevel{get;private set;}

        public Game()
        {
            State = GameState.Pause;
            MissionLevel = 0;
        }

        public void Initialize(Form parent,int[] dim)
        {
            if (!initialized)
            {
                Score = 0;
                initialized = true;
                size = new Size(dim[0], dim[1]);
                loadMap();
                mission=nextMission();
            }
        }

        private void loadMap()
        {
            DateTime start;
            gameSize = new Size(size.Width * 10, size.Height * 10);
            //Bilderordner angebe     
            MapParser.ImagePath = string.Format(@"{0}{1}MapImages", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
            start = DateTime.Now;

            Random rnd = new Random();
            int random = rnd.Next(0, 3);
            switch (random) 
            {
                case 0:
                    map = MapParser.Load(100, 100, MapParser.SpecialMapElement.RiverCrossing);
                    break;
                case 1:
                    map = MapParser.Load(100, 100, MapParser.SpecialMapElement.None);
                    break;
                case 2:
                    map = MapParser.Load(100, 100, MapParser.SpecialMapElement.CentralPark);
                    break;
            }
            

            Console.WriteLine("MapParser.Load:" + (System.DateTime.Now - start).ToString());
            //Karte vergrößert sich automatisch mit GUI
            //Karte im GUI anzeigen
            start = DateTime.Now;
            map.Initialize(gameSize);
            Console.WriteLine("MapParser.init:" + (System.DateTime.Now - start).ToString());
            //Karte an Größe des GUIs anpassen
            start = DateTime.Now;
            miniMap = map.DrawImage(size);
            Console.WriteLine("MapParser.draw:" + (System.DateTime.Now - start).ToString());



            entity = map.Collision.AddEntity(new Size(12, 23));
            car = new CarImpl(new int[]{size.Width,size.Height},gameSize, entity);
            car.Location = map.GetRandomTilePosition(true, gameSize);

            loaded = true;
        }

        public void updateGame(int ellapsedTIme)
        {
            if (GameState.Running.Equals(State))
            {
                Rectangle bounds = new Rectangle(0, 0, gameSize.Width, gameSize.Height);
                car.Move(bounds);
                mission.Update(ellapsedTIme);

                if (mission.Finished)
                {
                    Score=Score+mission.GetFinishedScore();
                    mission=nextMission();
                }
                else if (mission.Failed)
                {
                    State = GameState.Failed;
                }
                
            }
        }


        public void draw(Graphics g)
        {
            //updateGame(ellapsedTime);

            if (GameState.Loading.Equals(State))
            {
                return;
            }
            else if(GameState.MiniMap.Equals(State))
            {
                g.DrawImageUnscaled(miniMap,0,0);
                car.drawMiniMap(g);
                mission.DrawMiniMap(g,size.Width,size.Height);
            }
            else {
                int x = calculateFramePosition(gameSize.Width,size.Width,car.Location.X);
                int y = calculateFramePosition(gameSize.Height,size.Height,car.Location.Y);
                
                //g.DrawImage(gameSize,new Rectangle(0,0,_Dimension[0], _Dimension[1]),new Rectangle(x,y,_Dimension[0], _Dimension[1]),GraphicsUnit.Pixel);
                //g.DrawImageUnscaled(gameSize, x, y);
                map.Draw(g, size, x, y);
                car.draw(g);
                mission.Draw(g,x,y);
            }
        }




        private int calculateFramePosition(int gameSizeEdge,int screenEdge, int carPosition)
        {
            int result = screenEdge / 2 - carPosition;

            if (result > 0)
            {
                result = 0;
            }
            else if (result < screenEdge - gameSizeEdge)
            {
                result = screenEdge - gameSizeEdge;
            }

            return result;
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

            if (car != null){
                if (key == Keys.Tab)
                {
                    State=GameState.MiniMap;
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
        }

        public void keyUp(Keys key)
        {
            if (GameState.Loading.Equals(this.State))
            {
                return;
            }
            if (car != null)
            {
                if (key == Keys.Tab)
                {
                    State=GameState.Running;
                }
                else if (key == Keys.Up)
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

        private Mission nextMission()
        {
            return new Mission(map.GetRandomTilePosition(true, gameSize), map.GetRandomTilePosition(true, gameSize), car, gameSize.Height, gameSize.Width, MissionLevel++);
        }

    }
}
