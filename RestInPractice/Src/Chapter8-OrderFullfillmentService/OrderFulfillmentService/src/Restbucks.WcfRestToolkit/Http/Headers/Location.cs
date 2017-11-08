using System;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class Location : IResponseHeader
    {
        private readonly Uri value;

        public Location(Uri value)
        {
            this.value = value;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetLocation(value.AbsoluteUri);
        }
    }
}