using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NetCore.Security
{
    internal class SecurityEncryptor : ISecurityEncryptor
    {
        private readonly byte[] key;

        private readonly DateTime zeroTime;

        public SecurityEncryptor(IOptions<SecurityOptions> options)
        {
            key = Encoding.UTF8.GetBytes(options.Value.SecureKey);
            zeroTime = DateTimeOffset.Parse("2018-01-01T00:00:00Z").UtcDateTime;
        }

        public string Encrypt(byte[] payload)
        {
            return Encrypt(payload, Timeout.InfiniteTimeSpan);
        }

        public string Encrypt(byte[] payload, TimeSpan lifetime)
        {
            if (payload == null)
            {
                return null;
            }
            if (payload.Length == 0)
            {
                return string.Empty;
            }
            byte[] array;
            if (lifetime == Timeout.InfiniteTimeSpan)
            {
                array = IntToBytes(-1);
            }
            else
            {
                DateTime time = DateTime.UtcNow.Add(lifetime);
                int value = ToTimestamp(time);
                array = IntToBytes(value);
            }
            byte[] array2 = new byte[array.Length + payload.Length];
            Buffer.BlockCopy(array, 0, array2, 0, array.Length);
            Buffer.BlockCopy(payload, 0, array2, array.Length, payload.Length);
            return Secure(array2);
        }

        public byte[] Decrypt(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            byte[] array = Crack(s);
            if (array == null)
            {
                return null;
            }
            if (array.Length <= 4)
            {
                return null;
            }
            int num = BytesToInt(array, 0);
            if (num != -1)
            {
                DateTime t = ToDateTime(num);
                DateTime utcNow = DateTime.UtcNow;
                if (t <= utcNow)
                {
                    return null;
                }
            }
            byte[] array2 = new byte[array.Length - 4];
            Buffer.BlockCopy(array, 4, array2, 0, array2.Length);
            return array2;
        }

        private string Secure(byte[] input)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(input, 0, input.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] array = memoryStream.ToArray();
                        return Base64UrlTextEncoder.Encode(array);
                    }
                }
            }
        }

        private byte[] Crack(string s)
        {
            try
            {
                byte[] array = Base64UrlTextEncoder.Decode(s);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(array, 0, array.Length);
                            cryptoStream.FlushFinalBlock();
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private int ToTimestamp(DateTime time)
        {
            double totalSeconds = (time - zeroTime).TotalSeconds;
            return Convert.ToInt32(Math.Ceiling(totalSeconds));
        }

        private DateTime ToDateTime(int timestamp)
        {
            return zeroTime.AddSeconds(timestamp);
        }

        private byte[] IntToBytes(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value);
            }
            return new byte[4]
            {
            (byte)value,
            (byte)(value >> 8),
            (byte)(value >> 16),
            (byte)(value >> 24)
            };
        }

        private int BytesToInt(byte[] bytes, int startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt32(bytes, startIndex);
            }
            return (bytes[startIndex + 3] << 24) | (bytes[startIndex + 2] << 16) | (bytes[startIndex + 1] << 8) | bytes[startIndex];
        }
    }

}
