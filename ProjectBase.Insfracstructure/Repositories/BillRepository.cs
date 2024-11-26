using ProjectBase.Insfracstructure.Data;
using ProjectBase.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using ProjectBase.Domain.Interfaces.IRepositories;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BillRepository : RepositoryBase<Bill>, IBillRepository
    {
        public BillRepository(AppDBContext context) : base(context)
        {
        }
    }
}
