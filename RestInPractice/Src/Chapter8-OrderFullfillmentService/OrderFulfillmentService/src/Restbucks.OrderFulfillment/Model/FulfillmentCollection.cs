using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Model
{
    public class FulfillmentCollection
    {
        private const string ServiceName = "Order Fulfillment Service";
        private static readonly Guid Id = Guid.NewGuid();

        private readonly Collection collection;
        private readonly List<SyndicationItem> members;

        public FulfillmentCollection(DateTimeOffset createdDateTime, Uri uri)
        {
            collection = new Collection
                   {
                       Title = SyndicationContent.CreatePlaintextContent(ServiceName),
                       LastUpdatedTime = createdDateTime,
                       Generator = ServiceName,
                       Id = new UniqueId(Id).ToString(),
                       CollectionExtension = new CollectionExtension
                                             {
                                                 Href = uri.AbsoluteUri,
                                                 Title = ServiceName,
                                                 Accept = new[] {MediaTypes.AtomEntry.TypeAndSubtypeAndParameters}
                                             }
                   };

            collection.Links.Add(SyndicationLink.CreateSelfLink(uri));

            members = new List<SyndicationItem>();
            collection.Items = members;
        }

        public Atom10FeedFormatter<Collection> GetAtomFormatter()
        {
            return new Atom10FeedFormatter<Collection>(collection);
        }

        public void Add(IEnumerable<Fulfillment> newMembers)
        {
            foreach (Fulfillment member in newMembers)
            {
                member.DoAction(i => members.Add(i));
            }

            members.Sort((x, y) =>
                       ((Member) y).EditedDateTime.CompareTo(
                           ((Member) x).EditedDateTime));

            if (members.Count > 0)
            {
                collection.LastUpdatedTime = ((Member) members.First()).EditedDateTime;
            }
        }
    }
}