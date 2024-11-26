using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IProductTypeService
    {
        Task<Result> AddProductType(ProductTypeCreateDTO ProductType);
        Task<Result<PageList<ProductTypeGetListDTOs>>> GetPagedList(int pageIndex, int pageSize);
        Task<Result> RemoveProductType(int id);
        Task<Result> UpdateProductType(ProductTypeUpdateDTO ProductType);
    }
}