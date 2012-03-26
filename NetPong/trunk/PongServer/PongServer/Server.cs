using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PongServer
{

    class Server
    {
        private TcpListener server;
        private ASCIIEncoding enc = new ASCIIEncoding();
        private Socket p1;
        private Socket p2;
        private int port = 8001;
        private IPAddress address;

        public Server()
        {
            //Grabs the LAN ip(v4) address of the server (may not work well if there are multiple network interfaces)
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ip = (
                      from addr in hostEntry.AddressList
                      where addr.AddressFamily == AddressFamily.InterNetwork
                      select addr
            ).LastOrDefault();

            //for now use localhost
            address = ip;
        }

        public void start()
        {
            
            Console.WriteLine("Welcome to the pong server!");
            Console.WriteLine("Your IP address is: {0}", address);
            try
            {
                server = new TcpListener(address, port);
                server.Start();
                p1 = server.AcceptSocket();
                Console.WriteLine("Player 1 connected!");
                p2 = server.AcceptSocket();
                Console.WriteLine("Player 2 connected!");
                
            }
            catch (Exception e)
            {

                Console.WriteLine("Error starting server: {0}", e.Message);
            }
        }
    }
}
