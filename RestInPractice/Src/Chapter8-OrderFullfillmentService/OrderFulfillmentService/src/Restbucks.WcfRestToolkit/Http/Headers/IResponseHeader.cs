namespace Restbucks.WcfRestToolkit.Http.Headers
{
    public interface IResponseHeader
    {
        void ApplyTo(IResponseContext context);
    }
}