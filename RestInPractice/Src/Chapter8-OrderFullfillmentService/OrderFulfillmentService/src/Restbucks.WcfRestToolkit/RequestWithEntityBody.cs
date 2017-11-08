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
    public class RequestWithEntityBody : Request
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static TResponseEntityBody Handle<TRequestEntityBody, TResponseEntityBody>(Func<IRequest<TRequestEntityBody>, NameValueCollection, Response<TResponseEntityBody>> handleRequest) where TResponseEntityBody : class
        {
            return new RequestWithEntityBody(OperationContext.Current, WebOperationContext.Current).HandleImpl(handleRequest);
        }

        public static void Handle<TRequestEntityBody>(Func<IRequest<TRequestEntityBody>, NameValueCollection, Response> handleRequest)
        {
            new RequestWithEntityBody(OperationContext.Current, WebOperationContext.Current).HandleImpl(handleRequest);
        }

        private readonly OperationContext operationContext;
        private readonly WebOperationContext webOperationContext;

        private RequestWithEntityBody(OperationContext operationContext, WebOperationContext webOperationContext) : base(operationContext, webOperationContext)
        {
            this.operationContext = operationContext;
            this.webOperationContext = webOperationContext;
        }

        private TResponseEntityBody HandleImpl<TRequestEntityBody, TResponseEntityBody>(Func<IRequest<TRequestEntityBody>, NameValueCollection, Response<TResponseEntityBody>> handleRequest) where TResponseEntityBody : class
        {
            var request = new WcfRequest<TRequestEntityBody>(operationContext, webOperationContext);

            Log.DebugFormat("Request:{0}.", request.CreateSummary());

            try
            {
                var response = handleRequest(request, GetParameters(webOperationContext));
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

        private void HandleImpl<TRequestEntityBody>(Func<IRequest<TRequestEntityBody>, NameValueCollection, Response> handleRequest)
        {
            var request = new WcfRequest<TRequestEntityBody>(operationContext, webOperationContext);

            Log.DebugFormat("Request:{0}.", request.CreateSummary());

            try
            {
                var response = handleRequest(request, GetParameters(webOperationContext));
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
    }
}