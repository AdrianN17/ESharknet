using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Libs.Esharknet.Model
{
    public class Data_broadcast
    {
        public string ip;
        public int port;
        public int players;
        public int max_players;
        public string name_server;

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
