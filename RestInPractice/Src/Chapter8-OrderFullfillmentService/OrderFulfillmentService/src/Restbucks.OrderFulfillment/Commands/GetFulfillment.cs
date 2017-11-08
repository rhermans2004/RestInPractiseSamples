using System;
using System.Collections.Specialized;
using System.Reflection;
using System.ServiceModel.Syndication;
using log4net;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit;
using Restbucks.WcfRestToolkit.Http;
using Restbucks.WcfRestToolkit.Http.Headers;
using Restbucks.WcfRestToolkit.Http.StatusCodes;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Commands
{
    public class GetFulfillment
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepository repository;

        public GetFulfillment(IRepository repository)
        {
            this.repository = repository;
        }

        public Response<Atom10ItemFormatter<Member>> Execute(IRequest request, NameValueCollection parameters)
        {
            Log.DebugFormat("Getting fulfillment. Id: [{0}].", parameters["id"]);
            
            ETaggedEntity eTaggedEntity;

            try
            {
                eTaggedEntity = repository.Get(new Guid(parameters["id"]));
            }
            catch (MemberDoesNotExistException)
            {
                return new Response<Atom10ItemFormatter<Member>>(Status.NotFound);
            }

            if (eTaggedEntity.EntityTag.Equals(request.Headers.IfNoneMatch))
            {
                return new Response<Atom10ItemFormatter<Member>>(
                    Status.NotModified,
                    headers => headers.Add(new ETag(eTaggedEntity.EntityTag)));
            }

            return new Response<Atom10ItemFormatter<Member>>(
                Status.OK,
                headers => headers
                               .Add(new ContentType(MediaTypes.AtomEntry))
                               .Add(new ETag(eTaggedEntity.EntityTag)),
                eTaggedEntity.Fulfillment.GetAtomFormatter());
        }
    }
}