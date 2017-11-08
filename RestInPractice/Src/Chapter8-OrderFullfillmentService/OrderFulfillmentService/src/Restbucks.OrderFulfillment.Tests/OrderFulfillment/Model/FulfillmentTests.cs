using System;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Restbucks.WcfRestToolkit.Utility;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Model
{
    [TestFixture]
    public class FulfillmentTests
    {
        [Test]
        public void ShouldReturnId()
        {
            Guid id = Guid.NewGuid();
            Fulfillment fulfillment = new FulfillmentBuilder().WithId(id).Build();

            Assert.AreEqual(id, fulfillment.Id);
        }

        [Test]
        public void ShouldReturnEditUri()
        {
            Guid id = Guid.NewGuid();
            Uri baseUri = new Uri("http://localhost/fulfillment");
            Fulfillment fulfillment = new FulfillmentBuilder().WithId(id).WithBaseUri(baseUri).Build();

            Assert.AreEqual(new Uri(baseUri.AbsoluteUri + "/" + id.ToString("N")), fulfillment.EditUri);
        }

        [Test]
        public void ShouldReturnEditedDateTime()
        {
            Fulfillment fulfillment = FulfillmentBuilder.BuildEditedFulfillment(DateTimes.OnePM, DateTimes.TwoPM);
            Assert.AreEqual(DateTimes.TwoPM, fulfillment.EditedDateTime);
        }

        [Test]
        public void ShouldCreateFulfillmentSyndicationItemWithEditedAndControlExtensionsAndEditLink()
        {
            Guid id = Guid.Empty;
            DateTimeOffset createdDateTime = DateTimes.OnePM;
            Order order = new Order {Id = 23};
            Uri uri = new Uri("http://localhost/fulfillment");

            Fulfillment fulfillment = new FulfillmentBuilder().WithId(id).WithCreatedDateTime(createdDateTime).WithOrder(order).WithBaseUri(uri).WithAgent("Cashier").Build();
            Member member = (Member) fulfillment.GetAtomFormatter().Item;

            Assert.AreEqual("urn:uuid:" + id.ToString("D"), member.Id);
            Assert.AreEqual("order", member.Title.Text);
            Assert.AreEqual(1, member.Authors.Count);
            Assert.AreEqual("Cashier", member.Authors[0].Name);
            Assert.AreEqual(createdDateTime, member.LastUpdatedTime);
            Assert.AreEqual(MediaTypes.Restbucks.TypeAndSubtype, member.Content.Type);
            Assert.AreEqual(23, ((XmlSyndicationContent) member.Content).ReadContent<Order>().Id);

            //AtomPub elements
            Assert.AreEqual(1, member.Links.Count);
            Assert.AreEqual("edit", member.GetEditLink().RelationshipType);
            Assert.AreEqual("http://localhost/fulfillment/00000000000000000000000000000000", member.GetEditLink().GetAbsoluteUri().AbsoluteUri);
            Assert.AreEqual(DraftStatus.Yes, member.Draft);
            Assert.AreEqual(DateTimes.OnePM, member.EditedDateTime);
        }

        [Test]
        public void WhenFulfillmentIsEditedTheEditedDateTimeOfTheMemberShouldBeUpdated()
        {
            Fulfillment fulfillment = new FulfillmentBuilder().WithCreatedDateTime(DateTimes.OnePM).Build();
            fulfillment.EditedDateTime = DateTimes.TwoPM;

            Member member = (Member) fulfillment.GetAtomFormatter().Item;

            Assert.AreEqual(DateTimes.TwoPM, member.EditedDateTime);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidOperationException))]
        public void ShouldThrowExceptionIfAttemptingToEditFulfillmentThatIsNoLongerInDraft()
        {
            Fulfillment fulfillment = FulfillmentBuilder.BuildNonDraftFulfillment(DateTimes.FourPM);
            fulfillment.Edit(new Member {Draft = DraftStatus.No}, DateTimes.SixPM);
        }
    }
}