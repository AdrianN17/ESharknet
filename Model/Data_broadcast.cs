using System;

namespace Assets.Libs.Esharknet.Model
{
    [Serializable]
    public class Data_broadcast
    {
        public string ip { get; set; }
        public int port { get; set; }
        public int players { get; set; }
        public int max_players { get; set; }
        public string name_server { get; set; }

        public Data_broadcast(string ip, int port, int players, int max_players, string name_server)
        {
            this.ip = ip;
            this.port = port;
            this.players = players;
            this.max_players = max_players;
            this.name_server = name_server;
        }

        public void Set_players(int players)
        {
            this.players = players;
        }
    }
}
