using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;

namespace ProjectBase.Application.Factories
{
    public abstract class UserFactory : IUserFactory
    {
        private IUnitOfWork _unitOfWork;
        protected UserFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public virtual async Task<Result> CreateUser(User user, UserCreateDTO dataCreate)
        {
            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public virtual async Task<Result> UpdateUser(User userExists, UserCreateDTO dataUpdate)
        {
            userExists = dataUpdate.ProjectToEntity<UserCreateDTO, User>(userExists);
            userExists.IsAccountBlocked = dataUpdate.SetAccountBlocked ?? userExists.IsAccountBlocked;

            _unitOfWork.UserRepository.Update(userExists);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
