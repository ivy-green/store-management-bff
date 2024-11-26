using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Azure.Storage.Blobs;
using DynamoService.Repositories.Implements;
using DynamoService.Repositories.Interfaces;
using DynamoService.Settings;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Retry;
using ProjectBase.Application.Factories;
using ProjectBase.Application.Middlewares;
using ProjectBase.Application.Policies;
using ProjectBase.Application.Services;
using ProjectBase.Application.UnitOfWork;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.HealthCheck;
using ProjectBase.Insfracstructure.Data;
using ProjectBase.Insfracstructure.Repositories;
using ProjectBase.Insfracstructure.Services.Azure.Blob;
using ProjectBase.Insfracstructure.Services.FileService;
using ProjectBase.Insfracstructure.Services.Jwts;
using ProjectBase.Insfracstructure.Services.Mail;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using ProjectBase.Insfracstructure.Services.Message.SQS;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ProjectBase
{
    [ExcludeFromCodeCoverage]
    public static class ServiceRegister
    {
        #region Application Services
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddHttpClient();

            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IUserRoleRepository, UserRoleRepository>();
            service.AddScoped<IRoleRepository, RoleRepository>();
            service.AddScoped<IProductRepository, ProductRepository>();
            service.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            service.AddScoped<IBillRepository, BillRepository>();
            service.AddScoped<IBillDetailsRepository, BillDetailsRepository>();
            service.AddScoped<IBlacklistRepository, BlacklistRepository>();
            service.AddScoped<IStatisticBillRepository, StatisticBillRepository>();
            service.AddScoped<IDynamoRepository, DynamoRepository>();
            service.AddScoped<IBranchRepository, BranchRepository>();
            service.AddScoped<IProductOnSaleRepository, ProductOnSaleRepository>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();

            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IFileService, FileService>();
            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<IHashService, SHA256HashService>();
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IProductTypeService, ProductTypeService>();
            service.AddScoped<IBillService, BillService>();
            service.AddScoped<IBlacklistService, BlacklistService>();
            service.AddScoped<IEmailService, EmailService>();
            service.AddScoped<IStatisticService, StatisticService>();
            service.AddScoped<ILoggerService, LoggerService>();
            service.AddScoped<IBranchService, BranchService>();
            service.AddScoped<IProductOnSaleService, ProductOnSaleService>();

            service.AddScoped<ISqsMessage, SqsMessage>();
            service.AddScoped<ISnsMessage, SnsMessage>();
            service.AddScoped<IBlobService, BlobService>();

            service.AddTransient<IAdminFactory, AdminFactory>();
            service.AddTransient<IManagerFactory, ManagerFactory>();
            service.AddTransient<IStaffFactory, StaffFactory>();
            service.AddTransient<IShipperFactory, ShipperFactory>();
            service.AddTransient<ICustomerFactory, CustomerFactory>();

            service.AddScoped<CheckTokenMiddleware>();
            service.AddScoped<ResolveResponseMiddleware>();
            service.AddScoped<DynamoSetting>();

            return service;
        }
        #endregion

        #region DBContext
        public static IServiceCollection AddDbConnectionString(
            this IServiceCollection service,
            AppSettingConfiguration setting)
        {
            if (setting.Database is null)
            {
                throw new NullException("ConnectionString is missing");
            }

            RetryPolicy retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    retryCount: 5,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, delay, retryCount, context) =>
                    {
                        // Log retry attempt
                        Console.WriteLine($"Retry {retryCount} due to: {exception}");
                    });

            service.AddDbContext<AppDBContext>(option =>
            {
                retryPolicy.Execute(() =>
                {
                    option.UseNpgsql(setting.Database.DefaultConnection);
                });
            });

            service.AddHealthChecks()
                .AddNpgSql(
                    setting.Database.DefaultConnection,
                    name: "PostgreSQL")
                .AddCheck<DatabaseHealthCheck>("Sample");

            return service;
        }
        #endregion

        #region JWT Authentication

        public static IServiceCollection AddJwtAuthenticate(this IServiceCollection service, AppSettingConfiguration setting)
        {
            if (setting.JWTSection is null || setting.JWTSection.SecretKey is null)
            {
                throw new NullException("JWT setting is missing");
            }

            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(setting.JWTSection.SecretKey)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            return service;
        }

        #endregion

        #region Swagger JWT

        public static IServiceCollection AddSwaggerJwtService(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return service;
        }

        #endregion

        #region AWS Services
        public static IServiceCollection AddAWSService(
            this IServiceCollection service,
            AppSettingConfiguration setting)
        {
            if (setting.AWSSection is null)
            {
                throw new NullException("AWS setting is missing");
            }

            service.AddSingleton<IAmazonS3>(sp =>
            {
                var awsOptions = setting.AWSSection;
                var config = new AmazonS3Config
                {
                    ServiceURL = awsOptions.LocalstackUrl,
                    ForcePathStyle = true
                };
                return new AmazonS3Client(awsOptions.AccessKey, awsOptions.Secret, config);
            });

            service.AddSingleton<IAmazonSQS>(sp =>
            {
                var awsOptions = setting.AWSSection;
                var config = new AmazonSQSConfig
                {
                    ServiceURL = awsOptions.LocalstackUrl,
                };
                return new AmazonSQSClient(awsOptions.AccessKey, awsOptions.Secret, config);
            });

            service.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var awsOptions = setting.AWSSection;
                var config = new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = awsOptions.LocalstackUrl,
                    //RegionEndpoint = RegionEndpoint.USEast1
                };
                return new AmazonSimpleNotificationServiceClient(
                    awsOptions.AccessKey,
                    awsOptions.Secret,
                    config);
            });

            service.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var awsOptions = setting.AWSSection;
                var config = new AmazonDynamoDBConfig
                {
                    ServiceURL = awsOptions.LocalstackUrl,
                    //RegionEndpoint = RegionEndpoint.USEast1
                };
                return new AmazonDynamoDBClient(
                    awsOptions.AccessKey,
                    awsOptions.Secret,
                    config);
            });

            return service;
        }
        #endregion

        #region Cors Policy
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string[] AllowHost)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.WithOrigins(AllowHost)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials());
            });

            return services;
        }
        #endregion

        #region Azure Service

        public static IServiceCollection AddAzureService(this IServiceCollection service, AppSettingConfiguration setting)
        {
            if (setting.AzureSection is null)
            {
                throw new NullException("Azure setting is missing");
            }

            service.AddSingleton(sp =>
            {
                return new BlobServiceClient(setting.AzureSection.ConnectionString);
            });

            return service;
        }

        #endregion

        #region Policies
        public static IServiceCollection AddUserManagementPolicies(this IServiceCollection service)
        {
            service.AddHttpContextAccessor();
            service.AddSingleton<IAuthorizationHandler, UserManagementPermissionHandler>();
            service.AddSingleton<IAuthorizationHandler, ProductManagementPermissionHandler>();
            service.AddSingleton<IAuthorizationHandler, BranchManagementPermissionHandler>();
            service.AddSingleton<IAuthorizationHandler, StatisticManagementPermissionHandler>();

            service.AddAuthorization(options =>
            {
                options.AddPolicy("UserManagement", policy =>
                    policy.Requirements.Add(new UserManagementPermissionRequirement()));

                options.AddPolicy("ProductManagement", policy =>
                    policy.Requirements.Add(new ProductManagementPermissionRequirement()));

                options.AddPolicy("BranchManagement", policy =>
                    policy.Requirements.Add(new BranchManagementPermissionRequirement()));

                options.AddPolicy("StatisticManagement", policy =>
                    policy.Requirements.Add(new StatisticManagementPermissionRequirement()));
            });

            return service;
        }
        #endregion

        #region Hangfire jobs
        public static IServiceCollection AddHangfireJobs(this IServiceCollection service)
        {
            service.AddHangfire(c => c.UseMemoryStorage());
            service.AddHangfireServer();

            return service;
        }

        public static void RecurrentJobs(AppSettingConfiguration appSettingConfiguration)
        {
            if (appSettingConfiguration.RecurrentJobs is null || appSettingConfiguration.RecurrentJobs.BackgroundConfigs is null)
            {
                throw new NullException("Recurrent job setting is missing");
            }

            // clear logger & save log to s3 bucket
            RecurringJob.AddOrUpdate<ILoggerService>(
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[1].JobId,
                x => x.Remove(appSettingConfiguration.DynamoDBTables!.LoggerTable),
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[1].Duration);

            // update product cache list
            RecurringJob.AddOrUpdate<IProductService>(
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[3].JobId,
                x => x.UpdateDataInCache(),
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[3].Duration);

            // healthcheck
            //RecurringJob.AddOrUpdate<HealthCheckService>(
            //    appSettingConfiguration.RecurrentJobs.BackgroundConfigs[2].JobId,
            //    async (x) =>
            //    {
            //        await x.CheckHealthAsync();
            //    },
            //    appSettingConfiguration.RecurrentJobs.BackgroundConfigs[2].Duration);
        }
        #endregion

        #region Fault Handling
        public static IServiceCollection AddFaultHandlingService(this IServiceCollection service)
        {
            service.AddResiliencePipeline("gh-null-retry",
                pipelineBuilder =>
                {
                    pipelineBuilder.AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 2,
                        BackoffType = DelayBackoffType.Constant,
                        Delay = TimeSpan.Zero, // delay between retry = 0
                        ShouldHandle = new PredicateBuilder()
                            .Handle<NullException>(),
                        OnRetry = retryArguments =>
                        {
                            Console.WriteLine($"Retry attemp: {retryArguments.AttemptNumber}, " +
                                $"{retryArguments.Outcome.Exception}");
                            return ValueTask.CompletedTask;
                        }
                    });
                });

            service.AddResiliencePipeline("gh-message-retry",
                pipelineBuilder =>
                {
                    pipelineBuilder.AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 2,
                        BackoffType = DelayBackoffType.Constant,
                        Delay = TimeSpan.Zero,
                        ShouldHandle = new PredicateBuilder()
                            .Handle<AmazonSimpleNotificationServiceException>(),
                        OnRetry = retryArguments =>
                        {
                            Console.WriteLine($"SNS Error - Retry attemp: {retryArguments.AttemptNumber}, " +
                                $"{retryArguments.Outcome.Exception}");
                            return ValueTask.CompletedTask;
                        }
                    });
                });

            return service;
        }
        #endregion

        public static IServiceCollection AddHealthCheckService(this IServiceCollection service)
        {
            service.AddHealthCheckService();

            return service;
        }

        public static IServiceCollection AddResultPatternService(this IServiceCollection service)
        {
            service.AddSingleton<Result>();

            return service;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection service)
        {
            service.AddDistributedMemoryCache();
            service.AddSingleton<ICacheService, CacheService>();

            return service;
        }
    }

}
