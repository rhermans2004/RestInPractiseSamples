using System;

namespace Restbucks.WcfRestToolkit.Http.StatusCodes
{
    public class Status : IStatus
    {
        public static IStatus OK = new Status(200, "OK");
        public static IStatus NotModified = new Status(304, "Not Modified");
        public static IStatus BadRequest = new Status(400, "Bad Request");
        public static IStatus Unauthorized = new Status(401, "Unauthorized");
        public static IStatus NotFound = new Status(404, "Not Found");
        public static IStatus Conflict = new Status(409, "Conflict");
        public static IStatus Gone = new Status(410, "Gone");
        public static IStatus PreconditionFailed = new Status(412, "Precondition Failed");
        public static IStatus UnsupportedMediaType = new Status(415, "Unsupported Media Type");
        public static IStatus ServerError = new Status(500, "Server Error");

        public static IStatus Created(Uri location)
        {
            return new CreatedImpl(location);
        }

        private readonly int statusCode;
        private readonly string statusDescription;

        private Status(int statusCode, string statusDescription)
        {
            this.statusCode = statusCode;
            this.statusDescription = statusDescription;
        }

        public void ApplyTo(IResponseContext context)
        {
            context.SetStatusCode(statusCode);
            context.SetStatusDescription(statusDescription);
        }

        private class CreatedImpl : IStatus
        {
            private readonly Uri location;

            public CreatedImpl(Uri location)
            {
                this.location = location;
            }

            public void ApplyTo(IResponseContext context)
            {
                context.SetStatusCode(201);
                context.SetStatusDescription("Created");
                context.SetLocation(location.AbsoluteUri);
            }
        }
    }
}