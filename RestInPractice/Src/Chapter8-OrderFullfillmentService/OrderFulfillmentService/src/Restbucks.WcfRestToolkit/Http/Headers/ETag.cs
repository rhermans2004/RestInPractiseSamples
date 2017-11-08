using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class ETag : IResponseHeader
    {
        private readonly EntityTag value;

        public ETag(EntityTag value)
        {
            this.value = value;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetETag(value.Value);
        }
    }
}