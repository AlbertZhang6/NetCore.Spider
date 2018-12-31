using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCore.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace NetCore.Jwt
{
    internal class JwtTokenGenerator : IJwtTokenGenerator
    {
        private JwtOptions options;

        private readonly SigningCredentials signingCredentials;

        private IDistributedCache cache;

        private ISecurityEncryptor encryptor;

        private ILogger<IJwtTokenGenerator> logger;

        public JwtTokenGenerator(IOptions<JwtOptions> options, IDistributedCache cache, ISecurityEncryptor encryptor, ILogger<IJwtTokenGenerator> logger)
        {
            this.options = options.Value;
            signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.options.PrivateKey)), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");
            this.cache = cache;
            this.encryptor = encryptor;
            this.logger = logger;
        }

        public JwtTokenPair Generate(ClaimsPrincipal principal)
        {
            return Generate(principal, options.ValidTime);
        }

        public JwtTokenPair Generate(ClaimsPrincipal principal, TimeSpan validTime)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }
            if (principal.HasClaim((Claim c) => c.Type == "jti"))
            {
                throw new ArgumentException(string.Format("principal claims could not contain the {0} claim.", "jti"));
            }
            string text = NewID();
            DateTime utcNow = DateTime.UtcNow;
            utcNow = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);
            DateTime value = utcNow.Add(validTime);
            List<Claim> list = new List<Claim>(principal.Claims);
            list.Add(new Claim("jti", text, "http://www.w3.org/2001/XMLSchema#string"));
            JwtHeader val = new JwtHeader(signingCredentials);
            JwtPayload val2 = new JwtPayload(options.Issuer, options.Audience, (IEnumerable<Claim>)list, (DateTime?)null, (DateTime?)value, (DateTime?)utcNow);
            JwtSecurityToken val3 = new JwtSecurityToken(val, val2);
            JwtSecurityTokenHandler val4 = new JwtSecurityTokenHandler();
            string authToken = val4.WriteToken(val3);
            RefreshToken refreshToken = new RefreshToken
            {
                Id = NewID(),
                JwtId = text,
                Expire = utcNow.Add(validTime + options.ClockSkew)
            };
            string s = JsonConvert.SerializeObject((object)refreshToken);
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            string refreshToken2 = encryptor.Encrypt(bytes);
            return new JwtTokenPair
            {
                AuthToken = authToken,
                RefreshToken = refreshToken2,
                IssuedAt = utcNow,
                ExpirationTime = value
            };
        }

        public JwtTokenPair Refresh(string refreshToken, ClaimsPrincipal principal)
        {
            return Refresh(refreshToken, principal, options.ValidTime);
        }

        public JwtTokenPair Refresh(string refreshToken, ClaimsPrincipal principal, TimeSpan validTime)
        {
            //IL_01d1: Unknown result type (might be due to invalid IL or missing references)
            //IL_01d6: Unknown result type (might be due to invalid IL or missing references)
            //IL_01f2: Expected O, but got Unknown
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException("refreshToken");
            }
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }
            if (!principal.HasClaim((Claim c) => c.Type == "jti"))
            {
                throw new ArgumentException(string.Format("principal claims must contain the {0} claim.", "jti"));
            }
            byte[] array = encryptor.Decrypt(refreshToken);
            if (array == null)
            {
                throw new InvalidOperationException("refresh token cracks failed");
            }
            string @string = Encoding.UTF8.GetString(array);
            RefreshToken refreshToken2 = JsonConvert.DeserializeObject<RefreshToken>(@string);
            if (refreshToken2.Expire <= DateTime.UtcNow)
            {
                throw new InvalidOperationException(string.Format("refresh token <{0}> has expired, expires on {1}Z, current server time is {2}Z", refreshToken2.Id, refreshToken2.Expire.ToString("s"), DateTime.UtcNow.ToString("s")));
            }
            Claim claim = principal.FindFirst("jti");
            if (refreshToken2.JwtId != claim.Value)
            {
                throw new InvalidOperationException($"refresh token <{refreshToken2.Id}>'s jwt id \"{refreshToken2.JwtId}\" doesn't match the current auth token id \"{claim.Value}\"");
            }
            byte[] array2 = cache.Get(refreshToken2.Id);
            if (array2 != null)
            {
                throw new InvalidOperationException($"refresh token <{refreshToken2.Id}> has used");
            }
            Claim[] claims = (from c in principal.Claims
                              where c.Type != "jti" && c.Type != "aud" && c.Type != "iss"
                              select c).ToArray();
            ClaimsPrincipal principal2 = new ClaimsPrincipal(new ClaimsIdentity(claims));
            JwtTokenPair result = Generate(principal2, validTime);
            IDistributedCache obj = cache;
            string id = refreshToken2.Id;
            byte[] bytes = BitConverter.GetBytes(value: true);
            DistributedCacheEntryOptions val = new DistributedCacheEntryOptions();
            val.AbsoluteExpiration = (DateTimeOffset?)new DateTimeOffset(refreshToken2.Expire);
            obj.Set(id, bytes, val);
            LoggerExtensions.LogInformation(logger, string.Format("refresh token <{0}> has refreshed successfully on {1}Z, correlative auth token id is \"{2}\"", refreshToken2.Id, DateTime.UtcNow.ToString("s"), claim.Value), Array.Empty<object>());
            return result;
        }

        private string NewID()
        {
            return Guid.NewGuid().ToString();
        }

        public void Destroy(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }
            Claim claim = principal.FindFirst("jti");
            Claim claim2 = principal.FindFirst("exp");
            if (claim == null || claim2 == null)
            {
                throw new ArgumentException("principal claims must contain the jti & exp claim");
            }
            DateTimeOffset value = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim2.Value)).Add(options.ClockSkew);
            IDistributedCache obj = cache;
            string value2 = claim.Value;
            byte[] bytes = BitConverter.GetBytes(value: true);
            DistributedCacheEntryOptions val = new DistributedCacheEntryOptions();
            val.AbsoluteExpiration = (DateTimeOffset?)value;
            obj.Set(value2, bytes, val);
        }
    }
}
