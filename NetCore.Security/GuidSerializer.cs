using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Security
{
    internal class GuidSerializer : IBinarySerializer
    {
        public bool CanHandle(Type valueType)
        {
            return valueType == typeof(Guid) || valueType == typeof(Guid?);
        }

        public object Read(byte[] bytes)
        {
            Guid guid = new Guid(bytes);
            return guid;
        }

        public byte[] Write(object value)
        {
            return ((Guid)value).ToByteArray();
        }
    }
}
