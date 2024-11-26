using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IProductOnSaleService
    {
        Task<Result<ProductOnSale>> AddProductOnSale(ProductOnSale ProductOnSale);
        Result UpdateProductOnSale(ProductOnSale ProductOnSale);
        Task<Result<ProductOnSale?>> GetById(string productId, string branchId);
    }
}