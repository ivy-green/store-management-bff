using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IStatisticService
    {
        Task<Result<GeneralStatisticByMonthResponseDTO?>> GetGeneralStatistic(int month);
        Task<Result<PageList<StatisticBill>>> GetRevenue(int pageIndex, int pageSize);
        Task<Result<PageList<StatisticBill>>> GetRevenueByDays(string startDate, int numberOfDays);
    }
}