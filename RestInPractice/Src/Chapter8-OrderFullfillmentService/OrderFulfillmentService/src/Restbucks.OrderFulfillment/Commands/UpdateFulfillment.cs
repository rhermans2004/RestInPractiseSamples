using System;
using System.Collections.Specialized;
using System.Reflection;
using System.ServiceModel.Syndication;
using log4net;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit;
using Restbucks.WcfRestToolkit.Http;
using Restbucks.WcfRestToolkit.Http.StatusCodes;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Commands
{
    public class UpdateFulfillment
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly IRepository repository;
        private readonly IDateTimeProvider dateTimeProvider;

        public UpdateFulfillment(IRepository repository, IDateTimeProvider dateTimeProvider)
        {
            this.repository = repository;
            this.dateTimeProvider = dateTimeProvider;
        }

        public Response Execute(IRequest<Atom10ItemFormatter<Member>> request, NameValueCollection parameters)
        {
            Log.DebugFormat("Updating fulfillment. Id: [{0}].", parameters["id"]);
            
            if (request.Headers.IfMatch == null)
            {
                return new Response(Status.PreconditionFailed);
            }

            if (!MediaTypes.AtomEntry.IsTypeAndSubtypeMatch(request.Headers.ContentType))
            {
                return new Response(Status.UnsupportedMediaType);
            }

            try
            {
                ETaggedEntity eTaggedEntity = repository.Get(new Guid(parameters["id"]));

                if (!eTaggedEntity.EntityTag.Equals(request.Headers.IfMatch))
                {
                    return new Response(Status.PreconditionFailed);
                }

                Fulfillment fulfillment = eTaggedEntity.Fulfillment.Edit((Member)request.EntityBody.Item, dateTimeProvider.Now);
                repository.Update(fulfillment, request.Headers.IfMatch);
            }
            catch (MemberDoesNotExistException)
            {
                return new Response(Status.NotFound);
            }
            catch (OptimisticUpdateFailedException)
            {
                return new Response(Status.PreconditionFailed);
            }
            catch (InvalidOperationException)
            {
                return new Response(Status.Conflict);
            }

            return new Response(Status.OK);
        }
    }
}