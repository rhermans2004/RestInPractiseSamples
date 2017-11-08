using System;
using System.Linq;
using NUnit.Framework;
using Restbucks.OrderFulfillment.IntegrationTests.Utility;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.IntegrationTests
{
    [TestFixture]
    public class UnhappyPathTests
    {
        [Test]
        public void CashierCancelsOrderWhilstItIsInProgress()
        {
            Uri uri = new Uri("http://localhost/fulfillment/");

            Cashier cashier = new Cashier("cashier", uri);
            Order order = new Order {Id = 1};

            Barista barista = new Barista(uri);

            using (Host host = new Host(uri))
            {
                host.Start();

                //Cashier creates an order
                MemberWrapper cashier1_memberWrapper1 = cashier.SubmitOrder(order);
                Assert.AreEqual(201, cashier1_memberWrapper1.StatusCode);
                Assert.AreEqual(1, Order.FromSyndicationContent(cashier1_memberWrapper1.Member.Content).Id);

                //Barista gets the feed
                Collection barista1_collection1 = barista.GetFulfillmentCollection();
                Assert.AreEqual(1, barista1_collection1.Items.Count());

                //Barista identifies the oldest outstanding order in the feed
                Member barista1_member1 = barista.IdentifyNextOrderAwaitingFulfillment(barista1_collection1);
                Assert.AreEqual(DraftStatus.Yes, barista1_member1.Draft);
                Order barista1_order1 = Order.FromSyndicationContent(barista1_member1.Content);
                Assert.AreEqual(1, barista1_order1.Id);

                //Barista gets the lastest version of the oldest outstanding order
                MemberWrapper barista1_memberWrapper1 = barista.GetLatestVersionOfOrder(barista1_member1);
                Assert.AreEqual(DraftStatus.Yes, barista1_memberWrapper1.Member.Draft);

                //Barista reserves the order
                Assert.AreEqual(200, barista.ReserveOrder(barista1_memberWrapper1).StatusCode);

                //Cashier cancels the order
                Assert.AreEqual(200, cashier.CancelOrder(cashier1_memberWrapper1).StatusCode);

                //Barista attempts to complete order
                Assert.AreEqual(410, barista.CompleteOrder(barista1_memberWrapper1).StatusCode);

                host.Stop();
            }
        }

        [Test]
        public void CashierTriesToUpdateOrderInProgress()
        {
            Uri uri = new Uri("http://localhost/fulfillment/");

            Cashier cashier = new Cashier("cashier", uri);
            Order order = new Order {Id = 1};

            Barista barista = new Barista(uri);

            using (Host host = new Host(uri))
            {
                host.Start();

                //Cashier creates an order
                MemberWrapper cashier1_memberWrapper1 = cashier.SubmitOrder(order);
                Assert.AreEqual(201, cashier1_memberWrapper1.StatusCode);
                Assert.AreEqual(1, Order.FromSyndicationContent(cashier1_memberWrapper1.Member.Content).Id);

                //Barista gets the feed
                Collection barista1_collection1 = barista.GetFulfillmentCollection();
                Assert.AreEqual(1, barista1_collection1.Items.Count());

                //Barista identifies the oldest outstanding order in the feed
                Member barista1_member1 = barista.IdentifyNextOrderAwaitingFulfillment(barista1_collection1);
                Assert.AreEqual(DraftStatus.Yes, barista1_member1.Draft);
                Order barista1_order1 = Order.FromSyndicationContent(barista1_member1.Content);
                Assert.AreEqual(1, barista1_order1.Id);

                //Barista gets the lastest version of the oldest outstanding order
                MemberWrapper barista1_memberWrapper1 = barista.GetLatestVersionOfOrder(barista1_member1);
                Assert.AreEqual(DraftStatus.Yes, barista1_memberWrapper1.Member.Draft);

                //Barista reserves the order
                Assert.AreEqual(200, barista.ReserveOrder(barista1_memberWrapper1).StatusCode);

                //Cashier attempts to update order, but gets 412 Precondition Failed
                Assert.AreEqual(412, cashier.UpdateOrder(cashier1_memberWrapper1).StatusCode);

                //Cashier gets latest version of order
                MemberWrapper cashier1_memberWrapper2 = cashier.GetLatestVersionOfOrder(cashier1_memberWrapper1.Member);

                //Ignoring the fact that order is no longer in draft, cashier attempts to modify again
                Assert.AreEqual(409, cashier.UpdateOrder(cashier1_memberWrapper2).StatusCode);

                host.Stop();
            }
        }
    }
}