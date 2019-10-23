using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assets.Libs.Esharknet.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Libs.Esharknet.Broadcast
{
    public class Broadcast_send
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

                    var json_data  = JsonConvert.SerializeObject(data);

                    Debug.Log("Broadcast Send : "+ json_data);

                    var bytes = Encoding.ASCII.GetBytes(json_data);

                    udpServer.Send(bytes, bytes.Length, "255.255.255.255", port_send);

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
