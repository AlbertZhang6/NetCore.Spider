using System;
using System.Text;

namespace NetCore.Security
{
    internal class StringSerializer : IBinarySerializer
    {
        public bool CanHandle(Type valueType)
        {
            return valueType == typeof(string);
        }

        public object Read(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] Write(object value)
        {
            string text = value as string;
            if (string.Empty.Equals(text))
            {
                return new byte[0];
            }
            return Encoding.UTF8.GetBytes(text);
        }
    }

}
