using System;
using System.Collections.Specialized;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using log4net;
using Restbucks.WcfRestToolkit.Http;
using Restbucks.WcfRestToolkit.Http.StatusCodes;
using Restbucks.WcfRestToolkit.WcfDecoupling;

namespace Restbucks.WcfRestToolkit
{
    public class Request
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static TResponseEntityBody Handle<TResponseEntityBody>(Func<IRequest, NameValueCollection, Response<TResponseEntityBody>> handleRequest) where TResponseEntityBody : class
        {
            return new Request(OperationContext.Current, WebOperationContext.Current).HandleImpl(handleRequest);
        }

        public static void Handle(Func<IRequest, NameValueCollection, Response> handleRequest)
        {
            new Request(OperationContext.Current, WebOperationContext.Current).HandleImpl(handleRequest);
        }

        private readonly OperationContext operationContext;
        private readonly WebOperationContext webOperationContext;

        protected Request(OperationContext operationContext, WebOperationContext webOperationContext)
        {
            this.operationContext = operationContext;
            this.webOperationContext = webOperationContext;
        }

        private TResponseEntityBody HandleImpl<TResponseEntityBody>(Func<IRequest, NameValueCollection, Response<TResponseEntityBody>> handleRequest) where TResponseEntityBody : class
        {
            var wcfRequest = new WcfRequest(operationContext, webOperationContext);

            Log.DebugFormat("Request:{0}.", wcfRequest.CreateSummary());

            try
            {
                var response = handleRequest(wcfRequest, GetParameters(webOperationContext));
                response.ApplyTo(GetResponseContext(webOperationContext));

                Log.DebugFormat("Response:{0}.", response.CreateSummary());

                return response.EntityBody;
            }
            catch (Exception ex)
            {
                Log.Error("Error handling request.", ex);
                
                var response = new Response(Status.ServerError);
                response.ApplyTo(GetResponseContext(webOperationContext));
                Log.DebugFormat("Response:{0}.", response.CreateSummary());

                return null;
            }
        }

        private void HandleImpl(Func<IRequest, NameValueCollection, Response> handleRequest)
        {
            var wcfRequest = new WcfRequest(operationContext, webOperationContext);

            Log.DebugFormat("Request:{0}.", wcfRequest.CreateSummary());

            try
            {
                var response = handleRequest(wcfRequest, GetParameters(webOperationContext));
                response.ApplyTo(GetResponseContext(webOperationContext));

                Log.DebugFormat("Response:{0}.", response.CreateSummary());
            }
            catch (Exception ex)
            {
                Log.Error("Error handling request.", ex);

                var response = new Response(Status.ServerError);
                response.ApplyTo(GetResponseContext(webOperationContext));
                Log.DebugFormat("Response:{0}.", response.CreateSummary());
            }
        }

        protected NameValueCollection GetParameters(WebOperationContext context)
        {
            return context.IncomingRequest.UriTemplateMatch.BoundVariables;
        }

        protected IResponseContext GetResponseContext(WebOperationContext context)
        {
            return new WcfResponseContext(context.OutgoingResponse);
        }
    }
}