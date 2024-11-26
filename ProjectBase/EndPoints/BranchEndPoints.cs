using Microsoft.AspNetCore.Mvc;
using ProjectBase.Domain.Constants;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class BranchEndPoints
    {
        public static void MapBranchPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/branch").RequireAuthorization("BranchManagement");

            group.MapGet("", GetList);

            group.MapPost("", AddBranch);

            group.MapPut("", UpdateBranch);

            group.MapDelete("{id}", RemoveBranch);
        }

        public static async Task<IResult> GetList([FromQuery] int pageIndex, [FromQuery] int pageSize, IBranchService _BranchService)
        {
            var res = await _BranchService.GetPagedList(pageIndex, pageSize);
            return res.IsSuccess
                ? Results.Ok(res.Value)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> AddBranch([FromBody] BranchCreateDTO data, IBranchService _BranchService)
        {
            var res = await _BranchService.AddBranch(data);

            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UpdateBranch([FromBody] BranchCreateDTO data, IBranchService _BranchService)
        {
            var res = await _BranchService.UpdateBranch(data);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> RemoveBranch([FromQuery] int id, IBranchService _BranchService)
        {
            var res = await _BranchService.RemoveBranch(id);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }
    }
}
