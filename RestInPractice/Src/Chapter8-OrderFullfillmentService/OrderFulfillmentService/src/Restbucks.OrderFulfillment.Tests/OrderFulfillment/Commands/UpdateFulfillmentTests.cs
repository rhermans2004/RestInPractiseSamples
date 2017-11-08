using System;
using System.Collections.Specialized;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Restbucks.WcfRestToolkit.Http.Testing;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Rhino.Mocks;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Commands
{
    [TestFixture]
    public class UpdateFulfillmentTests
    {
        private const string Uri = "http://localhost/fulfillment";

        [Test]
        public void IfRequestDoesNotIncludeIfMatchHeaderShouldReturn412PreconditionFailed()
        {
            UpdateFulfillment command = new UpdateFulfillment(FakeRepository.WithNoBehaviour(), FakeDateTimeProvider.ForNow());
            Member member = (Member) FulfillmentBuilder.NewFulfillment().GetAtomFormatter().Item;

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers.AddContentType(MediaTypes.AtomEntry.WithTypeAndSubtypeOnly())),
                          new NameValueCollection()));

            Assert.AreEqual(412, responseContext.StatusCode);
            Assert.AreEqual("Precondition Failed", responseContext.StatusDescription);
        }

        [Test]
        public void IfRequestDoesNotIncludeContentTypeHeaderShouldReturn415UnsupportedMediaType()
        {
            UpdateFulfillment command = new UpdateFulfillment(FakeRepository.WithNoBehaviour(), FakeDateTimeProvider.ForNow());
            Member member = (Member) FulfillmentBuilder.NewFulfillment().GetAtomFormatter().Item;

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers.AddIfMatch(EntityTag.CreateWithRandomValue())),
                          new NameValueCollection()));

            Assert.AreEqual(415, responseContext.StatusCode);
            Assert.AreEqual("Unsupported Media Type", responseContext.StatusDescription);
        }

        [Test]
        public void IfRequestIncludesUnsupportedContentTypeHeaderShouldReturn415UnsupportedMediaType()
        {
            UpdateFulfillment command = new UpdateFulfillment(FakeRepository.WithNoBehaviour(), FakeDateTimeProvider.ForNow());
            Member member = (Member) FulfillmentBuilder.NewFulfillment().GetAtomFormatter().Item;

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers
                                         .AddIfMatch(EntityTag.CreateWithRandomValue())
                                         .AddContentType(MediaTypes.Restbucks)),
                          new NameValueCollection()));

            Assert.AreEqual(415, responseContext.StatusCode);
            Assert.AreEqual("Unsupported Media Type", responseContext.StatusDescription);
        }

        [Test]
        public void IfETagIsNoLongerValidWhenFulfillmentPulledFromRepositoryShouldReturn412PreconditionFailed()
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();
            Member member = (Member) fulfillment.GetAtomFormatter().Item;

            UpdateFulfillment command = new UpdateFulfillment(repository, FakeDateTimeProvider.ForNow());

            using (mocks.Record())
            {
                Expect
                    .Call(repository.Get(fulfillment.Id))
                    .Return(new ETaggedEntity(EntityTag.CreateWithRandomValue(), fulfillment));
            }
            mocks.ReplayAll();

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers
                                         .AddIfMatch(EntityTag.CreateWithRandomValue())
                                         .AddContentType(MediaTypes.AtomEntry.WithTypeAndSubtypeOnly())),
                          new NameValueCollection {{"id", fulfillment.Id.ToString("N")}}));

            Assert.AreEqual(412, responseContext.StatusCode);
            Assert.AreEqual("Precondition Failed", responseContext.StatusDescription);

            mocks.VerifyAll();
        }

        [Test]
        public void IfETagIsNoLongerValidWhenPuttingBackToRepositoryRepositoryShouldReturn412PreconditionFailed()
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            EntityTag entityTag = EntityTag.CreateWithRandomValue();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();
            Member member = (Member)fulfillment.GetAtomFormatter().Item;

            UpdateFulfillment command = new UpdateFulfillment(repository, FakeDateTimeProvider.ForNow());

            using (mocks.Record())
            {
                Expect
                    .Call(repository.Get(fulfillment.Id))
                    .Return(new ETaggedEntity(entityTag, fulfillment));
                Expect.Call(() => repository.Update(null, null))
                    .IgnoreArguments()
                    .Do((Action<Fulfillment, EntityTag>)
                        ((m, e) =>
                        {
                            Assert.AreEqual(m.Id, fulfillment.Id);
                            Assert.AreEqual(e, entityTag);
                            throw new OptimisticUpdateFailedException();
                        }));
            }
            mocks.ReplayAll();

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers
                                         .AddIfMatch(entityTag)
                                         .AddContentType(MediaTypes.AtomEntry.WithTypeAndSubtypeOnly())),
                          new NameValueCollection { { "id", fulfillment.Id.ToString("N") } }));

            Assert.AreEqual(412, responseContext.StatusCode);
            Assert.AreEqual("Precondition Failed", responseContext.StatusDescription);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldUpdateEditedDateTimeOnFulfillmentMemberBeforeSubmittingToRepository()
        {
            DateTimeOffset createdDateTime = DateTimes.OnePM;
            DateTimeOffset originalEditedDateTime = DateTimes.TwoPM;
            DateTimeOffset updatedEditedDateTime = DateTimes.ThreePM;

            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            EntityTag entityTag = EntityTag.CreateWithRandomValue();
            Fulfillment fulfillment = FulfillmentBuilder.BuildEditedFulfillment(createdDateTime, originalEditedDateTime);
            Member member = (Member) fulfillment.GetAtomFormatter().Item;

            Assert.AreEqual(originalEditedDateTime, fulfillment.EditedDateTime);

            UpdateFulfillment command = new UpdateFulfillment(repository, FakeDateTimeProvider.For(updatedEditedDateTime));

            using (mocks.Record())
            {
                Expect
                    .Call(repository.Get(fulfillment.Id))
                    .Return(new ETaggedEntity(entityTag, fulfillment));
                Expect.Call(() => repository.Update(null, null))
                    .IgnoreArguments()
                    .Do((Action<Fulfillment, EntityTag>)
                        ((m, e) => Assert.AreEqual(updatedEditedDateTime, m.EditedDateTime)));
            }
            mocks.ReplayAll();

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers
                                         .AddIfMatch(entityTag)
                                         .AddContentType(MediaTypes.AtomEntry.WithTypeAndSubtypeOnly())),
                          new NameValueCollection {{"id", fulfillment.Id.ToString("N")}}));

            Assert.AreEqual(200, responseContext.StatusCode);
            Assert.AreEqual("OK", responseContext.StatusDescription);

            mocks.VerifyAll();
        }

        [Test]
        public void IfFulfillmentMemberDoesNotExistShouldReturn404NotFound()
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            EntityTag entityTag = EntityTag.CreateWithRandomValue();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();
            Member member = (Member) fulfillment.GetAtomFormatter().Item;

            UpdateFulfillment command = new UpdateFulfillment(repository, FakeDateTimeProvider.ForNow());

            using (mocks.Record())
            {
                Expect
                    .Call(repository.Get(fulfillment.Id))
                    .Return(new ETaggedEntity(entityTag, fulfillment));
                Expect.Call(() => repository.Update(null, null))
                    .IgnoreArguments()
                    .Do((Action<Fulfillment, EntityTag>)
                        ((m, e) => { throw new MemberDoesNotExistException(); }));
            }
            mocks.ReplayAll();

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers
                                         .AddIfMatch(entityTag)
                                         .AddContentType(MediaTypes.AtomEntry.WithTypeAndSubtypeOnly())),
                          new NameValueCollection {{"id", fulfillment.Id.ToString("N")}}));

            Assert.AreEqual(404, responseContext.StatusCode);
            Assert.AreEqual("Not Found", responseContext.StatusDescription);

            mocks.VerifyAll();
        }

        [Test]
        public void WhenAttemptingToUpdateAFulfillmentInstanceThatIsNoLongerInDraftShouuldReturn409Conflict()
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            EntityTag entityTag = EntityTag.CreateWithRandomValue();
            Fulfillment fulfillment = FulfillmentBuilder.BuildNonDraftFulfillment(DateTimes.TwoPM);
            Member member = (Member) fulfillment.GetAtomFormatter().Item;
            
            UpdateFulfillment command = new UpdateFulfillment(repository, FakeDateTimeProvider.ForNow());

            using (mocks.Record())
            {
                Expect
                    .Call(repository.Get(fulfillment.Id))
                    .Return(new ETaggedEntity(entityTag, fulfillment));
            }
            mocks.ReplayAll();

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(
                          Uri,
                          new Atom10ItemFormatter<Member>(member),
                          headers => headers
                                         .AddIfMatch(entityTag)
                                         .AddContentType(MediaTypes.AtomEntry.WithTypeAndSubtypeOnly())),
                          new NameValueCollection { { "id", fulfillment.Id.ToString("N") } }));

            Assert.AreEqual(409, responseContext.StatusCode);
            Assert.AreEqual("Conflict", responseContext.StatusDescription);

            mocks.VerifyAll();
        }
    }
}