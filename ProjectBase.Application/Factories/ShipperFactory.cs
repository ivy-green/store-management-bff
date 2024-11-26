using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;

namespace ProjectBase.Application.Factories
{
    public class ShipperFactory : UserFactory, IShipperFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShipperFactory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
