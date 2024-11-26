using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class PriceLessThan1000Exception : AppException
    {
        public static readonly HttpStatusCode Status = HttpStatusCode.BadRequest;
        public static new readonly string Code = "PRICE_INVALID";
        public static new readonly string Message = "Price cannot less than 1000";
        public PriceLessThan1000Exception() : base(Status, Code, Message)
        {
        }
    }
}
