using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class StatisticEndPoints
    {
        public static void MapStatisticPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/statistic")
                .RequireAuthorization()
                .RequireAuthorization("StatisticManagement");

            group.MapGet("revenue", GetRevenue);

            group.MapGet("revenue/days", GetRevenueByDays);

            group.MapGet("general", GetGeneralStatistic);

            group.MapGet("logger", GetLoggerItems);

            group.MapPost("logger/clear", ClearLogger);
        }

        public static async Task<IResult> GetRevenue(
            int pageIndex,
            int pageSize,
            IStatisticService _statisticService)
        {
            var res = await _statisticService.GetRevenue(pageSize, pageIndex);
            return Results.Ok(res.Value);
        }

        public static async Task<IResult> GetRevenueByDays(
            string startDate,
            int numberOfDays,
            IStatisticService _statisticService)
        {
            var res = await _statisticService.GetRevenueByDays(startDate, numberOfDays);
            return Results.Ok(res.Value);
        }

        public static async Task<IResult> GetRevenueByWeek(
            int pageIndex,
            int pageSize,
            IStatisticService _statisticService)
        {
            var res = await _statisticService.GetRevenue(pageSize, pageIndex);
            return Results.Ok(res.Value);
        }

        public static async Task<IResult> GetGeneralStatistic(
            int month,
            IStatisticService _statisticService)
        {
            var res = await _statisticService.GetGeneralStatistic(month);
            return res.IsSuccess
                ? Results.Ok(res.Value)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> GetLoggerItems(
            int pageIndex,
            int pageSize,
            AppSettingConfiguration _setting,
            ILoggerService _loggerService)
        {
            var res = await _loggerService.GetAll(_setting.DynamoDBTables!.LoggerTable);
            return Results.Ok(res);
        }

        public static async Task<IResult> ClearLogger(
            int pageIndex,
            int pageSize,
            AppSettingConfiguration _setting,
            ILoggerService _loggerService)
        {
            var res = await _loggerService.Remove(_setting.DynamoDBTables!.LoggerTable);
            return Results.Ok(res);
        }
    }
}
