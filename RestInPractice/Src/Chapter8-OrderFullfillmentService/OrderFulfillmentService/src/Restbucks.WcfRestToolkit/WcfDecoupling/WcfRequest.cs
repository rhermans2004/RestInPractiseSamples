using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Restbucks.WcfRestToolkit.Http;
using Restbucks.WcfRestToolkit.Http.Headers;

namespace Restbucks.WcfRestToolkit.WcfDecoupling
{
    public class WcfRequest : IRequest
    {
        private readonly Uri uri;
        private readonly IRequestHeaders headers;
        private readonly string method;

        public WcfRequest(OperationContext operationContext, WebOperationContext webOperationContext)
        {
            uri = GetUri(operationContext);
            headers = GetHeaders(webOperationContext);
            method = GetMethod(webOperationContext);
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public IRequestHeaders Headers
        {
            get { return headers; }
        }

        public string CreateSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" [Method: {0}]", method);
            sb.AppendFormat(" [Uri: {0}]", uri.AbsoluteUri);
            sb.Append(headers.CreateSummary());
            return sb.ToString();
        }

        protected static Uri GetUri(OperationContext context)
        {
            return context.EndpointDispatcher.EndpointAddress.Uri;
        }

        protected static IRequestHeaders GetHeaders(WebOperationContext context)
        {
            return new WcfRequestHeaders(context.IncomingRequest.Headers);
        }

        protected static string GetMethod(WebOperationContext context)
        {
            return context.IncomingRequest.Method;
        }
    }

    public class WcfRequest<T> : WcfRequest, IRequest<T>
    {
        private readonly T entityBody;

        public WcfRequest(OperationContext operationContext, WebOperationContext webOperationContext) : base(operationContext, webOperationContext)
        {
            entityBody = GetEntityBody(operationContext);
        }

        public T EntityBody
        {
            get { return entityBody; }
        }

        private T GetEntityBody(OperationContext context)
        {
            var storedMessage = context.Extensions.Find<StoredMessage>();
            if (storedMessage == null)
            {
                if (context.Host.Description.ServiceType.GetCustomAttributes(typeof(WcfDecouplingSupportAttribute), false).Length.Equals(0))
                {
                    throw new InvalidOperationException(string.Format("[{0}] attribute must be present on service implementation.", typeof(WcfDecouplingSupportAttribute).Name));
                }
                throw new NullReferenceException("Stored message was null.");
            }

            return storedMessage.Message.GetBody<T>();
        }
    }
}