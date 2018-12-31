using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace NetCore.WebApi
{
    public interface IUserIdentity
    {
        void Bind(ClaimsPrincipal principal);

        ClaimsPrincipal Build();
    }

}
