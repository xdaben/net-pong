using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace PongThreaded
{
    public class Paddle
    {
        // private fields
        private double yPos;
        private double speed;

        // properties
        public int Player { get; set; }
        public double Height { get; set; }
        public int Direction { get; set; }
        public double XPos { get; set; }

        public double YPos
        {
            get { return yPos; }

            set
            {
                if (value <= 0)
                {
                    yPos = 0;
                    this.Speed = 0;
                }
                else if (value >= 300)
                {
                    yPos = 300;
                    this.Speed = 0;
                }
                else
                    yPos = value;
            }
        }
        public double Speed
        {
            get { return speed; }

            set
            {
                if (Math.Abs(value) <= 7)
                    speed = value;
            }
        }

        public Paddle(int p)
        {
            Player = p;
            Speed = 0;
            XPos = 0;
            YPos = 0;
            Direction = 0;
            Height = 80;
        }
    }
}