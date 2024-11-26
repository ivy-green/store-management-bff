using Amazon.DynamoDBv2;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using ProjectBase.Jobs.ApplicationLogic.Services;
using ProjectBase.Jobs.ApplicationLogic.Utils;
using ProjectBase.Jobs.Core.ApplicationLogic.UnitOfWork;
using ProjectBase.Jobs.Core.Configuration;
using ProjectBase.Jobs.Core.Interfaces;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Core.Interfaces.IServices;
using ProjectBase.Jobs.Insfrastructure.Repositories;
using ProjectBase.Jobs.Postgres;

namespace ProjectBase.Jobs.StartUp
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddHttpClient();

            service.AddScoped<IProductRepository, ProductRepository>();
            service.AddScoped<IBillRepository, BillRepository>();
            service.AddScoped<IStatisticBillRepository, StatisticBillRepository>();
            service.AddScoped<IDynamoRepository, DynamoRepository>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();

            service.AddScoped<IStatisticService, StatisticService>();

            service.AddScoped<ISqsMessage, SqsMessage>();
            //service.AddScoped<ISnsMessage, SnsMessage>();

            service.AddScoped<DynamoSetting>();

            return service;
        }

        public static IServiceCollection AddDbConnectionString(
            this IServiceCollection service,
            AppSetting setting)
        {
            if (setting.Database is null)
            {
                throw new Exception("ConnectionString is missing");
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

            service.AddDbContext<AppDbContext>(option =>
            {
                retryPolicy.Execute(() =>
                {
                    option.UseNpgsql(setting.Database.DefaultConnection);
                });
            });

            return service;
        }

        public static IServiceCollection AddHangfireJobs(this IServiceCollection service)
        {
            service.AddHangfire(c => c.UseMemoryStorage());
            service.AddHangfireServer();

            return service;
        }

        public static void RecurrentJobs(AppSetting appSettingConfiguration)
        {
            if (appSettingConfiguration.RecurrentJobs is null || appSettingConfiguration.RecurrentJobs.BackgroundConfigs is null)
            {
                throw new Exception("Recurrent job setting is missing");
            }

            // update bill statistic
            RecurringJob.AddOrUpdate<IStatisticService>(
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[0].JobId,
                x => x.UpdateBillStatistic(),
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[0].Duration);

            RecurringJob.AddOrUpdate<IStatisticService>(
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[2].JobId,
                x => x.UpdateGeneralStatistic(),
                appSettingConfiguration.RecurrentJobs.BackgroundConfigs[2].Duration);
        }

        public static IServiceCollection AddAWSService(
            this IServiceCollection service,
            AppSetting setting)
        {
            if (setting.AWSSection is null)
            {
                throw new Exception("AWS setting is missing");
            }

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
    }
}
