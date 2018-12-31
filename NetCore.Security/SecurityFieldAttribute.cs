using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Security
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SecurityFieldAttribute : Attribute
    {
    }

}
