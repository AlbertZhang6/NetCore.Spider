using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Security
{
    internal interface IBinarySerializer
    {
        bool CanHandle(Type valueType);

        object Read(byte[] bytes);

        byte[] Write(object value);
    }

}
