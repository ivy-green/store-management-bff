using ProjectBase.Domain.Interfaces.IRepositories;

namespace ProjectBase.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IBlacklistRepository BlacklistRepository { get; }
        IProductOnSaleRepository ProductOnSaleRepository { get; }
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IRoleRepository RoleRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IBillRepository BillRepository { get; }
        IBillDetailsRepository BillDetailsRepository { get; }
        IStatisticBillRepository StatisticBillRepository { get; }
        IBranchRepository BranchRepository { get; }
        Task SaveChangesAsync();
    }
}
