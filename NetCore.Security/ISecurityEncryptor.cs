using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Security
{

    public interface ISecurityEncryptor
    {
        string Encrypt(byte[] payload);

        string Encrypt(byte[] payload, TimeSpan lifetime);

        byte[] Decrypt(string s);
    }

}
