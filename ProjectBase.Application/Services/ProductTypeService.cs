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
    public class ProductTypeService : IProductTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PageList<ProductTypeGetListDTOs>>> GetPagedList(int pageIndex, int pageSize)
        {
            var ProductTypes = await _unitOfWork.ProductTypeRepository.GetAll(pageIndex, pageSize);
            if (ProductTypes is null)
            {
                return ProductTypeError.NotFound;
            }

            IEnumerable<ProductTypeGetListDTOs?> pageData = [];
            if (ProductTypes.PageData is not null)
            {
                pageData = ProductTypes.PageData.Select(
                    x => x?.ProjectToEntity<ProductType, ProductTypeGetListDTOs>()) ?? [];
            }

            return new PageList<ProductTypeGetListDTOs>
            {
                PageData = pageData,
                PageIndex = ProductTypes.PageIndex,
                PageSize = ProductTypes.PageSize,
                TotalRow = ProductTypes.TotalRow,
            };
        }

        public async Task<Result> AddProductType(ProductTypeCreateDTO ProductType)
        {
            // check if ProductType's Name exists
            var ProductTypeExists = await _unitOfWork.ProductTypeRepository
                .GetByCondition(u => u.Name == ProductType.Name, ignoreFilter: true);
            if (ProductTypeExists != null)
            {
                ProductTypeExists = ProductType.ProjectToEntity(ProductTypeExists);
                ProductTypeExists.IsDeleted = false;
                _unitOfWork.ProductTypeRepository.Update(ProductTypeExists);
            }
            else
            {
                var convertedItem = ProductType.ProjectToEntity<ProductTypeCreateDTO, ProductType>();
                if (convertedItem is null)
                {
                    return Error.ConvertedError;
                }

                convertedItem.Code = 0;

                await _unitOfWork.ProductTypeRepository.Add(convertedItem);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result> UpdateProductType(ProductTypeUpdateDTO ProductType)
        {
            // check if ProductType's Name exists
            var ProductTypeExists = await _unitOfWork.ProductTypeRepository
                                                     .GetByCondition(u => u.Code == ProductType.Code,
                                                     true);
            if (ProductTypeExists == null)
            {
                return ProductTypeError.NotFound;
            }

            ProductTypeExists = ProductType.ProjectToEntity(ProductTypeExists);

            _unitOfWork.ProductTypeRepository.Update(ProductTypeExists);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result> RemoveProductType(int id)
        {
            // check if ProductType's Name exists
            var ProductTypeExists = await _unitOfWork.ProductTypeRepository
                                                     .GetByCondition(u => u.Code == id);
            if (ProductTypeExists == null)
            {
                return ProductTypeError.NotFound;
            }

            _unitOfWork.ProductTypeRepository.Remove(ProductTypeExists);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
