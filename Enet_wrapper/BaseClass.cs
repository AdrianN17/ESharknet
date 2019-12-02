using System;
using System.Collections.Generic;
using Assets.Libs.Esharknet.Serialize;

namespace Assets.Libs.Esharknet
{
    public class BaseClass: Serialize_Class
    {
        protected Dictionary<string, Action<ENet.Event>> TriggerFunctions;
        protected int timeout;
        /// <summary>
        /// Base Class to Server and Client Class
        /// </summary>
        public BaseClass()
        {
            this.TriggerFunctions = new Dictionary<string, Action<ENet.Event>>();
        }

        /// <summary>
        /// Encode Data Object to ENet Packet
        /// </summary>
        /// <param name="data">Data Object to Encode</param>
        /// <returns>get ENet Packet</returns>
        public ENet.Packet Encode(Data data)
        {
            ENet.Packet packet = default(ENet.Packet);
            packet.Create(Serialize(data));

            return packet;
        }

        /// <summary>
        /// Decode ENet Packet to Data Object
        /// </summary>
        /// <param name="packet_data">ENet Packet to Decode</param>
        /// <returns>get Data Object</returns>
        public Data Decode(ENet.Packet packet_data)
        {
            byte[] buffer = new byte[packet_data.Length];

            packet_data.CopyTo(buffer);

            return (Data)Deserialize(buffer);
        }

        /// <summary>
        /// Decode ENet Packet to Data Object
        /// </summary>
        /// <param name="key">Name for event </param>
        /// <param name="netEvent">Enet Event Packet</param>
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

        /// <summary>
        /// Decode ENet Packet to Data Object
        /// Used in Received Event Type
        /// </summary>
        /// <param name="netEvent">Enet Event Packet</param>
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

        /// <summary>
        /// Create new Trigger to respond Received Events
        /// Used in Received Event Type
        /// </summary>
        /// <param name="key">Name for event </param>
        /// <param name="function_calback">Function to respond the data</param>
        public void AddTrigger(string key, Action<ENet.Event> function_calback)
        {
            TriggerFunctions.Add(key, function_calback);
        }
    }
}
