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
        private Socket server;
        private ASCIIEncoding enc = new ASCIIEncoding();
        private const int port = 8001;
        private IPAddress address;
        //end Network stuff

        //Game vars
        //player 1
        //players
        List<Player> players = new List<Player>();
        int numOfPlayers = 0;
        const int maxNumOfPlayers = 2;
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
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(address, port));
                server.Listen(5);
                server.BeginAccept(new AsyncCallback(OnConnectRequest), server);


                
                Console.ReadLine(); 
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

        public void OnConnectRequest(IAsyncResult ar)
        {

            Socket listener = (Socket)ar.AsyncState;
            Player tmpPlayer = new Player();
            tmpPlayer.NetPlayer = server.EndAccept(ar);    
            if (numOfPlayers >= 2)
            {
                byte[] reply = enc.GetBytes("Sorry, server is full!");
                tmpPlayer.NetPlayer.Send(reply);
                Console.WriteLine("A Player {0}, joined but the server is full!", tmpPlayer.NetPlayer.RemoteEndPoint);
                tmpPlayer.NetPlayer.Close();
            }
            else
            {
                players.Add(tmpPlayer);
                numOfPlayers++;
                players[numOfPlayers - 1].PlayerNum = numOfPlayers;
                Console.WriteLine("Player {0}: {1}, joined", players[numOfPlayers - 1].PlayerNum, players[numOfPlayers - 1].NetPlayer.RemoteEndPoint);
            }
            


            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
        }

        internal void StartGame()
        {
            //TODO: Make a better loop
            //while (true)
           // {
          //      Update();
           //     Thread.Sleep(10);
           // }
            
        }

        private void Update()
        {
            
            //String dataP1 = String.Format("{0} {1} {2} ", player1.Xpos, player1.Ypos, player1.Score);
            //String dataP2 = String.Format("{0} {1} {2} ", player2.Xpos, player2.Ypos, player2.Score);
           // String dataBall = String.Format("{0} {1} {2} {3}", ball.Xpos, ball.Ypos, ball.Angle, ball.Speed);
           // player1.NetPlayer.Send(enc.GetBytes(dataP2 + dataBall));
           // player2.NetPlayer.Send(enc.GetBytes(dataP1 + dataBall));
            //SocketAsyncEventArgs async1 = new SocketAsyncEventArgs();
            //SocketAsyncEventArgs async2 = new SocketAsyncEventArgs();
            //async1.AcceptSocket = player1.NetPlayer;
            //async2.AcceptSocket = player2.NetPlayer;
            //async1.SetBuffer(enc.GetBytes(dataP2 + dataBall),0,0);
            //async2.SetBuffer(enc.GetBytes(dataP1 + dataBall), 0, 0);

            
            
            
        }

        
        
    }
}
