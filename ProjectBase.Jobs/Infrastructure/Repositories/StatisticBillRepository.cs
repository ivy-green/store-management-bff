using ProjectBase.Jobs.Core.Entities;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Postgres;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Jobs.Insfrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StatisticBillRepository : RepositoryBase<StatisticBill>, IStatisticBillRepository
    {
        public StatisticBillRepository(AppDbContext context) : base(context)
        {
        }
    }
}
