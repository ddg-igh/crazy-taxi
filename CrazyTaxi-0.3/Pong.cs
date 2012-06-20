using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CrazyTaxi
{
    public class Pong
    {
        private Size Resolution { get; set; }

        private double playerV;
        private double computerV;
        private double ballV;

        public int PlayerScore { get; private set; }
        public int ComputerScore { get; private set; }

        private RectangleF player;
        private RectangleF computer;
        private RectangleF ball;

        private double ballVX;
        private double ballVY;

        private Random ranDirectP;
        private Random ranDirectC;
        private Random ranAreaP;
        private Random ranAreaC;

        private float textSize;

        public Pong(Size resolution)
        {
            Resolution = resolution;

            Random ranSeed = new Random();
            ranDirectP = new Random(ranSeed.Next());
            ranDirectC = new Random(ranSeed.Next());
            ranAreaP = new Random(ranSeed.Next());
            ranAreaC = new Random(ranSeed.Next());

            playerV = resolution.Height  / 2550d;
            computerV = resolution.Height / 2550d;
            ballV = resolution.Width / 2500d + resolution.Height / 5000d;

            int pX = (int)(resolution.Height / 40d);
            int pY = (int)(resolution.Height / 2d);
            int pWidth = (int)(resolution.Height / 40d);
            int pHeight = (int)(resolution.Height / 4d);

            player = new Rectangle(pX, pY, pWidth, pHeight);

            int cX = (int)(resolution.Width - pX - pWidth);
            int cY = pY;
            int cWidth = pWidth;
            int cHeight = pHeight;

            computer = new Rectangle(cX, cY, cWidth, cHeight);

            ball = new Rectangle(0, 0, pWidth, pWidth);
            resetBall(false);

            textSize = resolution.Height / 24f;
        }

        public void ResetScore()
        {
            PlayerScore = 0;
            ComputerScore = 0;
        }

        public void Update(double time, List<Keys> keyCodes)
        {
            processPlayer(time, keyCodes);
            processComputer(time);
            updateBall(time);
        }

        private void processPlayer(double time, List<Keys> keyCodes)
        {
            if (keyCodes.Contains(Keys.Up))
            {
                player.Y -= (float)(playerV * time);
                if (player.Y < 0)
                    player.Y = 0;
            }
            if (keyCodes.Contains(Keys.Down))
            {
                player.Y += (float)(playerV * time);
                if (player.Y + player.Height > Resolution.Height)
                    player.Y = Resolution.Height - player.Height;
            }
        }

        private void processComputer(double time)
        {
            if (ball.Y + ball.Height >= computer.Y + computer.Height - computer.Height / 3)
            {
                computer.Y += (float)(computerV * time);
                if (computer.Y + computer.Height >= Resolution.Height)
                    computer.Y = Resolution.Height - computer.Height;
            }
            else if (ball.Y <= computer.Y + computer.Height / 3)
            {
                computer.Y -= (float)(computerV * time);
                if (computer.Y <= 0)
                    computer.Y = 0;
            }
        }

        private void updateBall(double time)
        {
            ball.X += (float)(ballVX * time);
            ball.Y += (float)(ballVY * time);

            if (ball.Y < 0)
            {
                ball.Y = 0;
                ballVY *= -1;
            }
            else if (ball.Y + ball.Height > Resolution.Height)
            {
                ball.Y = Resolution.Height - ball.Height;
                ballVY *= -1;
            }

            if (ball.X <= player.X + player.Width && ball.X >= player.X + player.Width / 2 &&
                ball.Y + ball.Height >= player.Y && ball.Y <= player.Y + player.Height)
            {
                if (ballVX < 0)
                    setRandomBallDirection(true);
            }
            else if (ball.X + ball.Width >= computer.X && ball.X + ball.Width <= computer.X + computer.Width / 2 &&
                 ball.Y + ball.Height >= computer.Y && ball.Y <= computer.Y + computer.Height)
            {
                if (ballVX > 0)
                    setRandomBallDirection(false);
            }
            else
            {
                if (ball.X <= 0)
                {
                    ComputerScore++;
                    resetBall(true);
                }
                else if (ball.X + ball.Width >= Resolution.Width)
                {
                    PlayerScore++;
                    resetBall(false);
                }
            }
        }

        private void setRandomBallDirection(bool forPlayer)
        {
            setRandomBallDirection(forPlayer, false);
        }

        private void setRandomBallDirection(bool forPlayer, bool noUpArea)
        {
            int direction;
            bool upArea; 

            upArea = forPlayer ? ranAreaP.Next(2) == 0 : ranAreaC.Next(2) == 0;

            if (forPlayer)
                direction = ranDirectP.Next(25, 56);
            else
                direction = ranDirectC.Next(125, 156);
            
            if (upArea && !noUpArea) direction *= -1;

            ballVX = ballV * Math.Cos(direction * Math.PI / 180);
            ballVY = ballV * Math.Sin(direction * Math.PI / 180);
        }

        private void resetBall(bool forPlayer)
        {
            ball.X = Resolution.Width / 2;
            ball.Y = 0;
            setRandomBallDirection(forPlayer, true);
        }
        
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 0, 0, Resolution.Width, Resolution.Height);
            g.FillRectangle(Brushes.White, player);
            g.FillRectangle(Brushes.White, computer);
            g.FillRectangle(Brushes.White, ball);

            g.DrawString("" + PlayerScore, new Font("Arial", textSize), Brushes.White, new PointF(player.Width * 2.5f, player.Width / 2));
            g.DrawString("" + ComputerScore, new Font("Arial", textSize), Brushes.White, new PointF(Resolution.Width - computer.Width * 4.5f, computer.Width / 2));
        }
    }
}
