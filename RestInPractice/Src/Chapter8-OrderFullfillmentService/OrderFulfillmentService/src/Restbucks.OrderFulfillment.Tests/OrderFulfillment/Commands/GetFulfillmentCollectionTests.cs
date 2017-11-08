using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit;
using Restbucks.WcfRestToolkit.Http.Testing;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Commands
{
    [TestFixture]
    public class GetFulfillmentCollectionTests
    {
        private const string Uri = "http://localhost/fulfillment";

        [Test]
        public void ShouldReturn200OK()
        {
            IRepository repository = FakeRepository.ForGetAll(new Fulfillment[] {});

            GetFulfillmentCollection command = new GetFulfillmentCollection(repository);
            
            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(FakeRequest.For(Uri), new NameValueCollection()));

            Assert.AreEqual(200, responseContext.StatusCode);
            Assert.AreEqual("OK", responseContext.StatusDescription);
            Assert.AreEqual(MediaTypes.AtomFeed.TypeAndSubtypeAndParameters, responseContext.ContentType);
        }

        [Test]
        public void WhenThereAreNoOrdersShouldReturnOnlyFeedMetadata()
        {
            IRepository repository = FakeRepository.ForGetAll(new Fulfillment[] {});

            GetFulfillmentCollection command = new GetFulfillmentCollection(repository);
            Response<Atom10FeedFormatter<Collection>> response = command.Execute(FakeRequest.For(Uri), new NameValueCollection());

            SyndicationFeed feed = response.EntityBody.Feed;

            Assert.AreEqual(0, feed.Items.Count());
        }

        [Test]
        public void WhenThereAreOrdersShouldReturnCollectionWhoseUpdatedDatetimeIsSameAsMostRecentlyEditedMember()
        {
            IEnumerable<Fulfillment> fulfillmentList = FulfillmentBuilder.BuildEditedFulfillmentList(DateTimes.OnePM,
                DateTimes.ThreePM, DateTimes.FivePM, DateTimes.TwoPM, DateTimes.SixPM, DateTimes.FourPM);

            IRepository repository = FakeRepository.ForGetAll(fulfillmentList);

            GetFulfillmentCollection command = new GetFulfillmentCollection(repository);
            Response<Atom10FeedFormatter<Collection>> response = command.Execute(FakeRequest.For(Uri), new NameValueCollection());

            SyndicationFeed feed = response.EntityBody.Feed;

            Assert.AreEqual(5, feed.Items.Count());
            Assert.AreEqual(DateTimes.SixPM, feed.LastUpdatedTime);
        }
    }
}