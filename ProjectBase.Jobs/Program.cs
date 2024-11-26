using Hangfire;
using ProjectBase.Jobs.Core.Configuration;
using ProjectBase.Jobs.StartUp;

var builder = WebApplication.CreateBuilder(args);

var appSetting = builder.Configuration.Get<AppSetting>() ??
    throw new Exception("AppSettingConfiguration is missing");

builder.Services.AddSingleton(appSetting);

builder.Services.AddDbConnectionString(appSetting);
builder.Services.AddAWSService(appSetting);
builder.Services.AddServices();
builder.Services.AddHangfireJobs();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangFireAuthorizationFilter() }
});

ServiceRegister.RecurrentJobs(appSetting);

app.UseHttpsRedirection();

app.Run();
