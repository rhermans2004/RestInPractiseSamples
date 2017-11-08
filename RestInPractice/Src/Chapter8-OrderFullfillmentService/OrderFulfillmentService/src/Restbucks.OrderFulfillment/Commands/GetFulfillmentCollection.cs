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
    public class GetFulfillmentCollection
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepository repository;

        public GetFulfillmentCollection(IRepository repository)
        {
            this.repository = repository;
        }

        public Response<Atom10FeedFormatter<Collection>> Execute(IRequest request, NameValueCollection parameters)
        {
            Log.Debug("Getting fulfillment collection.");
            
            FulfillmentCollection collection = new FulfillmentCollection(DateTime.Now, request.Uri);
            collection.Add(repository.GetAll());

            return new Response<Atom10FeedFormatter<Collection>>(
                Status.OK,
                headers => headers
                               .Add(new ContentType(MediaTypes.AtomFeed)),
                collection.GetAtomFormatter());
        }
    }
}