using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Serialization;

namespace Restbucks.WcfRestToolkit.Syndication.AtomPub
{
    public class Collection : SyndicationFeed
    {
        private const string CollectionElementName = "collection";
        private static readonly XmlSerializer CollectionSerializer = new XmlSerializer(typeof (CollectionExtension));

        private CollectionExtension collectionExtension = new CollectionExtension();

        public Collection()
        {
            collectionExtension = new CollectionExtension();
        }

        public CollectionExtension CollectionExtension
        {
            get { return collectionExtension; }
            set { collectionExtension = value; }
        }

        protected override SyndicationItem CreateItem()
        {
            return new Member();
        }

        protected override bool TryParseElement(XmlReader reader, string version)
        {
            if (reader.LocalName.Equals(CollectionElementName) && reader.NamespaceURI.Equals(Namespaces.AtomPub))
            {
                collectionExtension = (CollectionExtension) CollectionSerializer.Deserialize(reader);
                return true;
            }

            return base.TryParseElement(reader, version);
        }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            if (collectionExtension != null)
            {
                CollectionSerializer.Serialize(writer, collectionExtension);
            }

            base.WriteElementExtensions(writer, version);
        }
    }
}