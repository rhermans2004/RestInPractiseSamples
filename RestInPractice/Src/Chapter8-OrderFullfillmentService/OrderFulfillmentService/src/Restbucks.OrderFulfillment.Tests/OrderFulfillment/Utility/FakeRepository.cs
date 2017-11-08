using System;
using System.Collections.Generic;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Rhino.Mocks;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public static class FakeRepository
    {
        public static IRepository WithNoBehaviour()
        {
            return (IRepository) new MockRepository().Stub(typeof (IRepository));
        }

        public static IRepository ForGetAll(IEnumerable<Fulfillment> returnMembers)
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = (IRepository) mocks.Stub(typeof (IRepository));

            using (mocks.Record())
            {
                SetupResult.For(repository.GetAll()).Return(returnMembers);
            }
            return repository;
        }

        public static IRepository ForGet(Guid id, ETaggedEntity returnETaggedEntity)
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = (IRepository) mocks.Stub(typeof (IRepository));

            using (mocks.Record())
            {
                SetupResult.For(repository.Get(id)).Return(returnETaggedEntity);
            }

            return repository;
        }

        public static IRepository ForGetWhenMemberMissing(Guid id)
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = (IRepository) mocks.Stub(typeof (IRepository));

            using (mocks.Record())
            {
                SetupResult.For(repository.Get(id)).Throw(new MemberDoesNotExistException());
            }

            return repository;
        }

        public static IRepository ForAdd()
        {
            MockRepository mocks = new MockRepository();
            IRepository repository = (IRepository)mocks.Stub(typeof(IRepository));

            using (mocks.Record())
            {
                SetupResult.For(repository.Add(null)).IgnoreArguments().Do((Func<Fulfillment, ETaggedEntity>)
                        (f => new ETaggedEntityBuilder()
                                  .WithFulfillment(f)
                                  .WithEntityTag(EntityTag.CreateWithRandomValue())
                                  .Build()));
            }

            return repository;
        }
    }
}