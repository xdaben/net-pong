using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace PongThreaded
{
    class Networking
    {
        //constants
        private const int buffLength = 256; //NOTE: if not really necessary to change unless it causes lag, just make sure it is big enough to hold the sent data.
        //instance vars
        private string recievedData = "";
        private Queue<string> toSendBuffer = new Queue<string>(); //When you tell this object to send something it will get added to the queue to be sent.
        private IPAddress address;
        private Socket netPlayer;
        private byte[] buffer = new byte[buffLength];
        private int player;
        private bool playerSet;
        //Properties
        //This allows the program to get the raw recived string
        public string RecievedData
        {
            get
            {
                return recievedData;
            }
        }

        public int Player
        {
            get
            {
                return player;
            }
        }

        //No need to get the buffer
        public string ToSend
        {
            set
            {
                toSendBuffer.Enqueue(value);
            }
        }

        //end properties

        //ctor
        public Networking(string ipAddress)
        {
            bool success;
            success = IPAddress.TryParse(ipAddress, out address);
            netPlayer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            netPlayer.BeginConnect(new IPEndPoint(address,8001), OnConnect, netPlayer);
            SetupRecieveCallback();
            SetupSendCallback();
            ToSend = "C";

        }

        //Sets up the recieving thread
        //You must call this before starting the game
        //TODO: Consider calling this in the constructor
        public void SetupRecieveCallback()
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
        //TODO: Consider calling this in the constructor
        public void SetupSendCallback()
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

        public void OnConnect(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;
            
            // Check if we were sucessfull
            try
            {
                //    sock.EndConnect( ar );
                if (sock.Connected)
                    SetupRecieveCallback();
                else
                    Console.WriteLine("Error connecting to server");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unusual error during Connect!");
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
                    if (playerSet == false)
                    {
                        SetPlayer();
                        playerSet = true;
                    }
                    //ConvertRecievedData();

                    SetupRecieveCallback();
                }
                else
                {
                    // If no data was recieved then the connection is probably dead, this might need to be changed...
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

        //Call this to get the recieved data already processed, as out X, Y, Score
        public bool GetData(out double xPos, out double yPos, out double score)
        {
            //I am terrible at regex, there is probabally a better expression than this
            Regex r = new Regex(@"[^0-9\.]+");
            String[] tmp;
            tmp = r.Split(recievedData);
            double outDbl;
            int outInt;
            double.TryParse(tmp[0], out outDbl);
            yPos = outDbl;
            double.TryParse(tmp[1], out outDbl);
            xPos = outDbl;
            Int32.TryParse(tmp[2], out outInt);
            score = outInt;
            return true; //TODO: Get error checking in here
        }

        private void SetPlayer()
        {
            Int32.TryParse(recievedData,out player);
        
                
                
        }
    }
}
