using ProjectBase.Jobs.Core.Interfaces.IRepositories;

namespace ProjectBase.Jobs.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IBillRepository BillRepository { get; }
        IStatisticBillRepository StatisticBillRepository { get; }
        IProductRepository ProductRepository { get; }
        IDynamoRepository DynamoRepository { get; }
        Task SaveChangesAsync();
    }
}
