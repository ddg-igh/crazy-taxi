using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CrazyTaxi.Car;

namespace CrazyTaxi
{
    class Mission
    {

        enum MissionState
        {
            waiting = 0,
            running = 1,
            finished = 2,
            failed = 3
        }

        private Point start;
        private Point end;
        private long time;
        private MissionState state = MissionState.waiting;
        private CarImpl playerCar;
        private int gameFieldHeight;
        private int gameFieldWidth;
        private int level;

        public bool Finished { private set; get; }
        public bool Failed { private set; get; }

        public Mission(Point start,Point end, CarImpl car, int gameFieldHeight, int gameFieldWidth, int level)
        {
            this.start = start;
            this.end = end;
            this.playerCar = car;
            this.gameFieldHeight = gameFieldHeight;
            this.gameFieldWidth = gameFieldWidth;
            Finished = false;
            Failed = false;
            this.level = level;

            this.time=calculateTime();
        }


        private long calculateTime()
        {
            int distance = Math.Abs(end.X - start.X) + Math.Abs(end.Y - start.Y);
            double steps=distance/CarController.MAX_SPEED;

            return (long)Math.Round((steps * CT_UI.UPDATE_INTERVAL) * (2 - level / 20)); ;
        }

        private int radius = 5;
        public void Draw(Graphics g,int absX,int absY)
  		{
  			if (MissionState.waiting.Equals(state)){
                g.DrawEllipse(new Pen(Brushes.Red), start.X + absX-radius, start.Y + absY-radius, radius*2, radius*2);
  			}
  			else if (MissionState.running.Equals(state)){
                g.DrawEllipse(new Pen(Brushes.Red), end.X + absX-radius, end.Y-radius + absY, radius*2, radius*2);
  			    double timeInSec=time/1000;
  			    g.DrawString("Remaining:"+timeInSec+" s",new Font(FontFamily.GenericSerif,12),Brushes.Yellow,500,10);
  			}
  			else if (MissionState.finished.Equals(state)){
  			}
  			else if (MissionState.failed.Equals(state)){
  			}

            if (++radius > 25)
            {
                radius = 5;
            }
  		}

        private double miniMapRadius = 2;
        public void DrawMiniMap(Graphics g,int screenWidth,int screenHeight)
        {
            if (MissionState.waiting.Equals(state))
            {
                int x = screenWidth * start.X / gameFieldWidth;
                int y = screenHeight * start.Y / gameFieldHeight;
                g.FillEllipse(Brushes.Red, x - (int)miniMapRadius, y - (int)miniMapRadius, (int)(miniMapRadius * 2), (int)(miniMapRadius * 2));
            }
            else if (MissionState.running.Equals(state))
            {
                int x = screenWidth * end.X / gameFieldWidth;
                int y = screenHeight * end.Y / gameFieldHeight;
                g.FillEllipse(Brushes.Red, x - (int)miniMapRadius, y - (int)miniMapRadius, (int)(miniMapRadius * 2), (int)(miniMapRadius * 2));
            }

            miniMapRadius += 0.5;
            if (miniMapRadius > 15)
            {
                miniMapRadius = 2;
            }
        }



        public long GetFinishedScore()
        {
            if (MissionState.finished.Equals(state))
            {
                return time;
            }

            return 0;
        }


        public void Update(long elapsedMillis)
 		{
 			int reachedDistance=50;
 		
 			if (MissionState.running.Equals(state)){
 				time-=elapsedMillis;
 				
 			   if ((int)playerCar.carCon.Speed==0){
 					double distance=Math.Sqrt(Math.Pow(end.X-playerCar.Location.X,2)+Math.Pow(end.Y-playerCar.Location.Y,2));
 					if (distance<reachedDistance){
 						state=MissionState.finished;
                        Finished = true;
 					}
 				}
 				
 				if (time<=0){
 					state=MissionState.failed;
                    Failed = true;
 				}
 			}
 			else if (MissionState.waiting.Equals(state)){
                if ((int)playerCar.carCon.Speed == 0)
                {
 					double distance=Math.Sqrt(Math.Pow(start.X-playerCar.Location.X,2)+Math.Pow(start.Y-playerCar.Location.Y,2));
 					if (distance<reachedDistance){
 						state=MissionState.running;
 					}
 				}	
 			}
 			
 		}

    }
}
