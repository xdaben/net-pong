using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PongServer
{
    class Player
    {
        public int PlayerNum { get; set; }
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public int Score { get; set; }
        private StateObject state = new StateObject();
        public StateObject State
        {
            get
            {
                return state;
            }
        }

        public Socket NetPlayer
        {
            get
            {
                return State.workSocket;
            }
            set
            {
                State.workSocket = value;
            }
        }
    }
}
