using ProjectBase.Jobs.Core.Entities;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Postgres;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Jobs.Insfrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BillRepository : RepositoryBase<Bill>, IBillRepository
    {
        public BillRepository(AppDbContext context) : base(context)
        {

        }
    }
}
