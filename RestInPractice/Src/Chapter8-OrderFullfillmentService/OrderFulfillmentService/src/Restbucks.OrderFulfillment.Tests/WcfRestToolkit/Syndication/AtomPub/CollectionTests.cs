using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Restbucks.WcfRestToolkit.Syndication;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Tests.WcfRestToolkit.Syndication.AtomPub
{
    [TestFixture]
    public class CollectionTests
    {
        [Test]
        public void CanSetAndGetCollectionExtension()
        {
            string href = "http://localhost/fulfillment";
            string title = "Fulfillment Service";

            Collection feed = new Collection
                              {
                                  CollectionExtension = new CollectionExtension
                                                        {
                                                            Href = href,
                                                            Title = title,
                                                            Accept = new[] {MediaTypes.Restbucks.TypeAndSubtype, MediaTypes.AtomEntry.TypeAndSubtypeAndParameters}
                                                        }
                              };

            CollectionExtension extension = feed.CollectionExtension;

            Assert.AreEqual(href, extension.Href);
            Assert.AreEqual(title, extension.Title);
            Assert.AreEqual(MediaTypes.Restbucks.TypeAndSubtype, extension.Accept[0]);
            Assert.AreEqual(MediaTypes.AtomEntry.TypeAndSubtypeAndParameters, extension.Accept[1]);
        }

        [Test]
        public void CanReplaceCollectionExtension()
        {
            Collection feed = new Collection
                              {
                                  CollectionExtension = new CollectionExtension
                                                        {
                                                            Href = "http://localhost/fulfillment",
                                                            Title = "Fulfillment Service",
                                                            Accept = new[] {MediaTypes.Restbucks.TypeAndSubtype, MediaTypes.AtomEntry.TypeAndSubtypeAndParameters}
                                                        }
                              };

            feed.CollectionExtension = new CollectionExtension
                                       {
                                           Href = "http://localhost:8080/fulfillment",
                                           Title = "Order Fulfillment Service",
                                           Accept = new[] {MediaTypes.Restbucks.TypeAndSubtype}
                                       };

            Assert.AreEqual(1, feed.CollectionExtension.Accept.Length);
        }

        [Test]
        public void CollectionExtensionIsSerializedCorrectly()
        {
            string href = "http://localhost/fulfillment";
            string title = "Fulfillment Service";

            Collection feed = new Collection
                              {
                                  CollectionExtension = new CollectionExtension
                                                        {
                                                            Href = href,
                                                            Title = title,
                                                            Accept = new[] {MediaTypes.Restbucks.TypeAndSubtype}
                                                        }
                              };

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                feed.GetAtom10Formatter().WriteTo(writer);
            }
            XElement atomDoc = XElement.Parse(builder.ToString());

            XElement collectionElement = (from XElement x in atomDoc.Elements()
                                          where x.Name.NamespaceName.Equals(Namespaces.AtomPub) && x.Name.LocalName.Equals("collection")
                                                && (from a in x.Attributes() where a.Name.LocalName.Equals("href") && a.Value.Equals(href) select a).Any()
                                          select x).FirstOrDefault();
            Assert.IsNotNull(collectionElement);

            Assert.IsNotNull((from x in collectionElement.Elements()
                              where x.Name.NamespaceName.Equals(Namespaces.Atom) && x.Name.LocalName.Equals("title") && x.Value.Equals(title)
                              select x).FirstOrDefault());
            Assert.IsNotNull((from x in collectionElement.Elements()
                              where x.Name.NamespaceName.Equals(Namespaces.AtomPub) && x.Name.LocalName.Equals("accept") && x.Value.Equals(MediaTypes.Restbucks.TypeAndSubtype)
                              select x).FirstOrDefault());
        }

        [Test]
        public void CanSerializeAndDeserializeCollection()
        {
            string id = new UniqueId(Guid.NewGuid()).ToString();
            Collection collection = new Collection {Id = id};
            collection.CollectionExtension = new CollectionExtension {Accept = new[] {"application/xml", "application/json"}};
            collection.Items = new List<SyndicationItem> {new Member()};
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings {Encoding = Encoding.UTF8, CloseOutput = true, OmitXmlDeclaration = true};
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                collection.GetAtom10Formatter().WriteTo(writer);
            }
            XDocument doc = XDocument.Parse(sb.ToString());
            Atom10FeedFormatter<Collection> formatter = new Atom10FeedFormatter<Collection>();
            formatter.ReadFrom(doc.CreateReader());
            Collection copy = (Collection) formatter.Feed;
            Assert.AreEqual(id, copy.Id);
            Assert.AreEqual(1, copy.Items.Count());
            Assert.AreEqual(2, copy.CollectionExtension.Accept.Length);
        }
    }
}