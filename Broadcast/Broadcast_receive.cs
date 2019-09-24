using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Libs.Esharknet.Broadcast
{
    public class Broadcast_receive
    {
        public UdpClient udpClient;
        private IPEndPoint ip_point;
        private Thread thread;
        private Dictionary<string, dynamic> servers_list;

        public Broadcast_receive(string ip_address, ushort port_send, int timedelay)
        {
            udpClient = new UdpClient();
            ip_point = new IPEndPoint(IPAddress.Parse(ip_address), port_send);
            udpClient.Client.Bind(ip_point);

            //servers_list = new Dictionary<string, dynamic>();

            thread = new Thread(delegate ()
            {
                while (udpClient != null && thread != null)
                {
                    try
                    {
                        if (udpClient != null)
                        {
                            var bytes = udpClient.Receive(ref ip_point);
                            var json_data = Encoding.ASCII.GetString(bytes);
                            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json_data);

                            Debug.Log("Broadcast receive : " + json_data);

                            var data = dictionary["Broadcast"];
                            validate(data["ip"], data);
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        Debug.LogWarning(ex.Message);
                        Thread.ResetAbort();
                    }


            }

            });

            thread.Start();
            Thread.Sleep(1000);

            /*
             *  {broadcast = {ip=192.168.0.3,port=22122,players=1,max_players=5,name_server=room1}}
             * 
             */

        }

        private void validate(string ip,dynamic data)
        {
            if(servers_list.ContainsKey(ip))
            {
                var dictionary = data;
                servers_list[ip]["players"] = data["players"];
            }
            else
            {
                servers_list.Add(ip,data);
            }
        }

        Dictionary<string, dynamic> GetListObtained()
        {
            return this.servers_list;
        }

        public void Destroy()
        {
            Debug.LogWarning("Broadcast receive finish");
            thread.Abort();
            udpClient.Close();
            servers_list.Clear();

            udpClient = null;
            thread = null;
            
        }
    }
}
