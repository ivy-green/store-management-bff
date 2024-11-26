using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.Interfaces.IRepositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        bool IsUserRole(User user, string[] roleName);
        void LoadUserRole(ref User user);
    }
}
