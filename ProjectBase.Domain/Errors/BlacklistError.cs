
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record BlacklistError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error NotFound = new(
            "BLACKLIST_NOT_FOUND",
            Description: "Blacklist record not found!");

        public static readonly Error TokenNotFound = new(
            "TOKEN_NOT_FOUND",
            Description: "Token not found in blacklist!");
    }
}
