using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class CurrentUserNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "CURR_USER_NOT_FOUND";
        public static new readonly string Message = "Current user not found";
        public CurrentUserNotFoundException() : base(Status, Code, Message)
        {
        }
    }
}
