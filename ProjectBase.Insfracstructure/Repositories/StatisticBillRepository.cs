using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StatisticBillRepository : RepositoryBase<StatisticBill>, IStatisticBillRepository
    {
        public StatisticBillRepository(AppDBContext context) : base(context)
        {
        }
    }
}
