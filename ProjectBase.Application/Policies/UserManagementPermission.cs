using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ProjectBase.Application.Policies
{
    [ExcludeFromCodeCoverage]
    public class UserManagementPermissionRequirement : IAuthorizationRequirement
    {
        public UserManagementPermissionRequirement() { }
    }

    [ExcludeFromCodeCoverage]
    public class UserManagementPermissionHandler : AuthorizationHandler<UserManagementPermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManagementPermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

#pragma warning disable CS1998
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserManagementPermissionRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                throw new NullException("HttpContext is missing");
            }

            var roles = context.User.FindAll(ClaimTypes.Role).ToList();
            var request = _httpContextAccessor.HttpContext.Request;

            if (roles.IsMatchingRole("Admin") ||
                request.IsProfileRequest() ||
                request.IsUpdateRequest("user") ||
                request.IsUpdateRequest("user/profile/update"))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
