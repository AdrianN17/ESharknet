using System;
using ENet;
using System.Runtime.InteropServices;

namespace Assets.Libs.Esharknet
{
    public class Client: BaseClass
    {
        private Host client;
        private Peer peer;

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

        public uint RountTripTimer()
        {
            return peer.RoundTripTime;
        }

        public void Destroy()
        {
            peer.Disconnect(0);

            client.Flush();
            ENet.Library.Deinitialize();
            //Debug.LogWarning("Client finish");
        }
    }
}
