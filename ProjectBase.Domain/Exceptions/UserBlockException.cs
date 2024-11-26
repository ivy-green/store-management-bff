using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class UserBlockException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "USER_BLOCK";
        public static new readonly string Message = "User has been blocked";
        public UserBlockException() : base(Status, Code, Message)
        {
        }
    }
}
