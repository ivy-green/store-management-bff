
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record StatisticError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error InvalidDateTime = new(
            "INVALID_DATE",
            Description: "Date is invalid!");
    }
}
