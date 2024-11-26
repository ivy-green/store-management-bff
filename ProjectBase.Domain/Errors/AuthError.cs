
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record AuthError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error WrongPassword = new(
            "WRONG_PASSWORD",
            Description: "Wrong password");

        public static readonly Error UserBlocked = new(
            "USER_BLOCKED",
            Description: "User has been blocked. Please contact to Manager/ Admin.");
    }
}
