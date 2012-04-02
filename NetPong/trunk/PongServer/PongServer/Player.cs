﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace PongServer
{
    public class Player
    {
        private double speed;
        private int height = 80;



        //Networking vars
        public Socket NetPlayer { get; set; }
        private const int buffLength = 256;
        private byte[] buffer = new byte[buffLength];
        public string recievedData = "";
        private Queue<string> toSendBuffer = new Queue<string>();
        //Properties
        public int PlayerNum { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
        public int Direction { get; set; }
        public int Score { get; set; }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (Math.Abs(value) <= 7)
                    speed = value;
            }
        }

        public string ToSend
        {
            set
            {
                toSendBuffer.Enqueue(value);
            }
        }

      

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
                    if (recievedData != "")
                    {
                        ConvertRecievedData();
                    }

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

        private void ConvertRecievedData()
        {
            //What did I just write :/
            //Gotta love regex autocompletion
            Regex r = new Regex(@"[^0-9\.]+");
            String[] tmp;
            tmp = r.Split(recievedData);
            double outDbl;
            int outInt;
            double.TryParse(tmp[0], out outDbl);
            YPos = outDbl;
            double.TryParse(tmp[1], out outDbl);
            XPos = outDbl;
            Int32.TryParse(tmp[2], out outInt);
            Score = outInt;
        }
    }
}
