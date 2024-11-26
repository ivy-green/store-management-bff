using Hangfire;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using ProjectBase;
using ProjectBase.Application.Middlewares;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.Exceptions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appConfig = builder.Configuration.Get<AppSettingConfiguration>() ?? throw new NullException("AppSettingConfiguration is missing");
var allowedHosts = builder.Configuration.GetSection("AllowedHosts").Get<string[]>() ?? [];

builder.Services.AddCorsPolicy(allowedHosts);

builder.Services.AddSingleton(appConfig);
builder.Services.AddDbConnectionString(appConfig);

builder.Services.AddAWSService(appConfig);
builder.Services.AddAzureService(appConfig);

builder.Services.AddJwtAuthenticate(appConfig);
builder.Services.AddSwaggerJwtService();
builder.Services.AddServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUserManagementPolicies();
builder.Services.AddCacheService();
builder.Services.AddHangfireJobs();
builder.Services.AddFaultHandlingService();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MigrationInitialisation();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangFireAuthorizationFilter() }
});

ServiceRegister.RecurrentJobs(appConfig);

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
});

app.UseSerilogRequestLogging();

app.UseCors("CorsPolicy");

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<ResolveResponseMiddleware>();

app.UseAuthentication();

app.UseMiddleware<CheckTokenMiddleware>();

app.UseAuthorization();

app.AddRouteMapping(); // minimal APIs

app.Run();
