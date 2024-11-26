using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces.IServices;
using Serilog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProjectBase.Application.Middlewares
{
    [ExcludeFromCodeCoverage]
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
    [ExcludeFromCodeCoverage]
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private ILoggerService? _loggerService;
        private AppSettingConfiguration _setting;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger logger, AppSettingConfiguration setting)
        {
            _next = next;
            _logger = logger;
            _setting = setting;
        }

        public async Task InvokeAsync(HttpContext context, ILoggerService loggerService)
        {
            _loggerService = loggerService;

            var requestRoute = context.Request.Path.Value;
            var requestMethod = context.Request.Method;

            if (requestRoute == null || requestMethod == null)
            {
                throw new NullException("Route request or method not found");
            }

            var loggerItem = new LoggerCreateRequestDTO
            {
                Action = requestMethod,
                Service = requestRoute,
            };

            Stopwatch stopWatch = new Stopwatch();
            _logger.Information($"Start: {requestRoute}; {requestMethod}; ");

            try
            {
                stopWatch.Start();

                await _next(context);

                stopWatch.Stop();
                loggerItem.Description = $"Action Successfully";

                if (!requestRoute.Contains("logger"))
                {
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                        ts.Hours, ts.Minutes, ts.Seconds,
                                                        ts.Milliseconds / 10);

                    _logger.Information($"Done{elapsedTime}ms:{requestRoute};{requestMethod};");
                    loggerItem.Message = $"Done in {elapsedTime}ms;Route:{requestRoute};Method:{requestMethod}";

                    loggerItem.Driver = Guid.NewGuid().ToString();
                    loggerItem.Team = DateTime.UtcNow.ToString();

                    await _loggerService.Add(loggerItem, _setting.DynamoDBTables!.LoggerTable);
                }
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                loggerItem.Description = $"Action Failed;Error:{ex.Message}";

                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds / 10);

                _logger.Information($"Done{elapsedTime}ms:{requestRoute};{requestMethod};");
                loggerItem.Message = $"Done in {elapsedTime}ms;Route:{requestRoute};Method:{requestMethod}";

                await _loggerService.Add(loggerItem, _setting.DynamoDBTables!.LoggerTable);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string? code = null;
            string message = "";
            List<string> errors = [];

            if (exception is AppException appException)
            {
                code = appException.Code;
                message = appException.Message != "" ? appException.Message : exception.Message;
                errors = appException.Errors;
            }
            else
            {
                code = "INTERNAL_EXCEPTION";
                message = exception.Message;
            }

            var result = new GlobalExceptionDTO
            {
                Code = code,
                Message = message,
                Errors = errors,
                StatusCode = exception is AppException httpstatus ? (int)httpstatus.StatusCode : 500,
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.StatusCode;

            _logger.Error($"Error: {JsonConvert.SerializeObject(result)}");
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
