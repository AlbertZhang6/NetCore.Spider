using System;
using System.Security.Claims;

namespace NetCore.Jwt
{
    public interface IJwtTokenGenerator
    {
        JwtTokenPair Generate(ClaimsPrincipal principal);

        JwtTokenPair Generate(ClaimsPrincipal principal, TimeSpan validTime);

        JwtTokenPair Refresh(string refreshToken, ClaimsPrincipal principal);

        JwtTokenPair Refresh(string refreshToken, ClaimsPrincipal principal, TimeSpan validTime);

        void Destroy(ClaimsPrincipal principal);
    }

}
