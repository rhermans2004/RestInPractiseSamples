using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Model
{
    public class Repository : IRepository
    {
        private readonly ReaderWriterLockSlim readerWriterLock;
        private readonly IDictionary<Guid, ETaggedEntity> fulfillmentMembers;
        private readonly IList<Guid> removedIDs;

        public Repository()
        {
            readerWriterLock = new ReaderWriterLockSlim();
            fulfillmentMembers = new Dictionary<Guid, ETaggedEntity>();
            removedIDs = new List<Guid>();
        }

        public IEnumerable<Fulfillment> GetAll()
        {
            readerWriterLock.EnterReadLock();
            try
            {
                return (from e in fulfillmentMembers.Values select e.Fulfillment);
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        public ETaggedEntity Get(Guid id)
        {
            readerWriterLock.EnterReadLock();
            try
            {
                if (!fulfillmentMembers.ContainsKey(id))
                {
                    throw new MemberDoesNotExistException();
                }
                return fulfillmentMembers[id];
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        public ETaggedEntity Add(Fulfillment fulfillment)
        {
            readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (fulfillmentMembers.ContainsKey(fulfillment.Id))
                {
                    throw new MemberAlreadyExistsException();
                }

                readerWriterLock.EnterWriteLock();
                try
                {
                    ETaggedEntity eTaggedEntity = new ETaggedEntity(EntityTag.CreateWithRandomValue(), fulfillment);
                    fulfillmentMembers.Add(fulfillment.Id, eTaggedEntity);
                    return eTaggedEntity;
                }
                finally
                {
                    readerWriterLock.ExitWriteLock();
                }
            }
            finally
            {
                readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        public void Update(Fulfillment fulfillment, EntityTag entityTag)
        {
            readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (!fulfillmentMembers.ContainsKey(fulfillment.Id))
                {
                    throw new MemberDoesNotExistException();
                }

                ETaggedEntity eTaggedEntity = fulfillmentMembers[fulfillment.Id];

                if (!eTaggedEntity.EntityTag.Equals(entityTag))
                {
                    throw new OptimisticUpdateFailedException();
                }

                readerWriterLock.EnterWriteLock();
                try
                {
                    fulfillmentMembers[fulfillment.Id] = new ETaggedEntity(EntityTag.CreateWithRandomValue(), fulfillment);
                }
                finally
                {
                    readerWriterLock.ExitWriteLock();
                }
            }
            finally
            {
                readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        public void Remove(Guid id)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                if (!fulfillmentMembers.ContainsKey(id))
                {
                    if (removedIDs.Contains(id))
                    {
                        throw new MemberNoLongerExistsException();
                    }
                }
                
                fulfillmentMembers.Remove(id);
                removedIDs.Add(id);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}