using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class UsernameNotfoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "USERNAME_NOTFOUND";
        public static new readonly string Message = "User not found";
        public UsernameNotfoundException() : base(Status, Code, Message)
        {
        }
    }
}
