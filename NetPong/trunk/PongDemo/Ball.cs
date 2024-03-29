﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace PongDemo
{
    class Ball
    {
        // private fields
        private double angle;
        private double speed;
        private double xPos;
        private double yPos;

        // properties
        public Rectangle Sprite { get; set; }
        public int LastPaddle { get; set; } // 1 == paddle1, 2 == paddle 2, 3 == top, 4 == bottom
        public int LastBounce { get; set; }

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
            get
            { 
                return xPos;
            }
            set
            { 
                if (value >= 0 || value <= 570)
                    xPos = value;
            }
        }
        public double YPos
        {
            get
            {
                return yPos;
            }
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

        public Ball()
        {
            Angle = 0;
            Speed = 1;
            XPos = 0;
            YPos = 0;
        }
    }
}
