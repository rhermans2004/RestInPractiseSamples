using System;
using Restbucks.WcfRestToolkit.Http.Headers;

namespace Restbucks.WcfRestToolkit.Http.Testing
{
    public class FakeRequest : IRequest
    {
        public static IRequest For(string uri)
        {
            return new FakeRequest(new Uri(uri));
        }

        public static IRequest For(string uri, Action<FakeRequestHeadersBuilder> headerAction)
        {
            FakeRequestHeadersBuilder builder = new FakeRequestHeadersBuilder();
            headerAction(builder);
            return new FakeRequest(new Uri(uri), builder.Build());
        }

        public static IRequest<T> For<T>(string uri, T entityBody)
        {
            return new InnerFakeRequest<T>(new Uri(uri), entityBody);
        }

        public static IRequest<T> For<T>(string uri, T entityBody, Action<FakeRequestHeadersBuilder> headerAction)
        {
            FakeRequestHeadersBuilder builder = new FakeRequestHeadersBuilder();
            headerAction(builder);
            return new InnerFakeRequest<T>(new Uri(uri), builder.Build(), entityBody);
        }

        private readonly Uri uri;
        private readonly IRequestHeaders headers;

        private FakeRequest(Uri uri, IRequestHeaders headers)
        {
            this.uri = uri;
            this.headers = headers;
        }

        private FakeRequest(Uri uri) : this(uri, new FakeRequestHeadersBuilder().Build())
        {
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public IRequestHeaders Headers
        {
            get { return headers; }
        }

        public string CreateSummary()
        {
            return string.Empty;
        }

        private class InnerFakeRequest<T> : FakeRequest, IRequest<T>
        {
            private readonly T entityBody;

            public InnerFakeRequest(Uri uri, IRequestHeaders headers, T entityBody) : base(uri, headers)
            {
                this.entityBody = entityBody;
            }

            public InnerFakeRequest(Uri uri, T entityBody) : this(uri, new FakeRequestHeadersBuilder().Build(), entityBody)
            {
            }

            public T EntityBody
            {
                get { return entityBody; }
            }
        }
    }
}