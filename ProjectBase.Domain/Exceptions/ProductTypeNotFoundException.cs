using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class ProductTypeNotFoundException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "PRODUCT_TYPE_NOTFOUND";
        public static new readonly string Message = "Product type not found";
        public ProductTypeNotFoundException() : base(Status, Code, Message)
        {

        }
    }
}
