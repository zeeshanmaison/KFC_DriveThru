using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DriveThru.Integration.Core.Helpers
{
    public static class HttpHelper
    {
        public static TResponse Post<TData, TResponse>(string url, TData data, string contentType, Dictionary<string, string> headers = null) 
            where TData : class
            where TResponse : class
        {
            string json = JsonHelper.Serialize(data);
            StringContent content = new StringContent(json, Encoding.UTF8, contentType);
            HttpClient httpClient = new HttpClient();

            if (headers != null && headers.Count > 0)
            {
                foreach (string headerKey in headers.Keys)
                {
                    httpClient.DefaultRequestHeaders.Add(headerKey, headers[headerKey]);
                }
            }

            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            TResponse responseData = null;

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent httpContent = response.Content;
                string result = httpContent.ReadAsStringAsync().Result;

                httpContent.Dispose();

                if (!string.IsNullOrEmpty(result))
                {
                    responseData = JsonHelper.Deserialize<TResponse>(result);
                }
            }

            response.Dispose();
            httpClient.Dispose();

            return responseData;
        }

        public async static Task<string> PostJson(string url, string json, IDictionary<string, object> headers = null)
        {
            string response = string.Empty;

            using (var httpClientHandler = new HttpClientHandler())
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    if (headers != null)
                    {
                        foreach (string key in headers.Keys)
                        {
                            string value = headers[key].ToString();

                            client.DefaultRequestHeaders.Add(key, value);
                        }
                    }

                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    if (!string.IsNullOrEmpty(json))
                    {
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        requestMessage.Content = content;
                    }

                    try
                    {
                        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage,
                       HttpCompletionOption.ResponseContentRead,
                       CancellationToken.None)
                       .ConfigureAwait(false);
                        responseMessage.EnsureSuccessStatusCode();

                        if (responseMessage != null && (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Created))
                        {
                            response = await responseMessage.Content.ReadAsStringAsync();
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                   

                   
                }
            }
            return response;
        }

        private static HttpWebRequest PrepareRequest_Custom(string uri, string requestMethod, IDictionary<string, object> headers = null)
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

        public static string Get_Custom(string uri, IDictionary<string, object> headers = null)
        {
            string requestMethod = "GET";
            HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
            string response = string.Empty;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            return response;
        }

        public async static Task<string> Get(string url, IDictionary<string, object> headers = null)
        {
            string response = string.Empty;

            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        string value = headers[key].ToString();

                        client.DefaultRequestHeaders.Add(key, value);
                    }
                }

                try
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(url);

                    if (responseMessage != null && responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        response = await responseMessage.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }


            }

            return response;
        }

        public static T Get_Custom<T>(string uri, IDictionary<string, object> headers = null)
        {
            T responseData = default(T);

            try
            {
                string requestMethod = "GET";
                HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
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

        public static string Post_Custom(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "POST";
            HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
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

        public static T Post_Custom<T>(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "POST";
            HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
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

        public static string Put_Custom(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "PUT";
            HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
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

        public static T Put_Custom<T>(string uri, object payload, IDictionary<string, object> headers = null)
        {
            string requestMethod = "PUT";
            HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
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

        public static bool Delete_Custom(string uri, IDictionary<string, object> headers = null)
        {
            string requestMethod = "DELETE";
            HttpWebRequest httpWebRequest = PrepareRequest_Custom(uri, requestMethod, headers);
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
