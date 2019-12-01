using System;
using System.Collections.Generic;
using Assets.Libs.Esharknet.Serialize;

namespace Assets.Libs.Esharknet
{
    public class BaseClass: Serialize_Class
    {
        protected Dictionary<string, Action<ENet.Event>> TriggerFunctions;
        protected int timeout;

        public BaseClass()
        {
            this.TriggerFunctions = new Dictionary<string, Action<ENet.Event>>();
        }

        public ENet.Packet Encode(Data data)
        {
            ENet.Packet packet = default(ENet.Packet);
            packet.Create(Serialize(data));

            return packet;
        }

        public Data Decode(ENet.Packet packet_data)
        {
            byte[] buffer = new byte[packet_data.Length];

            packet_data.CopyTo(buffer);

            return (Data)Deserialize(buffer);
        }

        protected void ExecuteTrigger(string key, ENet.Event netEvent)
        {
            if (TriggerFunctions.ContainsKey(key))
            {
                TriggerFunctions[key](netEvent);
            }
            else
            {
               //Debug.LogError(key + " function not defined in dictionary");
            }
        }

        protected void ExecuteTriggerBytes(ENet.Event netEvent)
        {
            Data data = Decode(netEvent.Packet);

            if (TriggerFunctions.ContainsKey(data.key))
            {
                TriggerFunctions[data.key](netEvent);
            }
            else
            {
                //Debug.LogError(data.key + " function not defined in dictionary");
            }
        }

        public void AddTrigger(string key, Action<ENet.Event> function_calback)
        {
            TriggerFunctions.Add(key, function_calback);
        }
    }
}
