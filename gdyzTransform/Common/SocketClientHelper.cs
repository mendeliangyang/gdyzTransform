﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace gdyzTransform.Common
{
    public class SocketClientHelper
    {
        public byte[] DealOnce(byte[] byteMsg)
        {
            //
            Socket socketClient = null;
            List<byte> listByte = new List<byte>();
            try
            {
                IPAddress ipAddress = IPAddress.Parse(gdyzTransform.Properties.Settings.Default.ServiceIp);
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //set timeout
                socketClient.ReceiveTimeout = 5000;
                socketClient.SendTimeout = 5000;
                //connect
                socketClient.Connect(new IPEndPoint(ipAddress, gdyzTransform.Properties.Settings.Default.ServicePort));
                //send
                socketClient.Send(byteMsg);
                //receive
                int receiveLen = 0, receiveTotal = 0;
                byte[] byteResult = new byte[512];
                int available = socketClient.Available;
                do
                {
                    // 如果客户端Socket关闭，_Client会接受到receiveLength=0
                    receiveLen = socketClient.Receive(byteResult);
                    receiveTotal += receiveLen;
                    if (receiveLen > 0)
                    {
                        listByte.AddRange(byteResult);
                    }
                    Array.Clear(byteResult, 0, byteResult.Length);

                } while (receiveLen > 0 && receiveTotal < available);
                byte[] byteRet = new byte[receiveTotal];
                Array.Copy(listByte.ToArray(), 0, byteRet, 0, receiveTotal);
                return byteRet;
            }

            finally
            {
                if (socketClient != null && socketClient.Connected)
                {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                }
            }

        }
    }
}