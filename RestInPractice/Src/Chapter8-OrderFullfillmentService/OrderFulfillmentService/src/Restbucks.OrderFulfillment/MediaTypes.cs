using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment
{
    public static class MediaTypes
    {
        public static readonly MediaType Restbucks = MediaType.Parse("application/vnd.restbucks+xml");
        public static readonly MediaType AtomFeed = MediaType.Parse("application/atom+xml; type=feed");
        public static readonly MediaType AtomEntry = MediaType.Parse("application/atom+xml; type=entry");
    }
}