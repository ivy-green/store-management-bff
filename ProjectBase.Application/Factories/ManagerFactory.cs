using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;

namespace ProjectBase.Application.Factories
{
    public class ManagerFactory : UserFactory, IManagerFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManagerFactory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result> CreateUser(User user, UserCreateDTO dataCreate)
        {
            // check branchCode
            Branch? branch = await _unitOfWork.BranchRepository
                                            .GetByCondition(x => x.Code == dataCreate.BranchCode);
            if (branch is null)
            {
                return BranchError.NotFound;
            }

            user.BranchID = branch.Id;
            return await base.CreateUser(user, dataCreate);
        }

        public override async Task<Result> UpdateUser(User user, UserCreateDTO dataUpdate)
        {
            var branch = await _unitOfWork.BranchRepository.GetByCondition(x => x.Code == dataUpdate.BranchCode);
            if (branch == null)
            {
                return UserError.ManagerNotFound;
            }
            user.BranchID = branch.Id;

            return await base.UpdateUser(user, dataUpdate);
        }

    }
}
