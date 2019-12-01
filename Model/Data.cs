using System;

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