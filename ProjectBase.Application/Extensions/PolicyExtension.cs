using Microsoft.AspNetCore.Http;
using ProjectBase.Domain.Exceptions;
using System.Security.Claims;

namespace ProjectBase.Application.Extensions
{
    internal static class PolicyExtension
    {
        #region Role
        public static bool IsMatchingRole(this List<Claim> roles, string roleName)
        {
            return roles.Select(x => x.Value).Any(x => x.ToString() == roleName);
        }
        #endregion

        #region CURD Route
        public static bool IsProfileRequest(this HttpRequest request)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }

            return requestRoute.Value.Contains("profile") && requestMethod == "GET";
        }

        public static bool IsUpdateRequest(this HttpRequest request, string route)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }
            return requestRoute.Value.Equals($"/api/{route}") &&
                (requestMethod == "POST" || requestMethod == "PUT");
        }

        public static bool IsDeleteRequest(this HttpRequest request, string route)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }
            return requestRoute.Value.Contains($"/api/{route}") &&
                (requestMethod == "DELETE" || requestMethod == "PUT");
        }

        public static bool IsGetListRequest(this HttpRequest request, string route)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }
            return requestRoute.Value.Equals($"/api/{route}") && requestMethod == "GET";
        }
        #endregion

        #region AuthRoute
        public static bool IsLoginRequest(this HttpRequest request)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }

            return requestRoute.Value.Equals($"/api/auth/login") && requestMethod == "POST";
        }

        public static bool IsRegisterRequest(this HttpRequest request)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }

            return requestRoute.Value.Equals($"/auth/register") && requestMethod == "POST";
        }

        public static bool IsRefreshToken(this HttpRequest request)
        {
            var requestRoute = request.Path;
            var requestMethod = request.Method;

            if (requestRoute.Value is null)
            {
                throw new NullException("PathString request is missing");
            }

            return requestRoute.Value.Equals($"/auth/refreshToken") && requestMethod == "POST";
        }

        public static string? GetJWTToken(this HttpRequest request)
        {
            return request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
        }
        #endregion
    }
}
