using System;
using Restbucks.WcfRestToolkit.Http.Headers;

namespace Restbucks.WcfRestToolkit.Http
{
    public interface IRequest<T> : IRequest
    {  
        T EntityBody { get; }
    }

    public interface IRequest
    {
        Uri Uri { get; }
        IRequestHeaders Headers { get; }
        string CreateSummary();
    }
}