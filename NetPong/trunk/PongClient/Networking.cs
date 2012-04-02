using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace PongClient
{
    class Networking
    {
        //constants
        private const int buffLength = 256;
        private const int port = 8001;

        //instance vars
        private string recievedData = "";
        private Queue<string> toSendBuffer = new Queue<string>(); //When you tell this object to send something it will get added to the queue to be sent.
        private IPAddress address;
        private Socket netPlayer;
        private byte[] buffer = new byte[buffLength];
        private int player;
        private bool playerSet;
        private GameObjects gObjects;

        //Properties
        //This allows the program to get the raw recived string
        public string RecievedData
        {
            get
            {
                return recievedData;
            }
        }

        //Gets the player number
        public int Player
        {
            get
            {
                return player;
            }
        }

        //set to this to add to the sending buffer (Queue)
        public string ToSend
        {
            set
            {
                toSendBuffer.Enqueue(value);
            }
        }

        public GameObjects GObjects
        {
            get
            {
                return gObjects;
            }
        }

        //end properties

        //ctor
        public Networking(string ipAddress, GameObjects gObj)
        {
            
            IPAddress.TryParse(ipAddress, out address);
            netPlayer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            netPlayer.BeginConnect(new IPEndPoint(address, port), OnConnect, netPlayer);
            SetupRecieveCallback();
            SetupSendCallback();
            gObjects = gObj;
            //Ack packet
            //ToSend = "C";
        }

        //Sets up the recieving thread
        //You must call this before starting the game
        private void SetupRecieveCallback()
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                netPlayer.BeginReceive(buffer, 0, buffLength, SocketFlags.None, recieveData, netPlayer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed!");
            }
        }

        //Sets up the sending thread
        //You must call this before starting the game
        private void SetupSendCallback()
        {
            byte[] buffer;
            try
            {
                if (toSendBuffer.Count == 0)
                {
                    buffer = ASCIIEncoding.ASCII.GetBytes("");
                }
                else
                {
                    buffer = ASCIIEncoding.ASCII.GetBytes(toSendBuffer.Dequeue());
                }
                AsyncCallback sendData = new AsyncCallback(OnSentData);
                netPlayer.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, sendData, netPlayer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send callback setup failed!");
            }
        }

        //This is called when a connection has been made
        private void OnConnect(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            // Check if we were sucessfull
            try
            {
                if (sock.Connected)
                    SetupRecieveCallback();
                else
                    Console.WriteLine("Error connecting to server");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unusual error during Connection!");
            }
        }

        //This gets called whenever new data is recieved through a socket
        private void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Write the data
                    recievedData = Encoding.ASCII.GetString(buffer, 0, nBytesRec);
                    //The server announce what player we are untill we send the 'Ack' packet above, set the player here
                    if (playerSet == false)
                    {
                        SetPlayer();
                        playerSet = true;
                    }
                    GetRecievedData();
                    SetupRecieveCallback();
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("Server connection lost!");
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unusual error during Recieve!");
            }
        }


        //This gets called whenever data is successfully sent through a socket
        private void OnSentData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int sent = netPlayer.EndSend(ar);
            try
            {
                //Thread.Sleep(100);
                SetupSendCallback();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unusual error during sending!");
            }
        }

        //T
        private void GetRecievedData()
        {
            //I am terrible at regex, there is probabally a better expression than this
            Regex r = new Regex(@"[^0-9\.]+");
            String[] tmpSplit;
            double xPos;
            double yPos;
            double x2Pos;
            double y2Pos;
            double ballXPos;
            double ballYPos;
            int score;
            int score2;
            tmpSplit = r.Split(recievedData.Trim());
            Double.TryParse(tmpSplit[0], out xPos);
            Double.TryParse(tmpSplit[1], out yPos);
            Int32.TryParse(tmpSplit[2], out score);
            Double.TryParse(tmpSplit[3], out x2Pos);
            Double.TryParse(tmpSplit[4], out y2Pos);
            Int32.TryParse(tmpSplit[5], out score2);
            Double.TryParse(tmpSplit[6], out ballXPos);
            Double.TryParse(tmpSplit[7], out ballYPos);
            gObjects.Paddle1XPos = xPos;
            gObjects.Paddle1YPos = yPos;
            gObjects.Paddle2XPos = x2Pos;
            gObjects.Paddle2YPos = y2Pos;
            gObjects.Player1Score = score;
            gObjects.Player2Score = score2;
            gObjects.BallXPos = ballXPos;
            gObjects.BallYPos = ballYPos;
            

        }

        private void SetPlayer()
        {
            Int32.TryParse(recievedData, out player);
        }
    }
}
