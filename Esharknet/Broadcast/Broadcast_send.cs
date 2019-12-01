using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assets.Libs.Esharknet.Model;
using Assets.Libs.Esharknet.Serialize;
using UnityEngine;

namespace Assets.Libs.Esharknet.Broadcast
{
    public class Broadcast_send: Serialize_Class
    {

        public UdpClient udpServer;
        public Thread thread;

        private Data_broadcast broadcast_data;

        private bool loop = true;

        public Broadcast_send(string ip_address,ushort port, ushort port_send, int timedelay, Data_broadcast broadcast_data)
        {
            udpServer = new UdpClient();

            udpServer.Client.Bind(new IPEndPoint(IPAddress.Parse(ip_address), port));

            this.broadcast_data = broadcast_data;

            var data = new Data("broadcast", this.broadcast_data);

            thread = new Thread(delegate ()
            {
                while (loop)
                {

                    var byte_data = Serialize(data);


                    udpServer.Send(byte_data, byte_data.Length, "255.255.255.255", port_send);

                    Thread.Sleep(timedelay);

                }
            });

            thread.Start();

        }

        public void Destroy()
        {
            Debug.LogWarning("Broadcast send finish");

            loop = false;

            udpServer.Close();

        }
    }
}
