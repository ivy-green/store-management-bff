using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class ProductExistsException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "PRODUCT_EXISTS";
        public static new readonly string Message = "Product already exists";
        public ProductExistsException() : base(Status, Code, Message)
        {

        }
    }
}
