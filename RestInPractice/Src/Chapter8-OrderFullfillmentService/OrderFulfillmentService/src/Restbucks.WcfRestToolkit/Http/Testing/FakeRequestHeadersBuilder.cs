using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.WcfRestToolkit.Http.Testing
{
    public class FakeRequestHeadersBuilder
    {
        private EntityTag ifNoneMatch;
        private EntityTag ifMatch;
        private MediaType mediaType;

        public FakeRequestHeadersBuilder()
        {
            ifNoneMatch = null;
            ifMatch = null;
            mediaType = null;
        }

        public FakeRequestHeadersBuilder AddIfNoneMatch(EntityTag value)
        {
            ifNoneMatch = value;
            return this;
        }

        public FakeRequestHeadersBuilder AddIfMatch(EntityTag value)
        {
            ifMatch = value;
            return this;
        }

        public FakeRequestHeadersBuilder AddContentType(MediaType value)
        {
            mediaType = value;
            return this;
        }

        public FakeRequestHeaders Build()
        {
            return new FakeRequestHeaders
                   {
                       ContentType = mediaType,
                       IfNoneMatch = ifNoneMatch,
                       IfMatch = ifMatch
                   };
        }
    }
}