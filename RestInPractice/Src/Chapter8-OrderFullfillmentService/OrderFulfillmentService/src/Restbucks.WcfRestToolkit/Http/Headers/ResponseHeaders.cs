using System.Collections.Generic;

namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public class ResponseHeaders
    {
        private readonly Queue<IResponseHeader> headers;

        public ResponseHeaders()
        {
            headers = new Queue<IResponseHeader>();
        }

        public ResponseHeaders Add(IResponseHeader responseHeader)
        {
            headers.Enqueue(responseHeader);
            return this;
        }

        public void ApplyTo(IResponseContext context)
        {
            foreach (IResponseHeader header in headers)
            {
                header.ApplyTo(context);
            }
        }

    }
}