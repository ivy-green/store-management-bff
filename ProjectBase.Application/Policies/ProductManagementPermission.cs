using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ProjectBase.Application.Policies
{
    [ExcludeFromCodeCoverage]
    public class ProductManagementPermissionRequirement : IAuthorizationRequirement
    {
        public ProductManagementPermissionRequirement() { }
    }

    [ExcludeFromCodeCoverage]
    public class ProductManagementPermissionHandler : AuthorizationHandler<ProductManagementPermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductManagementPermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

#pragma warning disable CS1998
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProductManagementPermissionRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                throw new NullException("HttpContext is missing");
            }

            var roles = context.User.FindAll(ClaimTypes.Role).ToList();
            var request = _httpContextAccessor.HttpContext.Request;

            if (roles.IsMatchingRole("Admin") ||
                request.IsGetListRequest("product/market")
                || request.IsGetListRequest("product")
                || (roles.IsMatchingRole("Manager") && (request.IsUpdateRequest("product") || request.IsDeleteRequest("product")))
                || (roles.IsMatchingRole("Staff") && request.IsUpdateRequest("product/sale")))
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
