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

        
        public Server(string ip_address, ushort port, int max_clients, int chanel, int timeout)
        {
            ENet.Library.Initialize();

            clients = new List<Peer>();

            //init server

            Address address = new Address();
            address.SetHost(ip_address);
            address.Port = port;

            server = new Host();
            server.Create(address, max_clients, 0);
            server.EnableCompression();

            this.timeout = timeout;

            Debug.Log("Create server IP : " + ip_address);

            TriggerFunctions.Add("Connect", delegate(ENet.Event net_event) {
                AddPeer(net_event);
            });

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

                    ExecuteTriggerBytes(netEvent);
                    netEvent.Packet.Dispose();
                    break;
            }
        }

        

        public void SendToAllBut(string event_name, dynamic data_value, Peer peer)
        {
            var packet = JSONEncode(new Data(event_name, data_value));
            foreach(var client in clients)
            {
                if (!clients.Equals(peer))
                {
                    client.Send(0, ref packet);
                }
            }
        }

        public void SendToAll(string event_name, dynamic data_value)
        {
            var packet = JSONEncode(new Data(event_name,data_value));
            server.Broadcast(0, ref packet);
        }

        public void SendToPeer(string event_name, dynamic data_value, Peer peer)
        {
            var packet = JSONEncode(new Data(event_name, data_value));
            peer.Send(0,ref packet);
        }

        public void SendToPeerIndex(string event_name, dynamic data_value, int index)
        {
            var packet = JSONEncode(new Data(event_name, data_value));
            clients[index].Send(0, ref packet);
        }

        public List<Peer> GetListClients()
        {
            return clients;
        }

        public int GetListClientsCount()
        {
            return clients.Count;
        }

        public void AddPeer(ENet.Event net_event)
        {
            clients.Add(net_event.Peer);
        }

        public void RemovePeer(ENet.Event net_event)
        {
            clients.Remove(net_event.Peer);
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
