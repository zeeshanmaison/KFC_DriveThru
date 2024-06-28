using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DriveThru.Integration.Core.Helpers
{
    public static class SecurityHelper
    {
        private const string ENCRYPTION_KEY = "LIS-2020";
        private const int SALT_SIZE = 24;

        public static string Encrypt(string plainText)
        {
            string encryptedText = string.Empty;
            byte[] bytes = Encoding.Unicode.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes rfcBytes = new Rfc2898DeriveBytes(ENCRYPTION_KEY,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = rfcBytes.GetBytes(32);
                aes.IV = rfcBytes.GetBytes(16);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }

                    encryptedText = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return encryptedText;
        }

        public static string Decrypt(string encryptedText)
        {
            string plainText = string.Empty;
            encryptedText = encryptedText.Replace(" ", "+");
            byte[] bytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                var rfcBytes = new Rfc2898DeriveBytes(ENCRYPTION_KEY,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = rfcBytes.GetBytes(32);
                aes.IV = rfcBytes.GetBytes(16);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }

                    plainText = Encoding.Unicode.GetString(memoryStream.ToArray());
                }
            }

            return plainText;
        }

        public static string GenerateSalt()
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[SALT_SIZE];

            randomNumberGenerator.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        public static string GetHashedPassword(string plainPassword, string salt)
        {
            string key = string.Join(":", new string[] { plainPassword, salt });
            HMACSHA256 hMac = new HMACSHA256();
            hMac.Key = Encoding.UTF8.GetBytes(salt);

            return Convert.ToBase64String(hMac.ComputeHash(Encoding.UTF8.GetBytes(key)));
        }
    }
}
