using Microsoft.AspNetCore.Mvc;
using System;

namespace NetCore.Security
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class SecurityParameterAttribute : ModelBinderAttribute
    {
        public SecurityParameterAttribute()
            : base(typeof(SecurityParameterBinder))
        {
        }
    }

}
