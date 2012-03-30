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
        public Socket NetPlayer { get; set; }
        private const int buffLength = 256;
        private byte[] buffer = new byte[buffLength];
        public string recievedData = "";
        //private string toSend = "";
        private Queue<string> toSendBuffer = new Queue<string>();
        public string ToSend
        {
            set { toSendBuffer.Enqueue(value); }
        }
       
        //public string toSendOld = "";

        public void SetupRecieveCallback()
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                NetPlayer.BeginReceive(buffer, 0, buffLength, SocketFlags.None, recieveData, NetPlayer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed for player {0}", PlayerNum);
            }
        }

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
                
                NetPlayer.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, sendData, NetPlayer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send callback setup failed for player {0}", PlayerNum);
            }
        }

        public void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Write the data to the List
                    recievedData = Encoding.ASCII.GetString(buffer, 0, nBytesRec);


                    SetupRecieveCallback();
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("Player {0}, disconnected", PlayerNum);
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unusual error during Recieve!");
            }
        }

        public void OnSentData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int sent = NetPlayer.EndSend(ar);
            try
            {
                SetupSendCallback();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unusual error during sending!");
            }
        }
    }
}
