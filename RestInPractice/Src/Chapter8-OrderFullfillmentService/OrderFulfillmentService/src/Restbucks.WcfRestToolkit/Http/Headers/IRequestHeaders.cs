
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public interface IRequestHeaders
    {
        EntityTag IfNoneMatch { get; }
        EntityTag IfMatch { get; }
        MediaType ContentType { get; }
        Authorization Authorization { get; }
        string CreateSummary();
    }
}