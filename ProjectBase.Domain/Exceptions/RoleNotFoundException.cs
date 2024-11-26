using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class RoleNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "ROLE_NOTFOUND";
        public static new readonly string Message = "Role is not valid";
        public RoleNotFoundException() : base(Status, Code, Message)
        {
        }
    }
}
