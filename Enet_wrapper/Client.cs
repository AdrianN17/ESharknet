using System;
using ENet;
using System.Runtime.InteropServices;

namespace Assets.Libs.Esharknet
{
    public class Client: BaseClass
    {
        private Host client;
        private Peer peer;

        /// <summary>
        /// Inicializate Client
        /// max_channel default value is 0 and mean all channel, required
        /// timeout default value is 15, required
        /// </summary>
        /// <param name="ip_address">IP used for server </param>
        /// <param name="port">Port used for server</param>
        /// <param name="channel">Max channel used in server - Opcional</param>
        /// <param name="timeout">Max time to client's response time</param>
        public Client(string ip_address, ushort port, int channel, int timeout)
        {

            AllocCallback OnMemoryAllocate = (size) => {
                return Marshal.AllocHGlobal(size);
            };

            FreeCallback OnMemoryFree = (memory) => {
                Marshal.FreeHGlobal(memory);
            };

            NoMemoryCallback OnNoMemory = () => {
                throw new OutOfMemoryException();
            };

            Callbacks callbacks = new Callbacks(OnMemoryAllocate, OnMemoryFree, OnNoMemory);

            if (ENet.Library.Initialize(callbacks))
                //Debug.LogWarning("ENet successfully initialized using a custom memory allocator");

            client = new Host();

            Address address = new Address();

            address.Port = port;
            address.SetHost(ip_address);

            client.Create();
            client.EnableCompression();

            peer = client.Connect(address);

        }

        /// <summary>
        /// Update client
        /// </summary>
        public void update()
        {
           
            bool polled = false;

            ENet.Event netEvent;

            while (!polled)
            {
                if (client.CheckEvents(out netEvent) <= 0)
                {
                    if (client.Service(timeout, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                switch_callbacks(netEvent);
            }
 
        }

        /// <summary>
        /// Send packet to clients
        /// encode false is used in the case the data bounces to Server Peer
        /// channel 0 is all channel
        /// </summary>
        /// <param name="event_name">Name for event </param>
        /// <param name="data_value">Object Data to send </param>
        /// <param name="encode">Is neccesary encode the data to send? Default true</param>
        /// <param name="channel">Channel used to send, Default 0</param>
        public void Send(string event_name, dynamic data_value, bool encode = true, int channel = 0)
        {
            ENet.Packet packet;

            if (encode)
            {
                packet = Encode(new Data(event_name, data_value));
            }
            else
            {
                packet = data_value;
            }

            peer.Send((byte)channel, ref packet);
        }

        /// <summary>
        /// ENet Clients Callbacks 
        /// </summary>
        void switch_callbacks(ENet.Event netEvent)
        {
            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    //Debug.Log("Client connected to server");
                    ExecuteTrigger("Connect", netEvent);

                    break;

                case ENet.EventType.Disconnect:
                    //Debug.Log("Client disconnected from server");
                    ExecuteTrigger("Disconnect", netEvent);

                    break;

                case ENet.EventType.Timeout:
                    //Debug.Log("Client connection timeout");
                    ExecuteTrigger("Timeout", netEvent);

                    break;

                case ENet.EventType.Receive:
                    //Debug.Log("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                    ExecuteTriggerBytes(netEvent);
                    netEvent.Packet.Dispose();
                    break;

            }
        }

        /// <summary>
        /// Calculate time in miliseconds
        /// </summary>
        /// <returns>get ping </returns>
        public uint RountTripTimer()
        {
            return peer.RoundTripTime;
        }

        /// <summary>
        /// Destroy Client
        /// </summary>
        public void Destroy()
        {
            peer.Disconnect(0);

            client.Flush();
            ENet.Library.Deinitialize();
            //Debug.LogWarning("Client finish");
        }
    }
}
