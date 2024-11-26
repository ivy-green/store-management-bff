using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;

namespace ProjectBase.Application.Factories
{
    public class AdminFactory : UserFactory, IAdminFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminFactory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
