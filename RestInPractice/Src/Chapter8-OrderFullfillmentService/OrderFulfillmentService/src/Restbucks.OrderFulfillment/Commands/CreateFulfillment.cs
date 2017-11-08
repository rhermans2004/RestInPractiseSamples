using System.Collections.Specialized;
using System.Linq;
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
    public class CreateFulfillment
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepository repository;
        private readonly IIdGenerator idGenerator;
        private readonly IDateTimeProvider dateTimeProvider;

        public CreateFulfillment(IRepository repository, IIdGenerator idGenerator, IDateTimeProvider dateTimeProvider)
        {
            this.repository = repository;
            this.idGenerator = idGenerator;
            this.dateTimeProvider = dateTimeProvider;
        }

        public Response<Atom10ItemFormatter<Member>> Execute(IRequest<Atom10ItemFormatter> request, NameValueCollection parameters)
        {
            Log.Debug("Creating fulfillment.");
            
            if (!MediaTypes.AtomEntry.IsTypeAndSubtypeMatch(request.Headers.ContentType))
            {
                return new Response<Atom10ItemFormatter<Member>>(
                    Status.UnsupportedMediaType);
            }

            SyndicationItem item = request.EntityBody.Item;

            if (item.Authors.Count != 1)
            {
                return new Response<Atom10ItemFormatter<Member>>(
                    Status.BadRequest);
            }

            if (item.Content == null)
            {
                return new Response<Atom10ItemFormatter<Member>>(
                    Status.BadRequest);
            }

            Fulfillment fulfillment = new Fulfillment(idGenerator.NewId(),
                dateTimeProvider.Now,
                item.Content,
                request.Uri,
                item.Authors.First().Name);

            ETaggedEntity eTaggedEntity = repository.Add(fulfillment);

            return new Response<Atom10ItemFormatter<Member>>(
                Status.Created(eTaggedEntity.Fulfillment.EditUri),
                headers => headers
                               .Add(new ContentType(MediaTypes.AtomEntry))
                               .Add(new ETag(eTaggedEntity.EntityTag))
                               .Add(new ContentLocation(eTaggedEntity.Fulfillment.EditUri)),
                eTaggedEntity.Fulfillment.GetAtomFormatter());
        }
    }
}