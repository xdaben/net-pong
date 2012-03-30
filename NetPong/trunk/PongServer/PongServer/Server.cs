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
        //Timer timer;



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
            //address = IPAddress.Parse("127.0.0.1");
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


                
                //Console.ReadLine(); 
            }
            catch (Exception e)
            {

                Console.WriteLine("Error starting server: {0}", e.Message);
            }

            //set up game vars

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
                tmpPlayer.ToSend = String.Format("You are player {0}", thisPlayer);
                Console.WriteLine("Player {0}: {1}, joined", tmpPlayer.PlayerNum, players[thisPlayer - 1].NetPlayer.RemoteEndPoint);
                tmpPlayer.SetupRecieveCallback();
                tmpPlayer.SetupSendCallback();
                //tmpPlayer.logic = new GameLogic(tmpPlayer);
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
                
            }
        }
        

        

        internal void StartGame()
        {
            while (isReady == false)
            {
                //wait until the game is ready
            }
            
            updateThread = new Thread(new ThreadStart(Update));
            updateThread.Start();

        }

        private void Update()
        {

            //Timer logicTimer = new Timer(new TimerCallback(Update), null, 0, 10);
            //Regex r = new Regex(@"\d+");



            //List<string> playerDataToSend = new List<string>();
            while (endGame == false)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Player p in players)
                {
                    sb.Append(p.Xpos + " ");
                    sb.Append(p.Ypos + " ");
                    sb.Append(p.Score + " ");
                }

                foreach (Player p in players)
                {
                    
                    p.ToSend = sb.ToString();
                }
                Thread.Sleep(100);
            }
            updateThread.Abort();
            server.Close();

            
            
        }

        
        
    }
}
