using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;

namespace ProjectBase.Application.Services
{
    // TODO: create background job to remove token that expired
    public class BlacklistService : IBlacklistService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlacklistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Add(string accessToken, string username)
        {
            Blacklist blacklist = new()
            {
                Token = accessToken
            };

            await _unitOfWork.BlacklistRepository.Add(blacklist);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsTokenExists(string accessToken)
        {
            // Check if token exists in BlackList
            Blacklist? existingToken = await _unitOfWork.BlacklistRepository.GetByCondition(x => x.Token == accessToken);
            return existingToken is not null;
        }

        public async Task<Result> Remove(string accessToken)
        {
            Blacklist? res = await _unitOfWork.BlacklistRepository.GetByCondition(x => x.Token == accessToken);
            if (res is null)
            {
                return BlacklistError.TokenNotFound;
            }

            _unitOfWork.BlacklistRepository.Remove(res);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
