using Microsoft.AspNetCore.Http;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Result<User>> GetUser(string username);
        Task<Result<User>> GetUserWithRole(string username);
        Task<Result> AddUser(UserCreateDTO user, bool isActivated = true);
        Task<Result<PageList<UserResponsesDTO>>> GetPagedList(int pageIndex, int pageSize, string currUsername);
        Task<Result<PageList<UserResponsesDTO>>> Search(
            int pageIndex, int pageSize,
            string currUsername,
            string searchString = "",
            int branchCode = -1, string roles = "");
        Task<Result<List<User>>> GetUserByRole(int pageIndex, int pageSize, int RoleCode);
        Task<Result> RemoveUser(string username);
        Task<Result> UpdateUser(UserCreateDTO user);
        Task<Result> UploadProfileImage(IFormFile file, string mail);
        Task<Result> UpdateCustomerType(string username);
        Task<Result> UpdateProfile(string username, UserCreateDTO user);
        Task<Result<UserProfileResponsesDTO>> GetProfile(string username);
        Task<(MemoryStream, string)> DownloadProfileImage(string fileName);
    }
}