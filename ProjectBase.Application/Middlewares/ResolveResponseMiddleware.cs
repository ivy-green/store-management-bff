using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjectBase.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Application.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class ResolveResponseMiddleware : IMiddleware
    {
        public ResolveResponseMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            if (context.Items["Result"] is Result result)
            {
                context.Response.StatusCode = (int)result.StatusCode;
                context.Response.Headers.TryAdd("Content-Type", "application/json");

                if (result.IsSuccess && result.GetType().IsGenericType)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
                else
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result.Error));
                    context.Response.StatusCode = (int)result.StatusCode;
                }
            }

        }
    }
}
