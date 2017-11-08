using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility;
using Restbucks.WcfRestToolkit.Syndication;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Tests.WcfRestToolkit.Syndication.AtomPub
{
    [TestFixture]
    public class MemberTests
    {
        [Test]
        public void CanSetAndGetEditedDateTime()
        {
            Member member = new Member {EditedDateTime = DateTimes.OnePM};

            Assert.AreEqual(DateTimes.OnePM, member.EditedDateTime);
        }

        [Test]
        public void EditedDateTimeDefaultsToLastUpdatedDateTime()
        {
            Member member = new Member {LastUpdatedTime = DateTimes.OnePM};

            Assert.AreEqual(DateTimes.OnePM, member.EditedDateTime);
        }

        [Test]
        public void EditedDateTimeIsSerializedCorrectly()
        {
            Member member = new Member {EditedDateTime = DateTimes.OnePM};

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                member.GetAtom10Formatter().WriteTo(writer);
            }
            XElement atomDoc = XElement.Parse(builder.ToString());

            Assert.AreEqual(DateTimes.OnePM.ToString("yyyy-MM-ddTHH:mm:ssZ"), (from XElement x in atomDoc.Elements()
                                                                               where x.Name.NamespaceName.Equals(Namespaces.AtomPub) && x.Name.LocalName.Equals("edited")
                                                                               select x).FirstOrDefault().Value);
        }

        [Test]
        public void CanSetAndGetDraftStatus()
        {
            Member member = new Member {Draft = DraftStatus.Yes};
            Assert.AreEqual(DraftStatus.Yes, member.Draft);

            member.Draft = DraftStatus.No;
            Assert.AreEqual(DraftStatus.No, member.Draft);
        }

        [Test]
        public void DraftStatusDefaultsToNo()
        {
            Assert.AreEqual(DraftStatus.No, new Member().Draft);
        }

        [Test]
        public void ControlsExtensionIsSerializedCorrectly()
        {
            Member member = new Member();

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                member.GetAtom10Formatter().WriteTo(writer);
            }
            XElement atomDoc = XElement.Parse(builder.ToString());

            Assert.IsNotNull((from XElement x in atomDoc.Elements()
                              where x.Name.NamespaceName.Equals(Namespaces.AtomPub) && x.Name.LocalName.Equals("control")
                              select x).FirstOrDefault());
            Assert.IsNotNull((from XElement x in atomDoc.Elements()
                              where x.Name.NamespaceName.Equals(Namespaces.AtomPub) && x.Name.LocalName.Equals("control")
                              from c in x.Elements()
                              where c.Name.NamespaceName.Equals(Namespaces.AtomPub) && c.Name.LocalName.Equals("draft") && c.Value.Equals("no")
                              select c).FirstOrDefault());
        }

        [Test]
        public void WhenIncomingXmlDoesNotContainControlsAndDraftExtensionDraftStatusIsSetToNo()
        {
            StringBuilder builder = new StringBuilder();

            SyndicationItem item = new SyndicationItem();

            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                item.GetAtom10Formatter().WriteTo(writer);
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(builder.ToString())))
            {
                Atom10ItemFormatter<Member> formatter = new Atom10ItemFormatter<Member>();
                formatter.ReadFrom(reader);
                Member deserializedItem = (Member) formatter.Item;

                Assert.AreEqual(DraftStatus.No, deserializedItem.Draft);
            }
        }

        [Test]
        public void ShouldPreserveDraftStatusAndEditedDateTimeWhenRoundrippingSerialization()
        {
            Member originalMember = new Member {EditedDateTime = DateTimes.OnePM};
            originalMember.Draft = DraftStatus.Yes;

            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                originalMember.GetAtom10Formatter().WriteTo(writer);
            }

            Atom10ItemFormatter<Member> formatter = new Atom10ItemFormatter<Member>();
            formatter.ReadFrom(XmlReader.Create(new StringReader(sb.ToString())));

            Member copy = (Member) formatter.Item;

            Assert.AreEqual(DraftStatus.Yes, copy.Draft);
            Assert.AreEqual(DateTimes.OnePM, copy.EditedDateTime);
        }
    }
}