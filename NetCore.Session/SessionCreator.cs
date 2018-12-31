using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;

namespace NetCore.Session
{
    internal class SessionCreator : ISessionCreator
    {
        private IHttpContextAccessor contextAccessor;

        private ISessionStore store;

        private SessionProtector protector;

        private SessionOptions options;

        private SessionScope scope;

        private static readonly RandomNumberGenerator idGenerator = RandomNumberGenerator.Create();

        private const int SessionKeyLength = 16;

        public SessionCreator(IHttpContextAccessor contextAccessor, ISessionStore store, SessionProtector protector, IOptions<SessionOptions> options, SessionScope scope)
        {
            this.contextAccessor = contextAccessor;
            this.store = store;
            this.protector = protector;
            this.options = options.Value;
            this.scope = scope;
        }

        public SessionPair Create()
        {
            if (scope.Session != null)
            {
                return new SessionPair
                {
                    Session = scope.Session,
                    Key = scope.Key
                };
            }
            byte[] array = new byte[16];
            idGenerator.GetBytes(array);
            string text = ToIdString(array);
            ISession session = store.Create(text, options.IdleTimeout, options.IOTimeout, (Func<bool>)scope.TryEstablish, true);
            string key = protector.Protect(text);
            scope.Session = session;
            scope.Key = key;
            contextAccessor.HttpContext.Session = session;
            return new SessionPair
            {
                Session = session,
                Key = key
            };
        }

        private string ToIdString(byte[] idBytes)
        {
            return BitConverter.ToUInt32(idBytes, 0).ToString("X8") + "-" + BitConverter.ToUInt16(idBytes, 4).ToString("X4") + "-" + BitConverter.ToUInt16(idBytes, 6).ToString("X4") + "-" + BitConverter.ToUInt16(idBytes, 8).ToString("X4") + "-" + BitConverter.ToUInt32(idBytes, 10).ToString("X8") + BitConverter.ToUInt16(idBytes, 14).ToString("X4");
        }
    }
}
