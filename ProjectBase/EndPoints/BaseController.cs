using ProjectBase.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class BaseController
    {
        public static ClaimsPrincipal GetCurrentUser(HttpContext context)
        {
            return context.User;
        }

        public static string? ExtractTokenFromRequest(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer "))
                {
                    return authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            return null;
        }

        public static string? GetCurrentUsername(HttpContext context)
        {
            return GetCurrentUser(context)?.Identity?.IsAuthenticated == true ?
                GetCurrentUserNameFromClaim(context) :
                null;
        }

        public static string? GetCurrentUserRole(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string GetIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "";
        }

        public static void SetCookie(Dictionary<string, string> cookieValues, HttpContext context)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                //Expires = refreshToken.Expiry,
                Expires = DateTime.UtcNow.AddMinutes(30), // temp
                Secure = true,
                SameSite = SameSiteMode.None
            };

            foreach (var cookie in cookieValues)
            {
                context.Response.Cookies.Append(cookie.Key, cookie.Value, cookieOptions);
            }
        }

        private static string GetCurrentUserNameFromClaim(HttpContext context)
        {
            return GetCurrentUser(context)?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullException("CurrentUser is null");
        }

        public static string GetRequestScheme(HttpContext context)
        {
            return context.Request.Scheme;
        }

        public static IEnumerable<Claim> GetListClaims(HttpContext context)
        {
            return context.User.Claims;
        }
    }
}
