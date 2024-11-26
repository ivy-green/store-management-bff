using ProjectBase.Domain.Abstractions;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IBlacklistService
    {
        Task<bool> IsTokenExists(string accessToken);
        Task<Result> Add(string accessToken, string username);
        Task<Result> Remove(string accessToken);
    }
}
