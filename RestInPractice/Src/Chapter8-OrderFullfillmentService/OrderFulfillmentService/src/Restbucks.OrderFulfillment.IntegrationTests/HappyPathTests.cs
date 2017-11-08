using System;
using System.Linq;
using NUnit.Framework;
using Restbucks.OrderFulfillment.IntegrationTests.Utility;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.IntegrationTests
{
    [TestFixture]
    public class HappyPathTests
    {
        [Test]
        public void EndToEndTest()
        {
            Uri uri = new Uri("http://localhost/fulfillment/");

            Cashier cashier = new Cashier("cashier", uri);
            Order order1 = new Order {Id = 1};
            Order order2 = new Order {Id = 2};

            Barista barista1 = new Barista(uri);
            Barista barista2 = new Barista(uri);

            using (Host host = new Host(uri))
            {
                host.Start();

                //Cashier creates 2 orders
                Assert.AreEqual(201, cashier.SubmitOrder(order1).StatusCode);
                Assert.AreEqual(201, cashier.SubmitOrder(order2).StatusCode);

                //Barista 1 gets the oldest outstanding order from the feed
                Collection barista1_collection1 = barista1.GetFulfillmentCollection();
                Assert.AreEqual(2, barista1_collection1.Items.Count());
                Member barista1_member1 = barista1.IdentifyNextOrderAwaitingFulfillment(barista1_collection1);
                Assert.AreEqual(DraftStatus.Yes, barista1_member1.Draft);
                Order barista1_order1 = Order.FromSyndicationContent(barista1_member1.Content);
                Assert.AreEqual(1, barista1_order1.Id);

                //Barista 2 gets the same order from the feed
                Collection barista2_collection1 = barista2.GetFulfillmentCollection();
                Assert.AreEqual(2, barista2_collection1.Items.Count());
                Member barista2_member1 = barista2.IdentifyNextOrderAwaitingFulfillment(barista2_collection1);
                Assert.AreEqual(DraftStatus.Yes, barista2_member1.Draft);
                Order barista2_order1 = Order.FromSyndicationContent(barista2_member1.Content);
                Assert.AreEqual(1, barista2_order1.Id);

                //Barista 1 gets the lastest version of the oldest outstanding order
                MemberWrapper barista1_memberWrapper1 = barista1.GetLatestVersionOfOrder(barista1_member1);
                Assert.AreEqual(DraftStatus.Yes, barista1_memberWrapper1.Member.Draft);

                //Barista 2 gets the lastest version of the oldest outstanding order
                MemberWrapper barista2_memberWrapper1 = barista2.GetLatestVersionOfOrder(barista2_member1);
                Assert.AreEqual(DraftStatus.Yes, barista2_memberWrapper1.Member.Draft);

                //Barista 2 reserves the next order
                Assert.AreEqual(200, barista2.ReserveOrder(barista2_memberWrapper1).StatusCode);

                //Barista 1 attempts to reserve the next order
                Assert.AreEqual(412, barista1.ReserveOrder(barista1_memberWrapper1).StatusCode);

                //Barista 1 gets the next oldest outstanding order from the feed
                Member barista1_member2 = barista1.IdentifyNextOrderAwaitingFulfillment(barista1_collection1, barista1_member1);
                Assert.AreEqual(DraftStatus.Yes, barista1_member2.Draft);
                Order barista1_order2 = Order.FromSyndicationContent(barista1_member2.Content);
                Assert.AreEqual(2, barista1_order2.Id);

                //Barista 1 gets the lastest version of the next oldest outstanding order
                MemberWrapper barista1_memberWrapper2 = barista1.GetLatestVersionOfOrder(barista1_member2);
                Assert.AreEqual(DraftStatus.Yes, barista1_memberWrapper2.Member.Draft);

                //Barista 1 reserves the next order
                Assert.AreEqual(200, barista1.ReserveOrder(barista1_memberWrapper2).StatusCode);

                //Barista 1 completes the next oldest outstanding order
                Assert.AreEqual(200, barista1.CompleteOrder(barista1_memberWrapper2).StatusCode);

                //Barista 2 completes the oldest outstanding order
                Assert.AreEqual(200, barista1.CompleteOrder(barista2_memberWrapper1).StatusCode);

                //Ensure feed is empty
                Assert.AreEqual(0, barista1.GetFulfillmentCollection().Items.Count());

                host.Stop();
            }
        }
    }
}