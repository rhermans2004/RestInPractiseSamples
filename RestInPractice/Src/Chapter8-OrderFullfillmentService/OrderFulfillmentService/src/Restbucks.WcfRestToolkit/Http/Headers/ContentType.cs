using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class ContentType : IResponseHeader
    {
        private readonly MediaType mediaType;

        public ContentType(MediaType mediaType)
        {
            this.mediaType = mediaType;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetContentType(mediaType.TypeAndSubtypeAndParameters);
        }
    }
}