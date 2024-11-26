using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class NullException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "NULL_EXCEPTION";
        public static new readonly string Message = "Data cannot be null";
        public NullException() : base(Status, Code, Message)
        {
        }
        public NullException(string message) : base(Status, Code, message)
        {
        }
    }
}
