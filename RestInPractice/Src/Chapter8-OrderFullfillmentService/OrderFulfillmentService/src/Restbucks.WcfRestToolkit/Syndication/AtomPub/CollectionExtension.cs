using System.Xml.Serialization;

namespace Restbucks.WcfRestToolkit.Syndication.AtomPub
{
    [XmlRoot(ElementName = "collection", Namespace = Namespaces.AtomPub)]
    public class CollectionExtension
    {
        [XmlAttribute(AttributeName = "href", Namespace = Namespaces.AtomPub)]
        public string Href { get; set; }

        [XmlElement(ElementName = "title", Namespace = Namespaces.Atom)]
        public string Title { get; set; }

        [XmlElement(ElementName = "accept", Namespace = Namespaces.AtomPub)]
        public string[] Accept { get; set; }
    }
}