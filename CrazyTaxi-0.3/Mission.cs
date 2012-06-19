﻿using System;
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

        public Mission(int startX, int startY, int endX, int endY, long time, CarImpl car,int gameFieldHeight,int gameFieldWidth)
        {
            start = new Point(startX, startY);
            end = new Point(endX, endY);
            this.time = time;
            this.playerCar = car;
            this.gameFieldHeight = gameFieldHeight;
            this.gameFieldWidth = gameFieldWidth;
        }

        int radius = 5;
        public void draw(Graphics g,int absX,int absY)
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


        public void update(long elapsedMillis)
 		{
 			int reachedDistance=50;
 		
 			if (MissionState.running.Equals(state)){
 				time-=elapsedMillis;
 				
 			   if (playerCar.carCon.Speed==0){
 					double distance=Math.Sqrt(Math.Pow(end.X-playerCar.Location.X,2)+Math.Pow(end.Y-playerCar.Location.Y,2));
 					if (distance<reachedDistance){
 						state=MissionState.finished;
 					}
 				}
 				
 				if (time<=0){
 					state=MissionState.failed;
 				}
 			}
 			else if (MissionState.waiting.Equals(state)){
 				if (playerCar.carCon.Speed==0){
 					double distance=Math.Sqrt(Math.Pow(start.X-playerCar.Location.X,2)+Math.Pow(start.Y-playerCar.Location.Y,2));
 					if (distance<reachedDistance){
 						state=MissionState.running;
 					}
 				}	
 			}
 			
 		}

    }
}