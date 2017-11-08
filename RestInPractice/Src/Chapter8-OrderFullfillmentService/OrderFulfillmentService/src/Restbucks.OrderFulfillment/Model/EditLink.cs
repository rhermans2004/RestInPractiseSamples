using System;
using System.ServiceModel.Syndication;

namespace Restbucks.OrderFulfillment.Model
{
    public class EditLink
    {
        private static readonly UriTemplate UriTemplate = new UriTemplate("/{id}");

        private readonly Uri baseUri;
        private readonly Guid id;

        public EditLink(Uri baseUri, Guid id)
        {
            this.baseUri = baseUri;
            this.id = id;
        }

        public SyndicationLink ToSyndicationLink()
        {
            Uri uri = UriTemplate.BindByPosition(baseUri, id.ToString("N"));
            return new SyndicationLink {Uri = uri, RelationshipType = "edit"};
        }
    }
}