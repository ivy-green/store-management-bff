using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IBillService
    {
        Task<Result> AddBill(BillCreateDTO Bill, string username);
        Task<Result> UpdateStatus(string billId, int status, string username);
        Task<Result<PageList<BillGetListDTO>>> GetPagedList(
            int pageIndex, int pageSize,
            string username, BillFilter statuses);
    }
}