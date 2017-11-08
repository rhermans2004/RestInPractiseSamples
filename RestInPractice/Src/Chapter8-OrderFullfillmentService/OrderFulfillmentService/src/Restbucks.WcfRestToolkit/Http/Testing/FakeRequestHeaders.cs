using Restbucks.WcfRestToolkit.Http.Headers;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.WcfRestToolkit.Http.Testing
{
    public class FakeRequestHeaders : IRequestHeaders
    {
        public EntityTag IfNoneMatch { get; set; }
        public EntityTag IfMatch { get; set; }
        public MediaType ContentType { get; set; }
        public Authorization Authorization { get; set; }

        public string CreateSummary()
        {
            return string.Empty;
        }
    }
}