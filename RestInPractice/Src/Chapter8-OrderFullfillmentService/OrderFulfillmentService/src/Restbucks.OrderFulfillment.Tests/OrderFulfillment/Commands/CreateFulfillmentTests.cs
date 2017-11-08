using System;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Restbucks.WcfRestToolkit.Http.Testing;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Rhino.Mocks;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Commands
{
    [TestFixture]
    public class CreateFulfillmentTests
    {
        private const string Uri = "http://localhost/fulfillment";

        [Test]
        public void WhenOrderIsReceivedShouldCreateNewMemberAndReturn201Created()
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            Guid id = Guid.NewGuid();
            EntityTag entityTag = EntityTag.CreateWithRandomValue();

            using (mocks.Record())
            {
                Expect.Call(repository.Add(FulfillmentBuilder.NewFulfillment()))
                    .IgnoreArguments()
                    .Do((Func<Fulfillment, ETaggedEntity>)
                        (f => new ETaggedEntityBuilder()
                                  .WithFulfillment(f)
                                  .WithEntityTag(entityTag)
                                  .Build()));
            }
            mocks.ReplayAll();

            CreateFulfillment command = new CreateFulfillment(repository, FakeIdGenerator.For(id), FakeDateTimeProvider.ForNow());

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          MemberBuilder.NewMember().GetAtom10Formatter(),
                          headers => headers.AddContentType(MediaTypes.AtomEntry)),
                          new NameValueCollection()));

            Assert.AreEqual(201, responseContext.StatusCode);
            Assert.AreEqual("Created", responseContext.StatusDescription);
            Assert.AreEqual(Uri + "/" + id.ToString("N"), responseContext.Location);
            Assert.AreEqual(Uri + "/" + id.ToString("N"), responseContext.ContentLocation);
            Assert.AreEqual(entityTag.Value, responseContext.ETag);
            Assert.AreEqual(MediaTypes.AtomEntry.TypeAndSubtypeAndParameters, responseContext.ContentType);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldSetFirstAuthorToSuppliedAgentName()
        {
            string agentName = "Cashier";

            CreateFulfillment command = new CreateFulfillment(FakeRepository.ForAdd(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.ForNow());

            Response<Atom10ItemFormatter<Member>> response = command.Execute(FakeRequest.For(
                Uri,
                new MemberBuilder().WithAuthor(agentName).Build().GetAtom10Formatter(),
                headers => headers.AddContentType(MediaTypes.AtomEntry)),
                new NameValueCollection());

            Assert.AreEqual(agentName, response.EntityBody.Item.Authors.First().Name);
        }

        [Test]
        public void WhenAuthorIsMissingFromSubmittedOrderShouldReturn400BadRequest()
        {
            CreateFulfillment command = new CreateFulfillment(FakeRepository.WithNoBehaviour(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.ForNow());

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          MemberBuilder.WithoutAuthor().GetAtom10Formatter(),
                          headers => headers.AddContentType(MediaTypes.AtomEntry)),
                          new NameValueCollection()));

            Assert.AreEqual(400, responseContext.StatusCode);
            Assert.AreEqual("Bad Request", responseContext.StatusDescription);
        }

        [Test]
        public void ShouldGenerateNewIdForReturnedMember()
        {
            Member member = MemberBuilder.NewMember();

            CreateFulfillment command = new CreateFulfillment(FakeRepository.ForAdd(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.ForNow());

            Response<Atom10ItemFormatter<Member>> response = command.Execute(FakeRequest.For(
                Uri,
                member.GetAtom10Formatter(),
                headers => headers.AddContentType(MediaTypes.AtomEntry)),
                new NameValueCollection());

            Assert.AreNotEqual(member.Id, response.EntityBody.Item.Id);
        }

        [Test]
        public void ShouldGenerateNewLastUpdatedDateTimeForReturnedMember()
        {
            Member member = new MemberBuilder().WithLastUpdatedDateTime(DateTimes.OnePM).Build();

            CreateFulfillment command = new CreateFulfillment(FakeRepository.ForAdd(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.For(DateTimes.TwoPM));

            Response<Atom10ItemFormatter<Member>> response = command.Execute(FakeRequest.For(
                Uri,
                member.GetAtom10Formatter(),
                headers => headers.AddContentType(MediaTypes.AtomEntry)),
                new NameValueCollection());

            Assert.AreEqual(DateTimes.TwoPM, response.EntityBody.Item.LastUpdatedTime);
        }

        [Test]
        public void IfSubmittedMemberDoesNotContainContentShouldReturn400BadRequest()
        {
            CreateFulfillment command = new CreateFulfillment(FakeRepository.WithNoBehaviour(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.ForNow());

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          MemberBuilder.WithoutContent().GetAtom10Formatter(),
                          headers => headers.AddContentType(MediaTypes.AtomEntry)),
                          new NameValueCollection()));

            Assert.AreEqual(400, responseContext.StatusCode);
            Assert.AreEqual("Bad Request", responseContext.StatusDescription);
        }

        [Test]
        public void WhenSentWrongContentTypeShouldReturn415UnsupportedMediaType()
        {
            CreateFulfillment command = new CreateFulfillment(FakeRepository.WithNoBehaviour(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.ForNow());

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          MemberBuilder.NewMember().GetAtom10Formatter(),
                          headers => headers.AddContentType(MediaType.Parse("application/xml"))),
                          new NameValueCollection()));

            Assert.AreEqual(415, responseContext.StatusCode);
            Assert.AreEqual("Unsupported Media Type", responseContext.StatusDescription);
        }

        [Test]
        public void IfContentTypeHeaderIsMissingShouldReturn415UnsupportedMediaType()
        {
            CreateFulfillment command = new CreateFulfillment(FakeRepository.WithNoBehaviour(), FakeIdGenerator.ForAnyId(), FakeDateTimeProvider.ForNow());

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          MemberBuilder.NewMember().GetAtom10Formatter()),
                          new NameValueCollection()));

            Assert.AreEqual(415, responseContext.StatusCode);
            Assert.AreEqual("Unsupported Media Type", responseContext.StatusDescription);
        }
    }
}