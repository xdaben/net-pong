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
        public string toSend = "";
        public string toSendOld = "";

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
            try
            {
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(toSend);
                AsyncCallback sendData = new AsyncCallback(OnSentData);
                while (toSendOld.Equals(toSend))
                {
                    
                }
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
            NetPlayer.EndSend(ar);
            toSendOld = toSend;
            System.Threading.Thread.Sleep(500);
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
