using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PongServer
{

    class Server
    {
        //Network stuff
        private TcpListener server;
        private ASCIIEncoding enc = new ASCIIEncoding();
        private int port = 8001;
        private IPAddress address;
        //end Network stuff

        //Game vars
        //player 1
        Player player1 = new Player();
        //Player 2
        Player player2 = new Player();
        //ball
        Ball ball = new Ball();
        //end Game vars



        public Server()
        {
            //Grabs the LAN ip(v4) address of the server (may not work well if there are multiple network interfaces)
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ip = (
                      from addr in hostEntry.AddressList
                      where addr.AddressFamily == AddressFamily.InterNetwork
                      select addr
            ).FirstOrDefault();

            //for now use localhost
            address = IPAddress.Parse("127.0.0.1");
            //address = ip;
        }

        internal void Init()
        {
            
            Console.WriteLine("Welcome to the pong server!");
            try
            {
                server = new TcpListener(address, port);
                server.Start();
                Console.WriteLine("Server started, now waiting for clients...");
                Console.WriteLine("Your IP address is: {0}", address);
                Console.WriteLine("Give someone this if they want to join the pong server");
                player1.NetPlayer = server.AcceptSocket();
                Console.WriteLine("Player 1 connected! ({0})", player1.NetPlayer.RemoteEndPoint);
                player1.NetPlayer.Send(enc.GetBytes("Player 1."));
                player2.NetPlayer = server.AcceptSocket();
                Console.WriteLine("Player 2 connected! ({0})", player2.NetPlayer.RemoteEndPoint);
                player2.NetPlayer.Send(enc.GetBytes("Player 2."));
                
                
            }
            catch (Exception e)
            {

                Console.WriteLine("Error starting server: {0}", e.Message);
            }

            //set up game vars
            player1.Xpos = 15;
            player1.Ypos = 150;
            player1.Score = 0;

            player2.Xpos = 560;
            player2.Ypos = 150;
            player2.Score = 0;
            
            ball.Xpos = 40;
            ball.Ypos = 180;
            ball.Angle = 90;
            ball.Speed = 6;
        }

        internal void StartGame()
        {
            //TODO: Make a better loop
            while (true)
            {
                Update();
                Thread.Sleep(10);
            }
            
        }

        private void Update()
        {
            
            String dataP1 = String.Format("{0} {1} {2} ", player1.Xpos, player1.Ypos, player1.Score);
            String dataP2 = String.Format("{0} {1} {2} ", player2.Xpos, player2.Ypos, player2.Score);
            String dataBall = String.Format("{0} {1} {2} {3}", ball.Xpos, ball.Ypos, ball.Angle, ball.Speed);
            player1.NetPlayer.Send(enc.GetBytes(dataP2 + dataBall));
            player2.NetPlayer.Send(enc.GetBytes(dataP1 + dataBall));
            //SocketAsyncEventArgs async1 = new SocketAsyncEventArgs();
            //SocketAsyncEventArgs async2 = new SocketAsyncEventArgs();
            //async1.AcceptSocket = player1.NetPlayer;
            //async2.AcceptSocket = player2.NetPlayer;
            //async1.SetBuffer(enc.GetBytes(dataP2 + dataBall),0,0);
            //async2.SetBuffer(enc.GetBytes(dataP1 + dataBall), 0, 0);

            
            
            
        }
    }
}
