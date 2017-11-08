using System;
using NUnit.Framework;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Tests.WcfRestToolkit.Syndication.AtomPub
{
    [TestFixture]
    public class DraftStatusTests
    {
        [Test]
        public void WhenSuppliedValueIsValidShouldReturnDraftStatus()
        {
            Assert.AreEqual(DraftStatus.Yes, DraftStatus.Parse("yes"));
            Assert.AreEqual(DraftStatus.No, DraftStatus.Parse("no"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Invalid argument. Valid values are 'yes' and 'no'.")]
        public void WhenSuppliedValueIsInvalidShouldThrowException()
        {
            DraftStatus.Parse("YES");
        }
    }
}