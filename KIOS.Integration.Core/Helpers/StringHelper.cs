using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace DriveThru.Integration.Core.Helpers
{
    public static class StringHelper
    {
        public static string GenerateRandomCode(int size = 4)
        {
            char[] characters = new char[62];
            characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[size];
                crypto.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(size);

            foreach (byte byteData in data)
            {
                result.Append(characters[byteData % (characters.Length)]);
            }

            return result.ToString();
        }

        public static string JoinString<T>(IEnumerable<T> source, string seperator = "")
        {
            return string.Join(seperator, source);
        }

        public static string JoinEachString<T>(IEnumerable<T> source, string separator = "")
        {
            string result = string.Empty;
            int totalItems = source.Count();
            int currentItem = 0;

            foreach (T item in source)
            {
                ++currentItem;

                result += "'" + item + "'" + ((currentItem == totalItems) ? "" : separator);
            }

            return result;
        }

        public static string SplitString(string source, string seperator, string joiner)
        {
            string[] result = source.Split(seperator);

            if (result != null && result.Any())
            {
                return JoinString<string>(result, joiner);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string TruncatesString(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }
}
