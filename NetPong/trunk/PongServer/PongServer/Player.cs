using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PongServer
{
    class Player
    {
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public int Score { get; set; }
        public Socket NetPlayer { get; set; }
    }
}
