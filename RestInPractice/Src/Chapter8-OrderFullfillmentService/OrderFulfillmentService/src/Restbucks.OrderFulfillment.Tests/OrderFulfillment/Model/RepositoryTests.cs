using System;
using System.Linq;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Model;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Model
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void ShouldBeAbleToAddAndGetMember()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();

            Assert.AreEqual(0, repository.GetAll().Count());

            repository.Add(fulfillment);
            
            Assert.AreEqual(1, repository.GetAll().Count());
            Assert.AreEqual(fulfillment, repository.Get(fulfillment.Id).Fulfillment);
        }

        [Test]
        public void WhenAddingMemberShouldReturnEnvelopeContainingMemberAndEntityTag()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();

            ETaggedEntity eTaggedEntity = repository.Add(fulfillment);

            Assert.AreEqual(fulfillment, eTaggedEntity.Fulfillment);    
            Assert.IsFalse(String.IsNullOrEmpty(eTaggedEntity.EntityTag.Value));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(MemberAlreadyExistsException))]
        public void ShouldThrowExceptionWhenAddingDuplicateFulfillmentMember()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();

            repository.Add(fulfillment);
            repository.Add(fulfillment);
        }

        [Test]
        public void ShouldBeAbleToAddAndGetMultipleFulfillmentMembers()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment1 = FulfillmentBuilder.NewFulfillment();
            Fulfillment fulfillment2 = FulfillmentBuilder.NewFulfillment();
            Fulfillment fulfillment3 = FulfillmentBuilder.NewFulfillment();

            Assert.AreEqual(0, repository.GetAll().Count());

            repository.Add(fulfillment1);
            repository.Add(fulfillment2);
            repository.Add(fulfillment3);

            Assert.AreEqual(3, repository.GetAll().Count());

            Assert.AreEqual(fulfillment1, repository.GetAll().ElementAt(0));
            Assert.AreEqual(fulfillment2, repository.GetAll().ElementAt(1));
            Assert.AreEqual(fulfillment3, repository.GetAll().ElementAt(2));
        }

        [Test]
        public void MultipleGetsOfTheSameFulfillmentMemberReturnTheSameEntityTagValue()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();

            repository.Add(fulfillment);

            Assert.AreEqual(repository.Get(fulfillment.Id).EntityTag, repository.Get(fulfillment.Id).EntityTag);
        }

        [Test]
        public void ShouldBeAbleToRemoveAFulfillmentMember()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();

            Assert.AreEqual(0, repository.GetAll().Count());
            repository.Add(fulfillment);
            Assert.AreEqual(1, repository.GetAll().Count());
            repository.Remove(fulfillment.Id);
            Assert.AreEqual(0, repository.GetAll().Count());
        }

        [Test]
        public void ShouldDoNothingIfAttemptingToRemoveMemberThatDoesNotExist()
        {
            Repository repository = new Repository();
            repository.Add(FulfillmentBuilder.NewFulfillment());

            Assert.AreEqual(1, repository.GetAll().Count());
            repository.Remove(Guid.NewGuid());
            Assert.AreEqual(1, repository.GetAll().Count());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(MemberNoLongerExistsException))]
        public void ShouldThrowExceptionIfAttemptingToRemoveMemberThathasAlreadyBeenRemoved()
        {
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();

            Repository repository = new Repository();
            repository.Add(fulfillment);

            repository.Remove(fulfillment.Id);
            repository.Remove(fulfillment.Id);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(MemberDoesNotExistException))]
        public void ShouldThrowExceptionIfGettingAMemberThatDoesNotExist()
        {
            Repository repository = new Repository();
            repository.Add(FulfillmentBuilder.NewFulfillment());

            repository.Get(Guid.NewGuid());
        }

        [Test]
        public void ShouldUpdateEntityTagWhenUpdatingFulfillmentMember()
        {
            Repository repository = new Repository();
            Guid id = Guid.NewGuid();
            Fulfillment fulfillment = new FulfillmentBuilder().WithId(id).Build();

            repository.Add(fulfillment);

            ETaggedEntity eTaggedEntity = repository.Get(id);
            EntityTag originalEntityTag = eTaggedEntity.EntityTag;

            repository.Update(eTaggedEntity.Fulfillment, originalEntityTag);

            ETaggedEntity secondETaggedEntity = repository.Get(id);

            Assert.AreNotEqual(originalEntityTag, secondETaggedEntity.EntityTag);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(OptimisticUpdateFailedException))]
        public void ShouldThrowExceptionIfUpdatingFulfillmentMemberWithOutOfDateEntityTag()
        {
            Repository repository = new Repository();
            Fulfillment fulfillment = FulfillmentBuilder.NewFulfillment();
            repository.Add(fulfillment);

            repository.Update(fulfillment, EntityTag.CreateWithRandomValue());
        }
        
    }
}