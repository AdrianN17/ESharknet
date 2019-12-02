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


        /// <summary>
        /// Inicializate server
        /// max_channel default value is 0 and mean all channel, required
        /// timeout default value is 15, required
        /// </summary>
        /// <param name="ip_address">IP used for server </param>
        /// <param name="port">Port used for server</param>
        /// <param name="max_clients">Max clients in server</param>
        /// <param name="max_channel">Max channel used in server - Opcional</param>
        /// <param name="timeout">Max time to client's response time</param>

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

        /// <summary>
        /// Update server
        /// </summary>
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

        /// <summary>
        /// Send packet to clients
        /// encode false is used in the case the data bounces to another Peer
        /// channel 0 is all channel
        /// </summary>
        /// <param name="event_name">Name for event </param>
        /// <param name="data_value">Object Data to send </param>
        /// <param name="peer">Peer or client to send </param>
        /// <param name="encode">Is neccesary encode the data to send? Default true</param>
        /// <param name="channel">Channel used to send, Default 0</param>
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


        /// <summary>
        /// Send packet to clients except to especific peer
        /// encode false is used in the case the data bounces to another Peer
        /// </summary>
        /// <param name="event_name">Name for event </param>
        /// <param name="data_value">Object Data to send </param>
        /// <param name="peer">Peer or client to exclude </param>
        /// <param name="encode">Is neccesary encode the data to send? Default true</param>
        /// <param name="channel">Channel used to send, Default 0</param>
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

        /// <summary>
        /// Send packet to all clients
        /// encode false is used in the case the data bounces to another Peer
        /// </summary>
        /// <param name="event_name">Name for event </param>
        /// <param name="data_value">Object Data to send </param>
        /// <param name="encode">Is neccesary encode the data to send? Default true</param>
        /// <param name="channel">Channel used to send, Default 0</param>
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

        /// <summary>
        /// Send packet to client - Peer
        /// encode false is used in the case the data bounces to another Peer
        /// </summary>
        /// <param name="event_name">Name for event </param>
        /// <param name="data_value">Object Data to send </param>
        /// <param name="peer">Peer or client to send </param>
        /// <param name="encode">Is neccesary encode the data to send? Default true</param>
        /// <param name="channel">Channel used to send, Default 0</param>
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

        /// <summary>
        /// Send packet to client - Peer by index
        /// encode false is used in the case the data bounces to another Peer
        /// </summary>
        /// <param name="event_name">Name for event </param>
        /// <param name="data_value">Object Data to send </param>
        /// <param name="index">Peer or client index to send </param>
        /// <param name="encode">Is neccesary encode the data to send? Default true</param>
        /// <param name="channel">Channel used to send, Default 0</param>
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

        /// <summary>
        /// Get Peers List
        /// </summary>
        /// <returns>Peers List</returns>
        public List<Peer> GetListClients()
        {
            return clients;
        }

        /// <summary>
        /// Get Peers List count
        /// </summary>
        /// <returns>Peers List count</returns>
        public int GetListClientsCount()
        {
            return clients.Count;
        }

        /// <summary>
        /// Add new Peer
        /// </summary>
        /// <returns>index of Peer</returns>
        public int AddPeer(ENet.Event net_event)
        {
            clients.Add(net_event.Peer);
            int index = clients.IndexOf(net_event.Peer);
            return index;
        }

        /// <summary>
        /// Remove new Peer
        /// </summary>
        /// <returns>index of Peer</returns>
        public int RemovePeer(ENet.Event net_event)
        {
            //Debug.LogError("Client delete");

            int index = clients.IndexOf(net_event.Peer);
            clients.Remove(net_event.Peer);
            return index;
        }

        /// <summary>
        /// ENet Server Callbacks 
        /// </summary>
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

        /// <summary>
        /// Disconnect all peers
        /// </summary>
        public void DisconnectAllPeer()
        {
            foreach(Peer peer in clients)
            {
                peer.DisconnectNow(0);
            }
        }

        /// <summary>
        /// Destroy Server
        /// </summary>
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
