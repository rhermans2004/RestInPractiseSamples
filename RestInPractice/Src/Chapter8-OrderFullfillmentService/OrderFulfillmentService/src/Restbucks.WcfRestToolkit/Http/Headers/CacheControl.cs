namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class CacheControl : IResponseHeader
    {
        private readonly string value;

        public CacheControl(string value)
        {
            this.value = value;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetCacheControl(value);
        }
    }
}