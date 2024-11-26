using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ProjectBase.Application.Policies
{
    [ExcludeFromCodeCoverage]
    public class StatisticManagementPermissionRequirement : IAuthorizationRequirement
    {
        public StatisticManagementPermissionRequirement() { }
    }

    [ExcludeFromCodeCoverage]
    public class StatisticManagementPermissionHandler : AuthorizationHandler<StatisticManagementPermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StatisticManagementPermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

#pragma warning disable CS1998
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StatisticManagementPermissionRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                throw new NullException("HttpContext is missing");
            }

            var roles = context.User.FindAll(ClaimTypes.Role).ToList();
            var request = _httpContextAccessor.HttpContext.Request;

            if (roles.IsMatchingRole("Admin")
                || roles.IsMatchingRole("Manager"))
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
