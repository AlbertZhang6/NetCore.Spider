using System;

namespace NetCore.Security
{
    internal static class SecurityEncryptorFactory
    {
        private static ISecurityEncryptor singleton;

        public static void Configure(ISecurityEncryptor encryptor)
        {
            singleton = encryptor;
        }

        public static ISecurityEncryptor GetEncryptor()
        {
            if (singleton == null)
            {
                throw new InvalidOperationException("SecurityComponent is not configured in ServiceCollection.");
            }
            return singleton;
        }
    }
}
