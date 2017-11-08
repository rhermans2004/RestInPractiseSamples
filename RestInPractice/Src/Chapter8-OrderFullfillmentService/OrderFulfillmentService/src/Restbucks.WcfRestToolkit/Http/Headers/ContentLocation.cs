using System;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class ContentLocation : IResponseHeader
    {
        private readonly Uri value;

        public ContentLocation(Uri value)
        {
            this.value = value;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetContentLocation(value.AbsoluteUri);
        }
    }
}