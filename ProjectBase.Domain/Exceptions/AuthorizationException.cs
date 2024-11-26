using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class AuthorizationException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.Unauthorized;
        public static new readonly string Code = "UNAUTHORIZATION";
        public static new readonly string Message = "Please login before perform this action";
        public AuthorizationException() : base(Status, Code, Message)
        {
        }
    }
}
