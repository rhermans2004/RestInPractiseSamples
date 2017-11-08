using System;
using System.Net;
using System.ServiceModel.Web;
using Restbucks.WcfRestToolkit.Http;

namespace Restbucks.WcfRestToolkit.WcfDecoupling
{
    public class WcfResponseContext : IResponseContext
    {
        private readonly OutgoingWebResponseContext context;

        public WcfResponseContext(OutgoingWebResponseContext context)
        {
            this.context = context;
        }

        public void SetStatusDescription(string value)
        {
            context.StatusDescription = value;
        }

        public void SetStatusCode(int value)
        {
            context.StatusCode = (HttpStatusCode) Enum.Parse(typeof(HttpStatusCode), value.ToString());
        }

        public void SetCacheControl(string value)
        {
            context.Headers[HttpResponseHeader.CacheControl] = value;
        }

        public void SetContentType(string value)
        {
            context.ContentType = value;
        }

        public void SetLocation(string value)
        {
            context.Location = value;
        }

        public void SetLastModified(DateTimeOffset value)
        {
            context.LastModified = value.DateTime;
        }

        public void SetETag(string value)
        {
            context.ETag = value;
        }

        public void SetContentLocation(string value)
        {
            context.Headers[HttpResponseHeader.ContentLocation] = value;
        }
    }
}