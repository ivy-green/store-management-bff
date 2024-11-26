using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class ProductTypeExistsException : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "PRODUCT_TYPE_EXISTS";
        public static new readonly string Message = "Product type already exists";
        public ProductTypeExistsException() : base(Status, Code, Message)
        {
        }
    }
}
