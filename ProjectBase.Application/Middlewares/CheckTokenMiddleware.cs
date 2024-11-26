using Microsoft.AspNetCore.Http;
using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Application.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class CheckTokenMiddleware : IMiddleware
    {
        private readonly IBlacklistService _blacklistService;

        public CheckTokenMiddleware(IBlacklistService blacklistService)
        {
            _blacklistService = blacklistService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            if (request.IsLoginRequest() || request.IsRegisterRequest() || request.IsRefreshToken())
            {
                await next(context);
                return;
            }

            var token = request.GetJWTToken();
            if (token is null)
            {
                throw new UnauthorizedAccessException();
            }

            var isTokenExists = await _blacklistService.IsTokenExists(token);

            if (isTokenExists)
            {
                // custom error
                context.Response.StatusCode = 401; // business error
                context.Response.Headers.TryAdd("Content-Type", "application/json");
                await context.Response.WriteAsync("Invalid Token");
                return;
            }

            await next(context);
        }
    }
}
