using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Enums;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;

namespace ProjectBase.Application.Factories
{
    public class CustomerFactory : UserFactory, ICustomerFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerFactory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result> CreateUser(User user, UserCreateDTO dataCreate)
        {
            var initialCustomerType = CustomerType.FromName("Copper");
            if (initialCustomerType is null)
            {
                return Result.Failure(Error.InvalidType);
            }

            user.Type = initialCustomerType.Value;

            return await base.CreateUser(user, dataCreate);
        }
    }
}
