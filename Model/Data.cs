using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Libs.Esharknet
{
    public class Data
    {
        public string key;
        public dynamic value;
        public Data(string key,dynamic value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
