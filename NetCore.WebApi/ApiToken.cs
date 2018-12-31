using NetCore.Jwt;
using NetCore.Session;

namespace NetCore.WebApi
{
    public class ApiToken
    {
        public JwtTokenPair Jwt
        {
            get;
            set;
        }

        public SessionPair Session
        {
            get;
            set;
        }
    }

}
