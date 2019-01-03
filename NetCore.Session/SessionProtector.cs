using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Session
{
    internal class SessionProtector
    {
        private readonly IDataProtector protector;

        private readonly ILogger<SessionProtector> logger;

        public SessionProtector(IDataProtectionProvider protectionProvider, ILogger<SessionProtector> logger)
        {
            protector = protectionProvider.CreateProtector("HeaderSessionMiddleware");
            this.logger = logger;
        }

        public string Protect(string plainData)
        {
            if (string.IsNullOrEmpty(plainData))
            {
                return plainData;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(plainData);
            byte[] array = protector.Protect(bytes);
            return WebEncoders.Base64UrlEncode(array);
        }

        public string Unprotect(string protectedData)
        {
            if (string.IsNullOrEmpty(protectedData))
            {
                return protectedData;
            }
            try
            {
                byte[] array = WebEncoders.Base64UrlDecode(protectedData);
                byte[] bytes = protector.Unprotect(array);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (CryptographicException ex)
            {
                LoggerExtensions.LogWarning(logger, (Exception)ex, "the session key is invalid during unprotecting:" + protectedData, Array.Empty<object>());
            }
            catch (Exception ex2)
            {
                LoggerExtensions.LogWarning(logger, ex2, "the session key is failed to unprotect:" + protectedData, Array.Empty<object>());
            }
            return string.Empty;
        }
    }
}
