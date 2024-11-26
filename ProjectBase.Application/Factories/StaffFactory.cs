using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;

namespace ProjectBase.Application.Factories
{
    public class StaffFactory : UserFactory, IStaffFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        public StaffFactory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result> CreateUser(User user, UserCreateDTO dataCreate)
        {
            // check ReportTo
            User? manager = await _unitOfWork.UserRepository.GetByCondition(x
                => x.Username == dataCreate.ReportToPersonUsername);
            if (manager is null)
            {
                return Result.Failure(UserError.ManagerNotFound);
            }
            user.BranchID = manager.BranchID;

            return await base.CreateUser(user, dataCreate);
        }

        public override async Task<Result> UpdateUser(User user, UserCreateDTO dataUpdate)
        {
            if (dataUpdate.ReportToPersonUsername is not null)
            {
                var manager = await _unitOfWork.UserRepository.GetByCondition(x => x.Username == dataUpdate.ReportToPersonUsername);
                if (manager == null)
                {
                    return UserError.ManagerNotFound;
                }
                user.ReportTo = manager;
            }

            var branch = await _unitOfWork.BranchRepository.GetByCondition(x => x.Code == dataUpdate.BranchCode);
            if (branch == null)
            {
                return Result.Failure(UserError.ManagerNotFound);
            }
            user.BranchID = branch.Id;

            return await base.UpdateUser(user, dataUpdate);
        }
    }
}
