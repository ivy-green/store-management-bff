using ProjectBase.Insfracstructure.Data;
using ProjectBase.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using ProjectBase.Domain.Interfaces.IRepositories;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(AppDBContext context) : base(context)
        {

        }
    }
}
