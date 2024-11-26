using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BranchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PageList<BranchGetListDTO>>> GetPagedList(int pageIndex, int pageSize)
        {
            var branchs = await _unitOfWork.BranchRepository.GetAll(pageSize, pageIndex);
            if (branchs is null)
            {
                return BranchError.NotFound;
            }

            IEnumerable<BranchGetListDTO?> pageData = [];
            if (branchs.PageData is not null)
            {
                foreach (var branch in branchs.PageData)
                {
                    if (branch is null)
                    {
                        return Error.NullVal;
                    }

                    _unitOfWork.BranchRepository.ExplicitLoadCollection(branch, b => b.Users);
                }
            }

            pageData = branchs.PageData?.Select(x =>
                {
                    var data = (x?.ProjectToEntity<Branch, BranchGetListDTO>()) ?? throw new Exception("Converting failed");
                    data.UserList = x?.Users
                                .Select(a => a.ProjectToEntity<User, UserInBranchResponsesDTO>())
                                .ToList() ?? [];
                    return data;
                }) ?? [];

            return new PageList<BranchGetListDTO>
            {
                PageData = pageData,
                PageIndex = branchs.PageIndex,
                PageSize = branchs.PageSize,
                TotalRow = branchs.TotalRow,
            };
        }

        public async Task<Result> AddBranch(BranchCreateDTO Branch)
        {
            // check if Branch's Name exists
            var BranchExists = await _unitOfWork.BranchRepository
                .GetByCondition(u => u.Name == Branch.Name, ignoreFilter: true);
            if (BranchExists != null)
            {
                BranchExists = Branch.ProjectToEntity(BranchExists);
                BranchExists.IsDeleted = false;
                _unitOfWork.BranchRepository.Update(BranchExists);
            }
            else
            {
                var convertedItem = Branch.ProjectToEntity<BranchCreateDTO, Branch>();
                if (convertedItem is null)
                {
                    return Error.ConvertedError;
                }

                convertedItem.CreateAt = DateTime.UtcNow;

                await _unitOfWork.BranchRepository.Add(convertedItem);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result> UpdateBranch(BranchCreateDTO Branch)
        {
            // check if Branch's Name exists
            var BranchExists = await _unitOfWork.BranchRepository
                .GetByCondition(u => u.Code == Branch.Code, true);
            if (BranchExists == null)
            {
                return BranchError.NotFound;
            }

            BranchExists = Branch.ProjectToEntity(BranchExists);

            _unitOfWork.BranchRepository.Update(BranchExists);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result> RemoveBranch(int id)
        {
            // check if Branch's Name exists
            var BranchExists = await _unitOfWork.BranchRepository.GetByCondition(u => u.Code == id);
            if (BranchExists is null)
            {
                return BranchError.NotFound;
            }

            _unitOfWork.BranchRepository.Remove(BranchExists);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
