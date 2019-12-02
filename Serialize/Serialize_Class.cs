using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Libs.Esharknet.Serialize
{
    public class Serialize_Class
    {
        /// <summary>
        /// Serialize Object to Byte Array
        /// </summary>
        /// <param name="o">Object to Serialize</param>
        /// <returns>get Byte Array</returns>
        public static Byte[] Serialize(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream.ToArray();
        }

        /// <summary>
        /// Deserialize Byte Array to Object
        /// </summary>
        /// <param name="bytes">Byte Array to Deserialize</param>
        /// <returns>get Object</returns>
        public static object Deserialize(Byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);

            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }

}
