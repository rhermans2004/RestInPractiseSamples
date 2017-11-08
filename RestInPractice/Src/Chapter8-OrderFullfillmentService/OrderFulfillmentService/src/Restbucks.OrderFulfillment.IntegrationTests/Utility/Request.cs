using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public class Request
    {
        public static Request ForPost(Uri uri, string contentType, object requestBody)
        {
            return new Request(uri, "POST", contentType, null, requestBody);
        }

        public static Request ForGet(Uri uri)
        {
            return new Request(uri, "GET", null, null, null);
        }

        public static Request ForPut(Uri uri, string contentType, EntityTag entityTag, object requestBody)
        {
            return new Request(uri, "PUT", contentType, entityTag.Value, requestBody);
        }

        public static Request ForDelete(Uri uri)
        {
            return new Request(uri, "DELETE", null, null, null);
        }

        private readonly Uri uri;
        private readonly string method;
        private readonly string contentType;
        private readonly string eTag;
        private readonly object requestBody;

        private Request(Uri uri, string method, string contentType, string eTag, object requestBody)
        {
            this.uri = uri;
            this.method = method;
            this.contentType = contentType;
            this.eTag = eTag;
            this.requestBody = requestBody;
        }

        public Response Execute()
        {
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(uri);
            webRequest.Method = method;

            if (!string.IsNullOrEmpty(eTag))
            {
                webRequest.Headers[HttpRequestHeader.IfMatch] = eTag;
            }

            if (method.Equals("PUT") || method.Equals("POST"))
            {
                byte[] buffer = Serialization.ToByteArray(requestBody);

                webRequest.ContentType = contentType;
                webRequest.ContentLength = buffer.Length;
                webRequest.GetRequestStream().Write(buffer, 0, buffer.Length);
            }

            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                XDocument entityBody = null;

                if (webResponse.ContentLength > 0)
                {
                    TextReader reader = new StreamReader(webResponse.GetResponseStream());
                    entityBody = XDocument.Load(reader);
                    reader.Close();
                }

                Response response = new Response((int)webResponse.StatusCode, webResponse.StatusDescription, webResponse.ContentType, webResponse.Headers[HttpResponseHeader.ETag], entityBody);
                webResponse.Close();

                return response;
            }
            catch (WebException ex)
            {
                HttpWebResponse webResponse = (HttpWebResponse) ex.Response;
                return new Response((int)webResponse.StatusCode, webResponse.StatusDescription, null, null, null);
            }
            
        }

        
    }
}