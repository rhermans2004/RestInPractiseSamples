using System;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Model;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Model
{
    [TestFixture]
    public class EditLinkTests
    {
        [Test]
        public void ShouldAddEditLinkRelationValue()
        {
            EditLink editLink = new EditLink(new Uri("http://localhost/fulfillment/"), Guid.NewGuid());
            SyndicationLink link = editLink.ToSyndicationLink();

            Assert.AreEqual("edit", link.RelationshipType);
        }

        [Test]
        public void FormatsUriCorrectlyWhenBaseUriHasTrailingSlash()
        {
            Guid id = Guid.NewGuid();
            
            EditLink editLink = new EditLink(new Uri("http://localhost/fulfillment/"), id);
            SyndicationLink link = editLink.ToSyndicationLink();

            Assert.AreEqual("http://localhost/fulfillment/" + id.ToString("N"), link.Uri.AbsoluteUri);
        }

        [Test]
        public void FormatsUriCorrectlyWhenBaseUriDoesNotHaveTrailingSlash()
        {
            Guid id = Guid.NewGuid();
            
            EditLink editLink = new EditLink(new Uri("http://localhost/fulfillment"), id);
            SyndicationLink link = editLink.ToSyndicationLink();

            Assert.AreEqual("http://localhost/fulfillment/" + id.ToString("N"), link.Uri.AbsoluteUri);
        }
    }
}