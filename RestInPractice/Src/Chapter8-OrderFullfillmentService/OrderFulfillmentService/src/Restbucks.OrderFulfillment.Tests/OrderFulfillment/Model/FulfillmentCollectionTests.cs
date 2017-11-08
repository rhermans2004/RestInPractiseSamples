using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Restbucks.WcfRestToolkit.Utility;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Model
{
    [TestFixture]
    public class FulfillmentCollectionTests
    {
        [Test]
        public void ShouldCreateFulfillmentSyndicationFeedWithAtomPubCollectionExtension()
        {
            const string expectedServiceName = "Order Fulfillment Service";

            Uri uri = new Uri("http://localhost/fulfillment");

            FulfillmentCollection collection = new FulfillmentCollectionBuilder()
                .WithCreatedDateTime(DateTimes.OnePM)
                .WithBaseUri(uri)
                .Build();
            Collection feed = (Collection) collection.GetAtomFormatter().Feed;

            Assert.AreEqual(0, feed.Items.Count());
            Assert.AreEqual(expectedServiceName, feed.Title.Text);
            Assert.AreEqual(expectedServiceName, feed.Generator);
            Assert.IsTrue(feed.Id.StartsWith("urn:uuid:"));
            Assert.AreEqual(DateTimes.OnePM, feed.LastUpdatedTime);

            Assert.AreEqual(1, feed.Links.Count());
            Assert.AreEqual(uri, feed.GetSelfLink().GetAbsoluteUri());

            //app:collection extension
            Assert.AreEqual(uri, feed.CollectionExtension.Href);
            Assert.AreEqual(expectedServiceName, feed.CollectionExtension.Title);
            Assert.AreEqual("application/atom+xml;type=entry", feed.CollectionExtension.Accept.First());
        }

        [Test]
        public void WhenMemberIsAddedUpdatedTimeOfCollectionShouldChangeToReflectEditedDateTimeOfMember()
        {
            FulfillmentCollection collection = new FulfillmentCollectionBuilder().WithCreatedDateTime(DateTimes.OnePM).Build();
            Fulfillment fulfillment = FulfillmentBuilder.BuildEditedFulfillment(DateTimes.TwoPM, DateTimes.ThreePM);

            collection.Add(new[] {fulfillment});

            SyndicationFeed underlyingCollection = collection.GetAtomFormatter().Feed;

            Assert.AreEqual(1, underlyingCollection.Items.LongCount());
            Assert.AreEqual(DateTimes.ThreePM, underlyingCollection.LastUpdatedTime);
        }

        [Test]
        public void WhenMultipleMembersAreAddedUpdatedTimeOfCollectionShouldChangeToReflectEditedDateTimeOfMostRecentlyAddedMember()
        {
            FulfillmentCollection collection = new FulfillmentCollectionBuilder().WithCreatedDateTime(DateTimes.OnePM).Build();
            collection.Add(FulfillmentBuilder.BuildEditedFulfillmentList(DateTimes.TwoPM, DateTimes.ThreePM, DateTimes.FivePM, DateTimes.FourPM));

            SyndicationFeed underlyingCollection = collection.GetAtomFormatter().Feed;

            Assert.AreEqual(3, underlyingCollection.Items.LongCount());
            Assert.AreEqual(DateTimes.FivePM, underlyingCollection.LastUpdatedTime);
        }

        [Test]
        public void ItemsShouldBeSortedMostRecentlyEditedFirst()
        {
            FulfillmentCollection collection = new FulfillmentCollectionBuilder().WithCreatedDateTime(DateTimes.OnePM).Build();

            collection.Add(new[] {FulfillmentBuilder.BuildEditedFulfillment(DateTimes.TwoPM, DateTimes.FourPM)});
            collection.Add(FulfillmentBuilder.BuildEditedFulfillmentList(DateTimes.TwoPM, DateTimes.FivePM, DateTimes.SixPM, DateTimes.ThreePM));

            SyndicationFeed underlyingCollection = collection.GetAtomFormatter().Feed;
            IList<SyndicationItem> items = (IList<SyndicationItem>)underlyingCollection.Items;

            Assert.AreEqual(4, items.Count);
            Assert.AreEqual(DateTimes.SixPM, ((Member) items[0]).EditedDateTime);
            Assert.AreEqual(DateTimes.FivePM, ((Member) items[1]).EditedDateTime);
            Assert.AreEqual(DateTimes.FourPM, ((Member) items[2]).EditedDateTime);
            Assert.AreEqual(DateTimes.ThreePM, ((Member) items[3]).EditedDateTime);
        }

        [Test]
        public void CanHandleTwoItemsWithSameEditedDateTime()
        {
            FulfillmentCollection collection = new FulfillmentCollectionBuilder().WithCreatedDateTime(DateTimes.OnePM).Build();

            collection.Add(FulfillmentBuilder.BuildEditedFulfillmentList(DateTimes.TwoPM, DateTimes.ThreePM, DateTimes.ThreePM));

            SyndicationFeed underlyingCollection = collection.GetAtomFormatter().Feed;
            IList<SyndicationItem> items = (IList<SyndicationItem>)underlyingCollection.Items;

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(DateTimes.ThreePM, ((Member) items[0]).EditedDateTime);
            Assert.AreEqual(DateTimes.ThreePM, ((Member) items[1]).EditedDateTime);
        }
    }
}