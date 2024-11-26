
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record BillError(string Code, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? Description = "")
    {
        public static readonly Error NotFound = new(
            "BILL_NOT_FOUND",
            Description: "Bill record not found!");

        public static readonly Error BillDetailsNotFound = new(
            "BILL_DETAILS_NOT_FOUND",
            Description: "Bill Details's item is missing!");

        public static readonly Error EmptyMessage = new(
            "BILL_NOT_FOUND",
            Description: "Bill record not found!");
    }
}
