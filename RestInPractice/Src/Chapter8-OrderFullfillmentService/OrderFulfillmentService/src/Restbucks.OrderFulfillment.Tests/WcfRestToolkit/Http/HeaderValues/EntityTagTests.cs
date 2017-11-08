using NUnit.Framework;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Tests.WcfRestToolkit.Http.HeaderValues
{
    [TestFixture]
    public class EntityTagTests
    {
        [Test]
        public void ShouldAddDoubleQuotesToValueWithoutQuotes()
        {
            EntityTag entityTag = EntityTag.Parse("123");
            Assert.AreEqual(@"""123""", entityTag.Value);
        }

        [Test]
        public void ShouldPreserveExistingDoubleQuotes()
        {
            EntityTag entityTag = EntityTag.Parse(@"""123""");
            Assert.AreEqual(@"""123""", entityTag.Value);
        }

        [Test]
        public void ShouldReturnNullForNullValue()
        {
            Assert.IsNull(EntityTag.Parse(null));
        }

        [Test]
        public void ShouldReturnNullForEmptyString()
        {
            Assert.IsNull(EntityTag.Parse(string.Empty));
        }

        [Test]
        public void ShouldReturnNullForDoubleQuotesOnly()
        {
            Assert.IsNull(EntityTag.Parse(@""""""));
        }

        [Test]
        public void ShouldStripOutAnyEmbeddedDoubleQuotes()
        {
            EntityTag entityTag = EntityTag.Parse(@"12""3");
            Assert.AreEqual(@"""123""", entityTag.Value);
        }

        [Test]
        public void ShouldExhibitValueEquality()
        {
            EntityTag entityTagValue1 = EntityTag.Parse("123");
            EntityTag entityTagValue2 = EntityTag.Parse("123");
            EntityTag entityTagValue3 = EntityTag.Parse("ABC");

            Assert.AreEqual(entityTagValue1, entityTagValue2);
            Assert.AreEqual(entityTagValue1.GetHashCode(), entityTagValue2.GetHashCode());

            Assert.AreNotEqual(entityTagValue1, entityTagValue3);
            Assert.AreNotEqual(entityTagValue1.GetHashCode(), entityTagValue3.GetHashCode());

            Assert.AreNotEqual(null, entityTagValue1);
        }

        [Test]
        public void ShouldIgnoreDoubleQuotesAtBeginningAndEndOfValues()
        {
            EntityTag entityTagValue1 = EntityTag.Parse(@"""123""");
            EntityTag entityTagValue2 = EntityTag.Parse("123");

            Assert.IsTrue(entityTagValue1.Equals(entityTagValue2));
            Assert.IsTrue(entityTagValue2.Equals(entityTagValue1));
        }
    }
}