using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly IUserRoleRepository _roleRepository;
        public UserRepository(AppDBContext context, IUserRoleRepository roleRepository)
            : base(context)
        {
            _roleRepository = roleRepository;
        }

        public bool IsUserRole(User user, string[] roleName)
        {
            ExplicitLoadCollection(user, u => u.UserRoles);
            if (user.UserRoles is null)
            {
                throw new NullException("UserRole is missing");
            }

            foreach (var item in user.UserRoles)
            {
                _roleRepository.ExplicitLoad(item, item => item.Role);
            }

            return user.UserRoles.Any(item => item.Role is not null
                                            ? roleName.Any(x => x == item.Role.RoleName)
                                            : throw new NullException("Role is missing"));
        }

        public void LoadUserRole(ref User user)
        {
            ExplicitLoadCollection(user, u => u.UserRoles);
            if (user.UserRoles is null)
            {
                throw new NullException("UserRole is missing");
            }

            foreach (var item in user.UserRoles)
            {
                _roleRepository.ExplicitLoad(item, item => item.Role);
            }
        }
    }
}
