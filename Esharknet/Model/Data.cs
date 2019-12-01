using Assets.Libs.Esharknet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Libs.Esharknet
{
    [Serializable]
    public class Data
    {
        public string key { get; set; }
        public object value { get; set; }
        public Data(string key, object value)
        {
            this.key = key;

            this.value = value;
        }
    }
}