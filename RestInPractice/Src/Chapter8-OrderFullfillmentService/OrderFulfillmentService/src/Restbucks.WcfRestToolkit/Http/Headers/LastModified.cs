using System;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class LastModified : IResponseHeader
    {
        private readonly DateTimeOffset lastModified;

        public LastModified(DateTimeOffset lastModified)
        {
            this.lastModified = lastModified;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetLastModified(lastModified.DateTime);
        }
    }
}