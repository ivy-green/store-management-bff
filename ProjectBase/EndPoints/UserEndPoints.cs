using Microsoft.AspNetCore.Mvc;
using Polly.Registry;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Constants;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class UserEndPoints
    {
        public static void MapUserPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/user").RequireAuthorization("UserManagement");

            group.MapGet("", GetList);

            group.MapGet("profile", GetProfile);

            group.MapPut("profile/update", UpdateProfile);

            group.MapPost("", AddUser);

            group.MapDelete("", RemoveUser);

            group.MapPost("upload-file", UploadFile);

            group.MapGet("download-file", DownloadFile);
        }

        public static async Task<IResult> GetList(
            int pageIndex,
            int pageSize,
            IUserService _userService,
            HttpContext context,
            ResiliencePipelineProvider<string> pipelineProvider,
            string searchString = "",
            int branchCode = -1,
            string roles = "")
        {
            var policy = pipelineProvider.GetPipeline("gh-null-retry");
            var response = await policy.ExecuteAsync(async token =>
            {
                var currUsername = BaseController.GetCurrentUsername(context);
                if (currUsername is null)
                {
                    throw new UnauthorizedAccessException();
                }

                var res = await _userService.Search(pageIndex, pageSize, currUsername, searchString, branchCode, roles);
                return res;
            }, CancellationToken.None);

            return response.IsSuccess
                ? Results.Ok(response.Value)
                : Results.BadRequest(response.Error);
        }

        public static async Task<IResult> GetProfile(IUserService _userService, HttpContext context)
        {
            var currUsername = BaseController.GetCurrentUsername(context);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }

            var res = await _userService.GetProfile(currUsername);
            return res.IsSuccess
                ? Results.Ok(res.Value)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UpdateProfile(
            [FromBody] UserCreateDTO data,
            IUserService _userService,
            HttpContext context)
        {
            var currUsername = BaseController.GetCurrentUsername(context);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }

            var res = await _userService.UpdateProfile(currUsername, data);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> AddUser(
            [FromBody] UserCreateDTO data,
            HttpContext _context,
            IUserService _userService)
        {
            // get user role
            var user = BaseController.GetCurrentUser(_context);
            Result res;
            // register from anomynous user
            if (!user.Identity!.IsAuthenticated && (data.RoleCode == 4 || data.RoleCode == 5))
            {
                if (data.RoleCode == 4)
                {
                    // shipper need to be approved by admin or manager
                    res = await _userService.AddUser(data, isActivated: false);
                }
                else if (data.RoleCode == 5)
                {
                    // register normal customer
                    res = await _userService.AddUser(data);
                }
                else
                {
                    // cannot create other role with anomynous
                    return Results.Unauthorized();
                }
            }
            else
            {
                res = await _userService.AddUser(data);
            }
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UpdateUser(
            [FromBody] UserCreateDTO data,
            IUserService _userService)
        {
            var res = await _userService.UpdateUser(data);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> RemoveUser(
            [FromQuery] string username,
            IUserService _userService)
        {
            var res = await _userService.RemoveUser(username);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UploadFile([FromForm] IFormFile file, IUserService _userService)
        {
            var res = await _userService.UploadProfileImage(file, "thaomy310702@gmail.com"); // fix
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);

        }

        public static async Task<IResult> DownloadFile([FromQuery] string fileName, IUserService _userService)
        {
            var (res, content) = await _userService.DownloadProfileImage(fileName);
            return Results.File(res, content);
        }
    }
}
