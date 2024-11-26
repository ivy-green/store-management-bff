using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class AuthEndPoints
    {
        public static void MapAuthPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth");

            group.MapPost("login", Login);

            group.MapPost("refreshToken", RefreshToken);

            group.MapPost("register", Register);

            group.MapPost("logout", Logout);
        }

        public static async Task<IResult> Login([FromBody] LoginRequestDTO data, IAuthService _authService)
        {
            var res = await _authService.Login(data);
            return res.IsSuccess
                ? Results.Ok(res.Value)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> RefreshToken([FromBody] LoginResponseDTO data, IAuthService _authService)
        {
            var res = await _authService.RefreshToken(data.RefreshToken ?? throw new Exception("Refresh token is missing"));
            return Results.Ok(res);
        }

        public static async Task<IResult> Register([FromBody] UserCreateDTO data, IAuthService _authService)
        {
            var res = await _authService.Register(data);
            return res.IsSuccess
                ? Results.Ok("Register successfully")
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> Logout(IAuthService _authService, HttpContext context)
        {
            var token = BaseController.ExtractTokenFromRequest(context);

            await _authService.Logout(token);

            return Results.Ok("Logout successfully");
        }
    }
}
