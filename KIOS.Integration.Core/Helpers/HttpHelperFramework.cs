using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace DriveThru.Integration.Core.Helpers
{
    public class HttpHelperFramework
    {
        private static HttpWebRequest PrepareRequest(string uri, string requestMethod, IDictionary<string, object> headers = null)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            if (headers != null)
            {
                if (headers.Count > 0)
                {
                    foreach (string key in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, headers[key].ToString());
                    }
                }
            }

            httpWebRequest.Method = requestMethod.ToUpper();

            return httpWebRequest;
        }

        public static string Get(string uri, IDictionary<string, object> headers = null)
        {
            string requestMethod = "GET";
            HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
            string response = string.Empty;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            return response;
        }

        public static T Get<T>(string uri, IDictionary<string, object> headers = null)
        {
            T responseData = default(T);

            try
            {
                string requestMethod = "GET";
                HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
                string response = string.Empty;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        response = reader.ReadToEnd();
                    }

                    if (httpWebResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response))
                    {
                        responseData = JsonHelper.Deserialize<T>(response);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return responseData;
        }

        public static string Post(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "POST";
            HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
            string content = string.Empty;
            string response = string.Empty;

            httpWebRequest.ContentType = "application/json; charset=utf-8";

            if (payload != null)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonHelper.Serialize(payload);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            return response;
        }

        public static T Post<T>(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "POST";
            HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
            string content = string.Empty;
            string response = string.Empty;

            httpWebRequest.ContentType = "application/json; charset=utf-8";

            if (payload != null)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonHelper.Serialize(payload);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                T responseData = default(T);

                if (httpWebResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response))
                {
                    responseData = JsonHelper.Deserialize<T>(response);
                }

                return responseData;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static string Put(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "PUT";
            HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
            string content = string.Empty;
            string response = string.Empty;

            httpWebRequest.ContentType = "application/json; charset=utf-8";

            if (payload != null)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonHelper.Serialize(payload);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            return response;
        }

        public static T Put<T>(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "PUT";
            HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
            string content = string.Empty;
            string response = string.Empty;

            httpWebRequest.ContentType = "application/json; charset=utf-8";

            if (payload != null)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonHelper.Serialize(payload);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            T responseData = default(T);

            if (httpWebResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response))
            {
                responseData = JsonHelper.Deserialize<T>(response);
            }

            return responseData;
        }

        public static bool Delete(string uri, IDictionary<string, object> headers = null)
        {
            string requestMethod = "DELETE";
            HttpWebRequest httpWebRequest = PrepareRequest(uri, requestMethod, headers);
            string response = string.Empty;
            bool isDelete = false;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            if (httpWebResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response))
            {
                isDelete = true;
            }

            return isDelete;
        }   
    }
}
