using NUnit.Framework;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Tests.WcfRestToolkit.Http.HeaderValues
{
    [TestFixture]
    public class MediaTypeTests
    {
        [Test]
        public void ShouldReturnNullForNullValue()
        {
            Assert.IsNull(MediaType.Parse(null));
        }

        [Test]
        public void ShouldReturnNullForEmptyString()
        {
            Assert.IsNull(MediaType.Parse(string.Empty));
        }

        [Test]
        public void ShouldConvertToLowerCase()
        {
            Assert.AreEqual("application/atom+xml;type=feed", MediaType.Parse("APPLICATION/ATOM+XML;TYPE=FEED").TypeAndSubtypeAndParameters);
        }

        [Test]
        public void ShouldStripOutWhitespace()
        {
            Assert.AreEqual("application/atom+xml;type=feed", MediaType.Parse("application/atom+xml; type=feed").TypeAndSubtypeAndParameters);
        }

        [Test]
        public void ShouldIdentifyTypeAndSubtype()
        {
            Assert.AreEqual("application/atom+xml", MediaType.Parse("application/atom+xml; type=feed").TypeAndSubtype);
        }

        [Test]
        public void CanReturnMediaTypeContainingOnlyTypeAndSubtype()
        {
            Assert.AreEqual("application/atom+xml", MediaType.Parse("application/atom+xml;type=feed").WithTypeAndSubtypeOnly().TypeAndSubtype);
            Assert.AreEqual("application/atom+xml", MediaType.Parse("application/atom+xml;type=feed").WithTypeAndSubtypeOnly().TypeAndSubtypeAndParameters);
        }

        [Test]
        public void ShouldMatchOnTypeAndSubtype()
        {
            MediaType mediaType1 = MediaType.Parse("application/atom+xml");
            MediaType mediaType2 = MediaType.Parse("application/atom+xml; type=feed");
            MediaType mediaType3 = MediaType.Parse("application/xml");

            Assert.IsTrue(mediaType1.IsTypeAndSubtypeMatch(mediaType2));
            Assert.IsTrue(mediaType2.IsTypeAndSubtypeMatch(mediaType1));
            Assert.IsFalse(mediaType1.IsTypeAndSubtypeMatch(mediaType3));
            Assert.IsFalse(mediaType1.IsTypeAndSubtypeMatch(null));
        }

        [Test]
        public void ShouldMatchOnTypeAndSubtypeAndParameters()
        {
            MediaType mediaType1 = MediaType.Parse("application/atom+xml; charset=utf8; type=feed");
            MediaType mediaType2 = MediaType.Parse("application/atom+xml; type=feed; charset=utf8");
            MediaType mediaType3 = MediaType.Parse("application/atom+xml; type=entry; charset=utf8");

            Assert.IsTrue(mediaType1.IsTypeSubtypeAndParameterMatch(mediaType2));
            Assert.IsTrue(mediaType2.IsTypeSubtypeAndParameterMatch(mediaType1));
            Assert.IsFalse(mediaType1.IsTypeSubtypeAndParameterMatch(mediaType3));
            Assert.IsFalse(mediaType1.IsTypeSubtypeAndParameterMatch(null));
        }
    }
}