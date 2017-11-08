using System.Net;
using System.Text;
using Restbucks.WcfRestToolkit.Http.Headers;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Authorization=Restbucks.WcfRestToolkit.Http.HeaderValues.Authorization;

namespace Restbucks.WcfRestToolkit.WcfDecoupling
{
    public class WcfRequestHeaders : IRequestHeaders
    {
        private readonly WebHeaderCollection headers;

        public WcfRequestHeaders(WebHeaderCollection headers)
        {
            this.headers = headers;
        }

        public EntityTag IfNoneMatch
        {
            get { return EntityTag.Parse(headers[HttpRequestHeader.IfNoneMatch]); }
        }

        public EntityTag IfMatch
        {
            get { return EntityTag.Parse(headers[HttpRequestHeader.IfMatch]); }
        }

        public MediaType ContentType
        {
            get { return MediaType.Parse(headers[HttpRequestHeader.ContentType]); }
        }

        public Authorization Authorization
        {
            get { return new Authorization(headers[HttpRequestHeader.Authorization]); }
        }

        public string CreateSummary()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string headerName in headers.AllKeys)
            {
                sb.AppendFormat(" [{0}: {1}]", headerName, headers[headerName]);
            }
            return sb.ToString();
        }
    }
}