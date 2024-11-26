
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record UserError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error SameUser = new(
            "SAME_USER",
            Description: "User cannot update other user's profile");

        public static readonly Error UserNotFound = new(
            "USER_NOT_FOUND",
            Description: "User not found");

        public static readonly Error CustomerNotFound = new(
            "CUSTOMER_NOT_FOUND",
            Description: "Customer not found");

        public static readonly Error RoleNotFound = new(
            "ROLE_NOT_FOUND",
            Description: "Role not found");

        public static readonly Error UsernameNotFound = new(
            "USERNAME_NOT_FOUND",
            Description: "Username not found");

        public static readonly Error UsernameExists = new(
            "USERNAME_EXISTS",
            Description: "Username exists!");

        public static readonly Error UseRoleIsNull = new(
            "USERROLE_NULL",
            Description: "UserRole not found");

        public static readonly Error ManagerNotFound = new(
            "MANAGER_NOT_FOUND",
            Description: "Manager not found");

        public static readonly Error InvalidPassword = new(
            "PASSWORD_INVALID",
            Description: "Password must be at least 8 characters");

        public static readonly Error FullnameNotFound = new(
            "FULLNAME_NOT_FOUND",
            Description: "Fullname is missing");
    }
}
