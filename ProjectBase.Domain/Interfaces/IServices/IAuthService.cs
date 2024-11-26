using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDTO>> Login(LoginRequestDTO data);
        Task<Result> Register(UserCreateDTO data);
        Task Logout(string? accessToken);
        Task<LoginResponseDTO> RefreshToken(string refreshToken);
    }
}