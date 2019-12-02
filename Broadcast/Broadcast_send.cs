using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Libs.Esharknet.Model;
using Assets.Libs.Esharknet.Serialize;

namespace Assets.Libs.Esharknet.Broadcast
{
    public class Broadcast_send: Serialize_Class
    {

        public UdpClient udpServer;
        public Thread thread;

        private Data_broadcast broadcast_data;

        private bool loop = true;

        /// <summary>
        /// Inicializate Broadcast to send data
        /// </summary>
        /// <param name="ip_address">IP to send</param>
        /// <param name="port">Port to send</param>
        /// <param name="port_send">Destination Port</param>
        /// <param name="timedelay">Frecuency of time to every send</param>
        /// <param name="broadcast_data">Data to send</param>
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

        /// <summary>
        /// Destroy Broadcast Send
        /// </summary>
        public void Destroy()
        {
            //Debug.LogWarning("Broadcast send finish");

            loop = false;

            udpServer.Close();

        }
    }
}
