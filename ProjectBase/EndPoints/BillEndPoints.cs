using Microsoft.AspNetCore.Mvc;
using Polly.Registry;
using ProjectBase.Domain.Constants;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class BillEndPoints
    {
        public static void MapBillPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/bill").RequireAuthorization();

            group.MapGet("", GetList);

            group.MapPost("", AddBill);

            group.MapPut("", UpdateStatus);
        }

        public static async Task<IResult> GetList(
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromQuery] BillFilter statuses,
            HttpContext _httpContext,
            IBillService _BillService)
        {
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }
            
            var res = await _BillService.GetPagedList(pageIndex, pageSize, currUsername, statuses);
            return Results.Ok(res.Value);
        }

        public static async Task<IResult> AddBill(
            BillCreateDTO data,
            ResiliencePipelineProvider<string> pipelineProvider,
            HttpContext _httpContext,
            IBillService _BillService)
        {
            var policy = pipelineProvider.GetPipeline("gh-message-retry");
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }
            
            var res = await policy.ExecuteAsync(async token =>
                await _BillService.AddBill(data, currUsername),
                CancellationToken.None);

            return res.IsSuccess
                ? Results.Ok("Add Bill successfully")
                : Results.BadRequest();
        }

        public static async Task<IResult> UpdateStatus(
            string billId,
            int status,
            HttpContext _httpContext,
            IBillService _billService)
        {
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }
            
            var res = await _billService.UpdateStatus(billId, status, currUsername);
            return Results.Ok(ResponseMessages.SUCCESS);
        }
    }
}
