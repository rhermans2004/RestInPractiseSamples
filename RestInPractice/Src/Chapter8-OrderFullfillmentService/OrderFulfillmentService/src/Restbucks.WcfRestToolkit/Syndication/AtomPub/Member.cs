using System;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Restbucks.WcfRestToolkit.Syndication.AtomPub
{
    public class Member : SyndicationItem
    {
        private const string EditedElementName = "edited";
        private const string ControlElementName = "control";
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

        private static readonly DataContractSerializer ControlSerializer = new DataContractSerializer(typeof (ControlExtension));

        private ControlExtension control;
        private DateTimeOffset? editedDateTime;

        public Member()
        {
            control = new ControlExtension {Draft = DraftStatus.No};
        }

        public DraftStatus Draft
        {
            get { return control.Draft; }
            set { control.Draft = value; }
        }

        public DateTimeOffset EditedDateTime
        {
            get
            {
                if (editedDateTime == null)
                {
                    editedDateTime = LastUpdatedTime;
                }
                return editedDateTime.Value;
            }
            set { editedDateTime = value; }
        }

        public override SyndicationItem Clone()
        {
            return (Member) MemberwiseClone();
        }

        protected override bool TryParseElement(XmlReader reader, string version)
        {
            if (reader.LocalName.Equals(ControlElementName) && reader.NamespaceURI.Equals(Namespaces.AtomPub))
            {
                control = (ControlExtension) ControlSerializer.ReadObject(reader);
                return true;
            }
            if (reader.LocalName.Equals(EditedElementName) && reader.NamespaceURI.Equals(Namespaces.AtomPub))
            {
                editedDateTime = reader.ReadElementContentAsDateTime();
                return true;
            }
            return base.TryParseElement(reader, version);
        }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            writer.WriteStartElement(EditedElementName, Namespaces.AtomPub);
            writer.WriteValue(FormatDateTime(EditedDateTime));
            writer.WriteEndElement();

            if (control != null)
            {
                ControlSerializer.WriteObject(writer, control);
            }

            base.WriteElementExtensions(writer, version);
        }

        private static string FormatDateTime(DateTimeOffset dateTime)
        {
            return dateTime.ToUniversalTime().ToString(DateTimeFormat);
        }
    }
}