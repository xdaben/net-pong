using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicPhysics
{
    class Program
    {
        static void Main(string[] args)
        {
            Game theGame = new Game();
            theGame.play();
        }
    }

    class Game
    {
        public void play()
        {
            Ball ball = new Ball(0, 5, 2, 1);

            while (true)
            {
                Console.Clear();
                ball.paintBall();
                System.Threading.Thread.Sleep(50);
                ball.moveBall();
            }
        }
    }

    class Ball
    {
        public double xPosition { get; set; }
        public double yPosition { get; set; }
        public double xVelocity { get; set; }
        public double yVelocity { get; set; }

        public Ball(int x, int y, int xV, int yV)
        {
            xPosition = x;
            yPosition = y;
            xVelocity = xV;
            yVelocity = yV;
        }

        public void moveBall()
        {
            double tmpX = xPosition + xVelocity;
            double tmpY = yPosition + yVelocity;

            if (tmpX >= Console.WindowWidth || tmpX < 0)
                xVelocity *= -1;

            if (tmpY >= Console.WindowHeight || tmpY < 0)
                yVelocity *= -1;

            xPosition += xVelocity;
            yPosition += yVelocity;
        }

        public void paintBall()
        {
            Console.CursorLeft = (int)xPosition;
            Console.CursorTop = (int)yPosition;
            Console.Write("@");
        }
    }
}
