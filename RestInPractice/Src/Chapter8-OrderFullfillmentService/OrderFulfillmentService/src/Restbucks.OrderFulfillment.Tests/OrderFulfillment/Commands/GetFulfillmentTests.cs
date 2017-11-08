using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Restbucks.WcfRestToolkit.Http.Testing;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Commands
{
    [TestFixture]
    public class GetFulfillmentTests
    {
        [Test]
        public void ShouldReturnMemberWithEtag()
        {
            Guid id = Guid.NewGuid();
            EntityTag entityTag = EntityTag.CreateWithRandomValue();
            ETaggedEntity eTaggedEntity = new ETaggedEntity(entityTag, FulfillmentBuilder.NewFulfillment());

            IRepository repository = FakeRepository.ForGet(id, eTaggedEntity);

            string uri = "http://localhost/fulfillment/" + id.ToString("N");
            NameValueCollection parameters = new NameValueCollection {{"id", id.ToString("N")}};

            GetFulfillment command = new GetFulfillment(repository);

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(FakeRequest.For(uri), parameters));

            Assert.AreEqual(200, responseContext.StatusCode);
            Assert.AreEqual("OK", responseContext.StatusDescription);
            Assert.AreEqual(MediaTypes.AtomEntry.TypeAndSubtypeAndParameters, responseContext.ContentType);
            Assert.AreEqual(entityTag.Value, responseContext.ETag);
        }

        [Test]
        public void ConditionalRequestForUnmodifedMemberShouldReturn304NotModifiedAndETagHeader()
        {
            Guid id = Guid.NewGuid();
            string requestUri = "http://localhost/fulfillment/" + id.ToString("N");
            NameValueCollection parameters = new NameValueCollection {{"id", id.ToString("N")}};

            EntityTag entityTag = EntityTag.CreateWithRandomValue();
            ETaggedEntity eTaggedEntity = new ETaggedEntityBuilder().WithEntityTag(entityTag).Build();

            IRepository repository = FakeRepository.ForGet(id, eTaggedEntity);

            GetFulfillment command = new GetFulfillment(repository);

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          requestUri,
                          headers => headers.AddIfNoneMatch(entityTag)),
                          parameters));

            Assert.AreEqual(304, responseContext.StatusCode);
            Assert.AreEqual("Not Modified", responseContext.StatusDescription);
            Assert.AreEqual(entityTag.Value, responseContext.ETag);
        }

        [Test]
        public void IfMemberDoesNotExistShouldReturn404NotFound()
        {
            Guid id = Guid.NewGuid();
            string requestUri = "http://localhost/fulfillment/" + id.ToString("N");
            NameValueCollection parameters = new NameValueCollection {{"id", id.ToString("N")}};
            
            IRepository repository = FakeRepository.ForGetWhenMemberMissing(id);

            GetFulfillment command = new GetFulfillment(repository);

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          requestUri,
                          headers => headers.AddIfNoneMatch(EntityTag.CreateWithRandomValue())),
                          parameters));

            Assert.AreEqual(404, responseContext.StatusCode);
            Assert.AreEqual("Not Found", responseContext.StatusDescription);
        }
    }
}