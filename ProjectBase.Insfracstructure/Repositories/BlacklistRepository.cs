using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BlacklistRepository : RepositoryBase<Blacklist>, IBlacklistRepository
    {
        public BlacklistRepository(AppDBContext context) : base(context)
        {

        }
    }
}
