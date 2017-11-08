using System.ServiceModel.Channels;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.WcfRestToolkit.Syndication
{
    public class XmlSubTypeExtensionContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            MediaType mediaType = MediaType.Parse(contentType);
            if (mediaType.TypeAndSubtype.EndsWith("xml"))
            {
                return WebContentFormat.Xml;
            }
            return WebContentFormat.Default;
        }
    }
}