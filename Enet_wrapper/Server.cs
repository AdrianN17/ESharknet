using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENet;
using UnityEngine;

namespace Assets.Libs.Esharknet
{
    public class Server: BaseClass
    {
        private Host server;
        private List<Peer> clients;

        
        public Server(string ip_address, ushort port, int max_clients, int max_channel, int timeout)
        {
            ENet.Library.Initialize();

            clients = new List<Peer>();

            //init server

            Address address = new Address();
            address.SetHost(ip_address);
            address.Port = port;

            server = new Host();
            server.Create(address, max_clients, max_channel);
            server.EnableCompression();

            this.timeout = timeout;

            Debug.Log("Create server IP : " + ip_address);

            /*TriggerFunctions.Add("Connect", delegate(ENet.Event net_event) {
                AddPeer(net_event);
            });*/

            TriggerFunctions.Add("Disconnect", delegate (ENet.Event net_event) {
                RemovePeer(net_event);
            });

            TriggerFunctions.Add("Timeout", delegate (ENet.Event net_event) {
                RemovePeer(net_event);
            });

        }

        public void update()
        {
            ENet.Event netEvent;

            if (server.CheckEvents(out netEvent) <= 0)
            {
                if (server.Service(timeout, out netEvent) <= 0)
                {
                    return;
                }
            }

            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    Debug.Log("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                    ExecuteTrigger("Connect", netEvent);
                    break;

                case ENet.EventType.Disconnect:
                    Debug.Log("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                    ExecuteTrigger("Disconnect", netEvent);
                    break;

                case ENet.EventType.Timeout:
                    Debug.Log("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                    ExecuteTrigger("Timeout", netEvent);
                    break;

                case ENet.EventType.Receive:
                    Debug.Log("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                    byte[] buffer = new byte[1024];
                    ExecuteTriggerBytes(netEvent);
                    netEvent.Packet.Dispose();
                    break;
            }
        }

        public void Send(string event_name, dynamic data_value, Peer peer, bool Encode = true,int channel=0)
        {
            ENet.Packet packet;

            if (Encode)
            {
                packet = JSONEncode(new Data(event_name, data_value));
            }
            else
            {
                packet = data_value;
            }

            peer.Send((byte)channel, ref packet);
        }

        public void SendToAllBut(string event_name, dynamic data_value, Peer peer, bool Encode=true, int channel = 0)
        {
            ENet.Packet packet;

            if (Encode)
            { 
                packet = JSONEncode(new Data(event_name, data_value));
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

        public void SendToAll(string event_name, dynamic data_value, bool Encode = true, int channel = 0)
        {
            ENet.Packet packet;

            if (Encode)
            { 
                packet = JSONEncode(new Data(event_name, data_value));
            }
            else
            {
                packet = data_value;
            }

            server.Broadcast((byte)channel, ref packet);
        }

        public void SendToPeer(string event_name, dynamic data_value, Peer peer, bool Encode = true,int channel=0)
        {
            ENet.Packet packet;

            if (Encode)
            {
                packet = JSONEncode(new Data(event_name, data_value));
            }
            else
            {
                packet = data_value;
            }

            peer.Send((byte)channel, ref packet);
        }

        public void SendToPeerIndex(string event_name, dynamic data_value, int index, bool Encode = true, int channel = 0)
        {
            ENet.Packet packet;

            if (Encode)
            {
                packet = JSONEncode(new Data(event_name, data_value));
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
            clients.Remove(net_event.Peer);
            int index = clients.IndexOf(net_event.Peer);
            return index;
        }

        public void Destroy()
        {
            clients.Clear();
            server.Flush();
            ENet.Library.Deinitialize();
            Debug.LogWarning("Server finish");
        }
    }  
}
