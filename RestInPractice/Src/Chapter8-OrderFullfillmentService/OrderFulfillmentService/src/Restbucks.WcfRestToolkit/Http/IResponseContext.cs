using System;

namespace Restbucks.WcfRestToolkit.Http
{
    public interface IResponseContext
    {
        void SetStatusDescription(string value);
        void SetStatusCode(int value);
        void SetCacheControl(string value);
        void SetContentType(string value);
        void SetLocation(string value);
        void SetLastModified(DateTimeOffset value);
        void SetETag(string value);
        void SetContentLocation(string value);
    }
}