using System;
using System.Collections.Generic;
using System.Text;
using ENet;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Libs.Esharknet
{
    public class BaseClass
    {
        protected Dictionary<string, Action<ENet.Event>> TriggerFunctions;
        protected int timeout;

        public BaseClass()
        {
            this.TriggerFunctions = new Dictionary<string, Action<ENet.Event>>();
        }

        public void Send(string event_name, dynamic data_value, Peer peer)
        {
            var packet = JSONEncode(new Data(event_name,data_value));
            peer.Send(0, ref packet);
        }

        public ENet.Packet JSONEncode(Data data)
        {
            ENet.Packet packet = default(ENet.Packet);

            String json_value = JsonConvert.SerializeObject(data);
            Byte[] byte_data = Encoding.ASCII.GetBytes(json_value);
            packet.Create(byte_data);

            Debug.Log("Sending : " + json_value);


            return packet;
        }

        public Data JSONDecode(ENet.Packet packet_data)
        {
            byte[] buffer = new byte[1024];

            packet_data.CopyTo(buffer);
            string json_value = Encoding.ASCII.GetString(buffer);
            Data data = JsonConvert.DeserializeObject<Data>(json_value);
            return data;
        }

        protected void ExecuteTrigger(string key, ENet.Event netEvent)
        {
            if (TriggerFunctions.ContainsKey(key))
            {
                TriggerFunctions[key](netEvent);
            }
            else
            {
                Debug.LogWarning(key + " function not defined in dictionary");
            }
        }

        protected void ExecuteTriggerBytes(ENet.Event netEvent)
        {
            Data data = JSONDecode(netEvent.Packet);

            TriggerFunctions[data.key](netEvent);

        }

        public void AddTrigger(string key, Action<ENet.Event> function_calback)
        {
            TriggerFunctions.Add(key, function_calback);
        }
    }
}
