using System;
using System.Collections.Specialized;
using System.Reflection;
using log4net;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit;
using Restbucks.WcfRestToolkit.Http;
using Restbucks.WcfRestToolkit.Http.StatusCodes;

namespace Restbucks.OrderFulfillment.Commands
{
    public class DeleteFulfillment
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepository repository;

        public DeleteFulfillment(IRepository repository)
        {
            this.repository = repository;
        }

        public Response Execute(IRequest request, NameValueCollection parameters)
        {
            Log.DebugFormat("Deleting fulfillment. Id: [{0}].", parameters["id"]);

            try
            {
                repository.Remove(new Guid(parameters["id"]));
                return new Response(Status.OK);
            }
            catch (MemberNoLongerExistsException)
            {
                return  new Response(Status.Gone);
            }            
        }
    }
}