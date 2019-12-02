using Assets.Libs.Esharknet.Model;
using Assets.Libs.Esharknet.Serialize;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Assets.Libs.Esharknet.Broadcast
{
    public class Broadcast_receive:Serialize_Class
    {
        public UdpClient udpClient;
        private IPEndPoint ip_point;
        private Thread thread;
        private List<Data_broadcast> servers_list;

        private bool loop = true;

        /// <summary>
        /// Inicializate Broadcast to receive data
        /// </summary>
        /// <param name="ip_address">IP to send</param>
        /// <param name="port_send">Destination Port</param>
        /// <param name="timedelay">Frecuency of time to every send</param>
        public Broadcast_receive(string ip_address, ushort port_send, int timedelay)
        {
            udpClient = new UdpClient();
            ip_point = new IPEndPoint(IPAddress.Parse(ip_address), port_send);
            udpClient.Client.Bind(ip_point);

            servers_list = new List<Data_broadcast>();

            thread = new Thread(delegate ()
            {
                while (loop)
                {
                    try
                    {

                        var bytes = udpClient.Receive(ref ip_point);

                        if (bytes.Length > 0)
                        {
                            
                            var data = (Data)Deserialize(bytes);


                            if (data.key== "broadcast")
                            {

                                var data_value = (Data_broadcast)data.value;
                                var data_existence = validate_ip_existence(data_value.ip);

                                //Debug.Log("Broadcast receive ip is : " + data_value.ip);

                                if (data_existence==null)
                                {
                                    servers_list.Add(data_value);
                                }
                                else
                                {
                                    servers_list[servers_list.IndexOf(data_existence)] = data_value;
                                }
                            }
                            
                        }

                        
                    }
                    catch (SocketException ex) 
                    {
                        //Debug.LogWarning(ex.Message);
                    }


                    Thread.Sleep(timedelay);
                }
            });

            thread.Start();

        }

        /// <summary>
        /// Validate IP in another object Data
        /// </summary>
        /// <returns>Get Data_Broadcast Object</returns>
        public Data_broadcast validate_ip_existence(string ip)
        {
            foreach(var data in servers_list)
            {
                if(data.ip==ip)
                {
                    return data;
                }
            }

            return null;
        }
        /// <summary>
        /// Get All Data obtained by Broadcast
        /// </summary>
        /// <returns>Get Data Broadcast List</returns>
        public List<Data_broadcast> GetListObtained()
        {
            return servers_list;
        }

        /// <summary>
        /// Destroy Broadcast Receive
        /// </summary>
        public void Destroy()
        {
            //Debug.LogWarning("Broadcast receive finish");

            loop = false;

            udpClient.Close();
            servers_list.Clear();
 
        }
    }
}
