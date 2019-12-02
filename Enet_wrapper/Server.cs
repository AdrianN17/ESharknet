using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ENet;

namespace Assets.Libs.Esharknet
{
    public class Server: BaseClass
    {
        private Host server;
        private List<Peer> clients;


        public Server(string ip_address, ushort port, int max_clients, int max_channel, int timeout)
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

            clients = new List<Peer>();

            //init server

            Address address = new Address();
            address.SetHost(ip_address);
            address.Port = port;

            server = new Host();
            server.Create(address, max_clients, max_channel);
            server.EnableCompression();

            this.timeout = timeout;

           // Debug.Log("Create server IP : " + ip_address);
        }

        public void update()
        {

            ENet.Event netEvent;

            bool polled = false;

            while (!polled)
            {

                if (server.CheckEvents(out netEvent) <= 0)
                {
                    if (server.Service(timeout, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                switch_callbacks(netEvent);
            }

        }

        public void Send(string event_name, dynamic data_value, Peer peer, bool encode = true,int channel=0)
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

        public void SendToAllBut(string event_name, dynamic data_value, Peer peer, bool encode = true, int channel = 0)
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


            foreach (var client in clients)
            {
                if (client.Equals(peer))
                {
                    
                }
                else
                {
                    client.Send((byte)channel, ref packet);
                }
            }
        }

        public void SendToAll(string event_name, dynamic data_value, bool encode = true, int channel = 0)
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

            server.Broadcast((byte)channel, ref packet);
        }

        public void SendToPeer(string event_name, dynamic data_value, Peer peer, bool encode = true,int channel=0)
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

        public void SendToPeerIndex(string event_name, dynamic data_value, int index, bool encode = true, int channel = 0)
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

            clients[index].Send((byte)channel, ref packet);
        }

        public List<Peer> GetListClients()
        {
            return clients;
        }

        public int GetListClientsCount()
        {
            return clients.Count;
        }

        public int AddPeer(ENet.Event net_event)
        {
            clients.Add(net_event.Peer);
            int index = clients.IndexOf(net_event.Peer);
            return index;
        }

        public int RemovePeer(ENet.Event net_event)
        {
            //Debug.LogError("Client delete");

            int index = clients.IndexOf(net_event.Peer);
            clients.Remove(net_event.Peer);
            return index;
        }

        void switch_callbacks(ENet.Event netEvent)
        {
            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    {
                        break;
                    }

                case ENet.EventType.Connect:
                    {
                        //Debug.Log("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        ExecuteTrigger("Connect", netEvent);
                        break;
                    }
                case ENet.EventType.Disconnect:
                    {
                        //Debug.Log("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        ExecuteTrigger("Disconnect", netEvent);
                        break;
                    }
                case ENet.EventType.Timeout:
                    {
                        //Debug.Log("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        ExecuteTrigger("Timeout", netEvent);
                        break;
                    }
                case ENet.EventType.Receive:
                    {
                        //Debug.Log("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                        ExecuteTriggerBytes(netEvent);
                        netEvent.Packet.Dispose();
                        break;
                    }
            }
        }

        public void DisconnectAllPeer()
        {
            foreach(Peer peer in clients)
            {
                peer.DisconnectNow(0);
            }
        }

        public void Destroy()
        {
            DisconnectAllPeer();

            server.Flush();
            server.Dispose();
            clients.Clear();

            ENet.Library.Deinitialize();
            //Debug.LogWarning("Server finish");
        }

        
    }  
}
