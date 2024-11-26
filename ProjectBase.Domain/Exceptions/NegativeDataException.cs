using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class NegativeDataException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "DATA_NEGATIVE";
        public static new readonly string Message = "Data cannot be negative";
        public NegativeDataException() : base(Status, Code, Message)
        {
        }
    }
}
