using Microsoft.AspNetCore.Mvc;
using NetCore.Session;
using System;

namespace NetCore.WebApi
{
    [Produces("application/json")]
    [ResponseCache(Duration = 1, Location = ResponseCacheLocation.None)]
    [VoidApiResult]
    [ApiErrorReply(Order = int.MaxValue)]
    public abstract class ApiController : ControllerBase
    {
        private ITypedSession session;

        private IUserIdentity identity;

        public ITypedSession Session
        {
            get
            {
                if (session == null)
                {
                    session = HttpContext.Session.AsTyped();
                }
                return session;
            }
        }

        public TIdentity Identity<TIdentity>() where TIdentity : class, IUserIdentity
        {
            if (identity == null || identity.GetType() != typeof(TIdentity))
            {
                identity = Activator.CreateInstance<TIdentity>();
                identity.Bind(this.User);
            }
            return identity as TIdentity;
        }
    }
}
