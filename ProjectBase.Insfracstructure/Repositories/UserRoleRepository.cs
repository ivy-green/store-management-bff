using ProjectBase.Insfracstructure.Data;
using ProjectBase.Domain.Entities;

using System.Diagnostics.CodeAnalysis;
using ProjectBase.Domain.Interfaces.IRepositories;
namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(AppDBContext context) : base(context)
        {

        }
    }
}
