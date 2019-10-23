﻿using Assets.Libs.Esharknet.Model;
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
        private List<Data_broadcast> servers_list;

        private bool loop = true;

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
                            var json_data = Encoding.ASCII.GetString(bytes);
                            var data = JsonConvert.DeserializeObject<Data>(json_data);

                            Debug.Log("Broadcast receive : " + json_data);

                            if (data.key== "broadcast")
                            {
                                
                                var data_value = data.value.ToObject<Data_broadcast>();
                                Debug.Log(data.value);
                                var data_existence = validate_ip_existence(data_value.ip);

                                Debug.Log("Broadcast receive ip is : " + data_value.ip);

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
                    catch (SocketException ex) // or whatever the exception is that you're getting
                    {
                        Debug.LogWarning(ex.Message);
                    }


                    Thread.Sleep(timedelay);
                }
            });

            thread.Start();

        }

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

        public List<Data_broadcast> GetListObtained()
        {
            return servers_list;
        }

        public void Destroy()
        {
            Debug.LogWarning("Broadcast receive finish");

            loop = false;

            udpClient.Close();
            servers_list.Clear();
 
        }
    }
}
