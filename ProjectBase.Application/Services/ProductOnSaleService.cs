using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;

namespace ProjectBase.Application.Services
{
    public class ProductOnSaleService : IProductOnSaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductOnSaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<ProductOnSale>> AddProductOnSale(ProductOnSale data)
        {
            await _unitOfWork.ProductOnSaleRepository.Add(data);
            return data;
        }

        public async Task<Result<ProductOnSale?>> GetById(string productId, string branchId)
        {
            ProductOnSale? data = await _unitOfWork.ProductOnSaleRepository
                .GetByCondition(x => x.ProductId == productId && x.BranchId == branchId);
            return data;
        }

        public Result UpdateProductOnSale(ProductOnSale data)
        {
            _unitOfWork.ProductOnSaleRepository.Update(data);
            return true;
        }
    }
}
