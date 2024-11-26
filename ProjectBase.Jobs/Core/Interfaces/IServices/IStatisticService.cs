using ProjectBase.Jobs.Core.Entities;

namespace ProjectBase.Jobs.Core.Interfaces.IServices
{
    public interface IStatisticService
    {
        Task<StatisticBill> AddBillStatistic(DateOnly today);
        Task<StatisticBill?> GetByDate(DateTime today);
        Task UpdateBillStatistic();
        Task UpdateGeneralStatistic();
    }
}