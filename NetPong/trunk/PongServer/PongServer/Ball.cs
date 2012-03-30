using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PongServer
{
    public class Ball
    {
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public double Angle { get; set; }
        public double Speed { get; set; }

        public int LastPaddle { get; set; }
    }
}
