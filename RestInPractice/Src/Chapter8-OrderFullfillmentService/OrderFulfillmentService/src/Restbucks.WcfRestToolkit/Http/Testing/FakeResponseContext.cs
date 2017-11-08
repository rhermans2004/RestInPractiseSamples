using System;

namespace Restbucks.WcfRestToolkit.Http.Testing
{
    public class FakeResponseContext : IResponseContext
    {
        public static FakeResponseContext Generate(Func<Response> func)
        {
            Response response = func.Invoke();
            FakeResponseContext responseContext = new FakeResponseContext();
            response.ApplyTo(responseContext);
            return responseContext;
        }

        private FakeResponseContext()
        {
        }

        private string statusDescription;
        private int? statusCode;
        private string cacheControl;
        private string contentType;
        private string location;
        private DateTimeOffset lastModified;
        private string etag;
        private string contentLocation;

        void IResponseContext.SetStatusDescription(string value)
        {
            statusDescription = value;
        }

        void IResponseContext.SetStatusCode(int value)
        {
            statusCode = value;
        }

        void IResponseContext.SetCacheControl(string value)
        {
            cacheControl = value;
        }

        void IResponseContext.SetContentType(string value)
        {
            contentType = value;
        }

        void IResponseContext.SetLocation(string value)
        {
            location = value;
        }

        void IResponseContext.SetLastModified(DateTimeOffset value)
        {
            lastModified = value;
        }

        void IResponseContext.SetContentLocation(string value)
        {
            contentLocation = value;
        }

        public void SetETag(string value)
        {
            etag = value;
        }

        public string StatusDescription
        {
            get { return statusDescription; }
        }

        public int? StatusCode
        {
            get { return statusCode; }
        }

        public string CacheControl
        {
            get { return cacheControl; }
        }

        public string ContentType
        {
            get { return contentType; }
        }

        public string Location
        {
            get { return location; }
        }

        public DateTimeOffset LastMofified
        {
            get { return lastModified; }
        }

        public string ETag
        {
            get { return etag; }
        }

        public string ContentLocation
        {
            get { return contentLocation; }
        }
    }
}