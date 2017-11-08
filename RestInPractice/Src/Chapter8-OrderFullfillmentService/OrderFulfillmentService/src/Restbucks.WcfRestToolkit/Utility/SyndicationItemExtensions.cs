using System.Linq;
using System.ServiceModel.Syndication;

namespace Restbucks.WcfRestToolkit.Utility
{
    public static class SyndicationItemExtensions
    {
        public static SyndicationLink GetSelfLink(this SyndicationItem item)
        {
            return item.Links.FirstOrDefault(l => l.RelationshipType.Equals("self"));
        }

        public static SyndicationLink GetEditLink(this SyndicationItem item)
        {
            return item.Links.FirstOrDefault(l => l.RelationshipType.Equals("edit"));
        }
    }
}