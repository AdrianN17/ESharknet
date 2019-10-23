using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ENet;

namespace Assets.Libs.Esharknet
{
    public class Client: BaseClass
    {
        private Host client;
        private Peer peer;


        public Client(string ip_address, ushort port, int chanel, int timeout)
        {
            ENet.Library.Initialize();
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
            ENet.Event netEvent;

            if (client.CheckEvents(out netEvent) <= 0)
            {
                if (client.Service(timeout, out netEvent) <= 0)
                {
                    return;
                }

                switch (netEvent.Type)
                {
                    case ENet.EventType.None:
                        break;

                    case ENet.EventType.Connect:
                        Debug.Log("Client connected to server");
                        ExecuteTrigger("Connect", netEvent);

                        break;

                    case ENet.EventType.Disconnect:
                        Debug.Log("Client disconnected from server");
                        ExecuteTrigger("Disconnect", netEvent);

                        break;

                    case ENet.EventType.Timeout:
                        Debug.Log("Client connection timeout");
                        ExecuteTrigger("Timeout", netEvent);

                        break;

                    case ENet.EventType.Receive:
                        Debug.Log("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                        ExecuteTriggerBytes(netEvent);
                        netEvent.Packet.Dispose();
                        break;
                }
            }
        }

        public void Send(string event_name, dynamic data_value, bool Encode = true, int channel = 0)
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

        public void Destroy()
        {
            peer.Disconnect(0);
            client.Flush();
            ENet.Library.Deinitialize();
            Debug.LogWarning("Client finish");
        }
    }
}
