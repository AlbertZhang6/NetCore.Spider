using System;

namespace NetCore.Security
{
    internal class DecimalSerializer : IBinarySerializer
    {
        public bool CanHandle(Type valueType)
        {
            return valueType == typeof(decimal) || valueType == typeof(decimal?);
        }

        public object Read(byte[] bytes)
        {
            int[] array = new int[4];
            Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
            decimal num = new decimal(array);
            return num;
        }

        public byte[] Write(object value)
        {
            decimal d = (decimal)value;
            int[] bits = decimal.GetBits(d);
            byte[] array = new byte[16];
            int count = Buffer.ByteLength(bits);
            Buffer.BlockCopy(bits, 0, array, 0, count);
            return array;
        }
    }
}
