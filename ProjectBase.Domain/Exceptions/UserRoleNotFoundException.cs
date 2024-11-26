using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class UserRoleNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "USERROLE_NOTFOUND";
        public static new readonly string Message = "UserRole is missing";
        public UserRoleNotFoundException() : base(Status, Code, Message)
        {
        }
    }
}
