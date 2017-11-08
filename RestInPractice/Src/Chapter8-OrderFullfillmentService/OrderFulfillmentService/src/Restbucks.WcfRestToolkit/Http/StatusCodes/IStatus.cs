namespace Restbucks.WcfRestToolkit.Http.StatusCodes
{
    public interface IStatus
    {
        void ApplyTo(IResponseContext context);
    }
}