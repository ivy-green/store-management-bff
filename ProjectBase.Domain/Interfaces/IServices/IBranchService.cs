using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IBranchService
    {
        Task<Result> AddBranch(BranchCreateDTO Branch);
        Task<Result<PageList<BranchGetListDTO>>> GetPagedList(int pageIndex, int pageSize);
        Task<Result> RemoveBranch(int id);
        Task<Result> UpdateBranch(BranchCreateDTO Branch);
    }
}