using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit.Http.Testing;
using Rhino.Mocks;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Commands
{
    [TestFixture]
    public class DeleteFulfillmentTests
    {
        [Test]
        public void ShouldDeleteFulfillmentMemberFromRepository()
        {
            Guid id = Guid.NewGuid();
            string uri = "http://localhost/fulfillment/" + id.ToString("N");
            NameValueCollection parameters = new NameValueCollection {{"id", id.ToString("N")}};

            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            using (mocks.Record())
            {
                Expect.Call(() => repository.Remove(id));
            }
            mocks.ReplayAll();

            DeleteFulfillment command = new DeleteFulfillment(repository);

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(uri),
                          parameters));

            Assert.AreEqual(200, responseContext.StatusCode);
            Assert.AreEqual("OK", responseContext.StatusDescription);

            mocks.VerifyAll();
        }

        [Test]
        public void IfMemberToBeDeletedNoLongerExistsShouldReturn410Gone()
        {
            Guid id = Guid.NewGuid();
            string uri = "http://localhost/fulfillment/" + id.ToString("N");
            NameValueCollection parameters = new NameValueCollection {{"id", id.ToString("N")}};

            MockRepository mocks = new MockRepository();
            IRepository repository = mocks.CreateMock<IRepository>();

            using (mocks.Record())
            {
                Expect.Call(() => repository.Remove(id))
                    .IgnoreArguments()
                    .Do((Action<Guid>)
                        (g =>
                         {
                             Assert.AreEqual(g, id);
                             throw new MemberNoLongerExistsException();
                         }));
            }
            mocks.ReplayAll();

            DeleteFulfillment command = new DeleteFulfillment(repository);

            FakeResponseContext responseContext = FakeResponseContext.Generate(
                () => command.Execute(
                          FakeRequest.For(uri),
                          parameters));

            Assert.AreEqual(410, responseContext.StatusCode);
            Assert.AreEqual("Gone", responseContext.StatusDescription);

            mocks.VerifyAll();
        }
    }
}