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
            

                Console.WriteLine("Player {0}: {1}, joined", tmpPlayer.PlayerNum, players[thisPlayer - 1].NetPlayer.RemoteEndPoint);
                tmpPlayer.SetupRecieveCallback();
                tmpPlayer.SetupSendCallback();
                while (tmpPlayer.recievedData != "C")
                {
                    tmpPlayer.ToSend = String.Format("{0}", thisPlayer);
                    Thread.Sleep(100);
                }
                
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
            
            players[0].Xpos = 15;
            players[0].Ypos = 150;
            players[1].Xpos = 560;
            players[1].Ypos = 150;
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
                //This section is written to assume that there is only 2 players! (which in this case works)
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} {1} {2}", players[1].Xpos, players[1].Ypos, players[1].Score);
                players[0].ToSend = sb.ToString();

                sb.AppendFormat("{0} {1} {2}", players[0].Xpos, players[0].Ypos, players[0].Score);
                players[1].ToSend = sb.ToString();
                Thread.Sleep(100);
            }
            updateThread.Abort();
            server.Close();

            
            
        }

        
        
    }
}
