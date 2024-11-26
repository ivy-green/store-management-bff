using ProjectBase.Insfracstructure.Data;
using ProjectBase.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using ProjectBase.Domain.Interfaces.IRepositories;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BillDetailsRepository : RepositoryBase<BillDetails>, IBillDetailsRepository
    {
        public BillDetailsRepository(AppDBContext context) : base(context)
        {
        }
    }
}
