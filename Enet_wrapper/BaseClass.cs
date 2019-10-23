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

            Debug.Log("Received : " + json_value);

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
                Debug.LogError(key + " function not defined in dictionary");
            }
        }

        protected void ExecuteTriggerBytes(ENet.Event netEvent)
        {
            Data data = JSONDecode(netEvent.Packet);

            if (TriggerFunctions.ContainsKey(data.key))
            {
                TriggerFunctions[data.key](netEvent);
            }
            else
            {
                Debug.LogError(data.key + " function not defined in dictionary");
            }
        }

        public void AddTrigger(string key, Action<ENet.Event> function_calback)
        {
            TriggerFunctions.Add(key, function_calback);
        }
    }
}
