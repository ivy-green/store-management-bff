using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IProductService
    {
        Task<Result> AddProduct(ProductCreateDTO Product, string username);
        Task<Result<PageList<ProductGetListDTOs>>> Search(
            int pageIndex,
            int pageSize,
            string? username = null,
            string searchString = "", int category = 0);
        Task<Result<PageList<ProductGetListDTOs>>> SearchV2(
            int pageIndex, int pageSize, string? username = null, string searchString = "", int categoryCode = 0);
        Task<Result<ProductGetListDTOs>> GetProductDTOByCode(int code);
        Task<Result> RemoveProduct(int code, string username);
        Task<Result> UpdateProduct(ProductCreateDTO Product, string username);
        Task<Result> UpdateOnSaleState(int productCode, string username);
        Task UpdateDataInCache();
    }
}