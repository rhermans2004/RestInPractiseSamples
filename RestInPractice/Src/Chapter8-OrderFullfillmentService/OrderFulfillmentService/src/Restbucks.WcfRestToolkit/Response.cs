using System;
using System.Text;
using Restbucks.WcfRestToolkit.Http;
using Restbucks.WcfRestToolkit.Http.Headers;
using Restbucks.WcfRestToolkit.Http.StatusCodes;

namespace Restbucks.WcfRestToolkit
{
    public class Response
    {
        private readonly IStatus status;
        private readonly Action<ResponseHeaders> headersAction;

        public Response(IStatus status) : this(status, headers => { })
        {
        }

        public Response(IStatus status, Action<ResponseHeaders> headersAction)
        {
            this.status = status;
            this.headersAction = headersAction;
        }

        public void ApplyTo(IResponseContext context)
        {
            status.ApplyTo(context);
            ResponseHeaders headers = new ResponseHeaders();
            headersAction(headers);
            headers.ApplyTo(context);
        }

        public string CreateSummary()
        {
            StringBuilder sb = new StringBuilder();
            SummaryResponseContext src = new SummaryResponseContext(sb);
            status.ApplyTo(src);
            ResponseHeaders headers = new ResponseHeaders();
            headersAction(headers);
            headers.ApplyTo(src);
            return sb.ToString();
        }

        private class SummaryResponseContext : IResponseContext
        {
            private readonly StringBuilder sb;

            public SummaryResponseContext(StringBuilder sb)
            {
                this.sb = sb;
            }

            public void SetStatusDescription(string value)
            {
                sb.AppendFormat(" [StatusDescription: {0}]", value);
            }

            public void SetStatusCode(int value)
            {
                sb.AppendFormat(" [StatusCode: {0}]", value);
            }

            public void SetCacheControl(string value)
            {
                sb.AppendFormat(" [CacheControl: {0}]", value);
            }

            public void SetContentType(string value)
            {
                sb.AppendFormat(" [ContentType: {0}]", value);
            }

            public void SetLocation(string value)
            {
                sb.AppendFormat(" [Location: {0}]", value);
            }

            public void SetLastModified(DateTimeOffset value)
            {
                sb.AppendFormat(" [LastModified: {0}]", value.ToString("R"));
            }

            public void SetETag(string value)
            {
                sb.AppendFormat(" [ETag: {0}]", value);
            }

            public void SetContentLocation(string value)
            {
                sb.AppendFormat(" [ContentLocation: {0}]", value);
            }
        }
    }

    public class Response<T> : Response where T : class
    {
        private readonly T entityBody;

        public Response(IStatus status) : this(status, headers => { }, null)
        {
        }

        public Response(IStatus status, Action<ResponseHeaders> headersAction) : this(status, headersAction, null)
        {
        }

        public Response(IStatus status, Action<ResponseHeaders> headersAction, T entityBody) : base(status, headersAction)
        {
            this.entityBody = entityBody;
        }

        public T EntityBody
        {
            get { return entityBody; }
        }
    }
}