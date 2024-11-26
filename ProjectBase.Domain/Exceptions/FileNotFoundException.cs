using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class FileNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "FILE_NOTFOUND";
        public static new readonly string Message = "File not found";
        public FileNotFoundException() : base(Status, Code, Message)
        {
        }
    }
}
