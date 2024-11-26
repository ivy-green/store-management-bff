
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record BranchError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error NotFound = new(
            "BRANCH_NOT_FOUND",
            Description: "Branch not found!");

        public static readonly Error Exists = new(
            "BRANCH_EXISTS",
            Description: "Branch existing!");
    }
}
