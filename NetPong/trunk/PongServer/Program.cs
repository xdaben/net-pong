﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PongServer
{
    class Program
    {
      

        static void Main(string[] args)
        {
            Server server = new Server();
            server.Init();
            server.StartGame();

            
        }
    }
}
