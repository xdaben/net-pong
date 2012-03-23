using System;
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

        // properties
        public Rectangle Sprite { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
        public int LastPaddle { get; set; }

        public double Angle
        {
            get { return angle; }
            set { angle = value % 360; }
        }

        public double Speed
        {
            get { return speed; }
            set { speed = value; }
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
