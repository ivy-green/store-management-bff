
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record ProductTypeError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error NotFound = new(
            "PRODUCT_TYPE_NOT_FOUND",
            Description: "ProductType record not found!");

    }
}
