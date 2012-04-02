using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

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
        public int numOfPlayers = 0;
        public const int maxNumOfPlayers = 2;
        //ball
        Ball ball = new Ball();
        //end Game vars

        //Misc vars
        bool endGame;
        Thread updateThread;
        bool isReady;
        GameLogic logic;
       



        public Server()
        {
            //Grabs the LAN ip(v4) address of the server (may not work well if there are multiple network interfaces)
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ip = (
                      from addr in hostEntry.AddressList
                      where addr.AddressFamily == AddressFamily.InterNetwork
                      select addr
            ).FirstOrDefault();

            
            address = ip;
        }

        internal void Init()
        {
            
            Console.WriteLine("Welcome to the pong server!");
            Console.WriteLine("Your IP address is {0}.",address);
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(address, port));
                server.Listen(5);
                server.BeginAccept(new AsyncCallback(OnConnectRequest), server);


                
                 
            }
            catch (Exception e)
            {

                Console.WriteLine("Error starting server: {0}", e.Message);
            }

            

        }

        public void OnConnectRequest(IAsyncResult ar)
        {

            Socket listener = (Socket)ar.AsyncState;
            Player tmpPlayer = new Player();
            tmpPlayer.NetPlayer = server.EndAccept(ar);    
            if (numOfPlayers >= maxNumOfPlayers)
            {
                tmpPlayer.ToSend = "Sorry, server is full!";
                Console.WriteLine("A Player {0}, joined but the server is full!", tmpPlayer.NetPlayer.RemoteEndPoint);
                tmpPlayer.NetPlayer.Close();
            }
            else
            {
                players.Add(tmpPlayer);
                int thisPlayer = ++numOfPlayers;
                tmpPlayer.PlayerNum = thisPlayer;
            

                Console.WriteLine("Player {0}: {1}, joined", tmpPlayer.PlayerNum, players[thisPlayer - 1].NetPlayer.RemoteEndPoint);
                tmpPlayer.SetupRecieveCallback();
                tmpPlayer.SetupSendCallback();
                while (tmpPlayer.recievedData != "C")
                {
                    tmpPlayer.ToSend = String.Format("{0}", thisPlayer);
                    Thread.Sleep(100);
                }
                
                
                IsReady();

            }
            


            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
        }


        private void IsReady()
        {
            if (numOfPlayers < maxNumOfPlayers)
            {
                foreach (Player p in players)
                {
                    p.ToSend = String.Format("Waiting for {0} more player(s)\n", maxNumOfPlayers - numOfPlayers);
                }
            }
            else
            {
                isReady = true;
                logic = new GameLogic(players);
                
            }
        }
        

        

        internal void StartGame()
        {
            while (isReady == false)
            {
                //wait until the game is ready
            }
            
            players[1].XPos = 15;
            players[1].YPos = 150;
            players[1].Score = 0;
            players[0].XPos = 560;
            players[0].YPos = 150;
            players[0].Score = 0;
            updateThread = new Thread(new ThreadStart(Update));
            updateThread.Start();

        }

        private void Update()
        {

            
            StringBuilder sb = new StringBuilder();
            const int round = 3;
            while (endGame == false)
            {
                foreach (Player p in players)
                {
                    sb.AppendFormat("{0} {1} {2} ", Math.Round(p.XPos,round), Math.Round(p.YPos,round), p.Score);
                }
                sb.AppendFormat("{0} {1}", Math.Round(logic.ball.XPos,round), Math.Round(logic.ball.YPos,round));
                foreach (Player p in players)
                {
                    p.ToSend = sb.ToString();
                }
                sb.Clear();
                Thread.Sleep(100);
                logic.MovePaddle(players[0]);
                logic.MovePaddle(players[1]);
                logic.MoveBall(logic.ball);
            }

            
            updateThread.Abort();
            server.Close();

            
            
        }

        
        
    }
}
