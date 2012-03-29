using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace PongThreaded
{
    public class Ball
    {
        // private fields
        private double angle;
        private double speed;
        private double xPos;
        private double yPos;

        // properties
        public int LastPaddle { get; set; } // 1 == paddle1, 2 == paddle 2

        public double Angle
        {
            get { return angle; }
            set { angle = value % 360; }
        }

        public double Speed
        {
            get { return speed; }

            set
            {
                if (value <= 20)
                    speed = value;
            }
        }

        public double XPos
        {
            get { return xPos; }
            set
            {
                if (value >= 0 || value <= 570)
                    xPos = value;
            }
        }
        public double YPos
        {
            get { return yPos; }
            set
            {
                if (value < 0)
                    yPos = 0;
                else if (value > 370)
                    yPos = 370;
                else
                    yPos = value;
            }
        }

        // no-argument default constructor
        public Ball()
        {
            Angle = 90;
            Speed = 6;
            XPos = 40;
            YPos = 180;
        }

        // explicit constructor
        public Ball(int angle, int speed, int xPos, int yPos)
        {
            Angle = angle;
            Speed = speed;
            XPos = xPos;
            YPos = yPos;
        }
    }
}