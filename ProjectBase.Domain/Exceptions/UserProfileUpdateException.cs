using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class UserProfileUpdateException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "PROFILE_UPDATE_EXCEPTION";
        public static new readonly string Message = "You are not allowed to perform this action";
        public UserProfileUpdateException() : base(Status, Code, Message)
        {
        }
    }
}
