using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class UsernameExistsException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "USERNAME_EXISTS";
        public static new readonly string Message = "User has already exists";
        public UsernameExistsException() : base(Status, Code, Message)
        {
        }
    }
}
