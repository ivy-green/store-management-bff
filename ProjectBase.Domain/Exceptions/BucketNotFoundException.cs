using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class BucketNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "BUCKET_NOTFOUND";
        public static new readonly string Message = "Bucket not found";
        public BucketNotFoundException() : base(Status, Code, Message)
        {
        }
    }
}
