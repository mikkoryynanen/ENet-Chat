using System;
using System.Text;

namespace Shared
{
    public class Serializer
    {
        public static byte[] Serialize(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }

        public static string Deserialize(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}