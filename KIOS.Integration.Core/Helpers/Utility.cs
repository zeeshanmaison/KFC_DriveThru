using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace DriveThru.Integration.Core.Helpers
{
    public static class Utility
    {
        private static readonly byte[] Bytes = Encoding.ASCII.GetBytes("Solution");

        public static int RandomNumber()
        {
            Random random = new Random();

            int randomCode = random.Next(9999);

            return randomCode;
        }

        public static string RandomString(int size, bool lowerCase = true)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            else
                return builder.ToString();
        }

        public static bool IsInteger(string str)
        {
            Regex regex = new Regex(@"^[0-9]+$");

            try
            {
                if (String.IsNullOrWhiteSpace(str))
                {
                    return false;
                }
                if (!regex.IsMatch(str))
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsNumeric(string value)
        {
            //Check if it is digit or numeric then return true
            return Regex.IsMatch(value, @"\d");
        }

        public static bool IsAllDigits(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public static string FormatDate(this string dateString, string dateFormat)
        {
            string returnDate = string.Empty;

            try
            {
                DateTime date = DateTime.Parse(dateString);
                returnDate = date.ToString(dateFormat);
            }
            catch (Exception ex)
            {
                returnDate = "InnerException: " + ex.InnerException + " | ErrorMessage: " + ex.Message +
                             " | StackTrace: " + ex.StackTrace;

            }

            return returnDate;
        }

        public static string GetEncryptedString(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(Bytes, Bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public static bool IsMobileNumber(string value)
        {
            //Check if it is digit or numeric then return true
            //return Regex.IsMatch(value, @"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$");
            return Regex.IsMatch(value, @"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$");
        }

        public static bool IsMobile(string value)
        {
            //Check if it is digit or numeric then return true
            //return Regex.IsMatch(value, @"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$");
            //return Regex.IsMatch(value, @"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$");
            return Regex.IsMatch(value, @"^((\+92)|(0092)|(92))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$");
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return true;
                }

                Regex regex = new Regex(@"\A(?:[a-z0-9&*+/=?^_{|}-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                return regex.IsMatch(email) && !email.EndsWith(".") && !email.Contains("'");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return true;
                }

                Regex regex = new Regex(@"^[A-Za-z\s]*$");
                return regex.IsMatch(name) && !name.Contains("'");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidDate(string date)
        {
            try
            {
                if (date != "")
                {
                    DateTime dDate;
                    if (DateTime.TryParse(date, out dDate))
                    {
                        return true;
                    }

                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool IsEmailExistInDomain(string email)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            bool isVerified = false;
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);

            string urlParameters = email;
            string api = ConfigurationManager.AppSettings["EmailCheckURI"];

            string URL = api + urlParameters;
            HttpResponseMessage response = null;
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                response = client.GetAsync(URL).Result;
                string result = string.Empty;

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    result = content.ReadAsStringAsync().Result;

                    if (result.Contains("OK"))
                    {
                        isVerified = true;
                    }

                    content.Dispose();
                }

                response.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {

                throw ex;
            }



            return isVerified;
        }

    }
}
