using System.Net;
using System.Net.Sockets;

namespace Assets.Libs.Esharknet.IP
{
    public class LocalIP
    {
        public string SetLocalIP()
        {
            string localIP;

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                socket.Close();
            }

            return localIP;
        }
    }
}
