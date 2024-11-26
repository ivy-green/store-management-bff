using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class BranchExistsException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "BRANCH_EXISTS";
        public static new readonly string Message = "Branch has already exists";
        public BranchExistsException() : base(Status, Code, Message)
        {
        }
    }
}
