using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class ProductNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "PRODUCT_NOTFOUND";
        public static new readonly string Message = "Product not found";
        public ProductNotFoundException() : base(Status, Code, Message)
        {

        }
    }
}
