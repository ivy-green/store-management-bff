using ProjectBase.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StatisticRepository
    {
        IUnitOfWork _unitOfWork;
        public StatisticRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
