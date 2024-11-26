using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Extensions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;

namespace ProjectBase.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;
        private readonly IProductOnSaleService _productOnSaleService;
        public ProductService(IUnitOfWork unitOfWork, IUserService userService, IProductOnSaleService productOnSaleService, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _cacheService = cacheService;
            _productOnSaleService = productOnSaleService;
        }

        public async Task<Result<PageList<ProductGetListDTOs>>> SearchV2(
            int pageIndex, int pageSize, string? username = null, string searchString = "", int categoryCode = 0)
        {
            // get user
            User? userRequest = null;
            if (username is not null)
            {
                var getUserWithRoleResponse = await _userService.GetUserWithRole(username);
                if (getUserWithRoleResponse.IsFailure)
                {
                    return getUserWithRoleResponse.Error;
                }
                userRequest = getUserWithRoleResponse.Value;
            }

            // get all products and save it into cache
            List<Product?>? productList;
            productList = await _cacheService.GetAsync<List<Product?>>("product");

            if (productList == null)
            {
                productList = (await _unitOfWork.ProductRepository.GetAll(pageSize, -1)).PageData.ToList();

                await _cacheService.SetAsync("product", productList);
            }

            // paging it
            //var products = await _unitOfWork.ProductRepository
            //    .GetPagedByCondition(u =>
            //        u.Name.ToLower().Contains(searchString.ToLower() ?? "") && // search by name
            //        (categoryCode != 0 ? u.ProductTypeCode == categoryCode : true), // search by category
            //    pageIndex, pageSize);

            List<Product?> products = productList.Where(u =>
                    u is not null
                    && u.Name.ToLower().Contains(searchString.ToLower() ?? "") // search by name
                    && (categoryCode != 0 ? u.ProductTypeCode == categoryCode : true)).ToList(); // search by category;

            List<ProductGetListDTOs> res = [];
            foreach (var item in products)
            {
                if (item is null)
                {
                    return Error.NullVal;
                }

                _unitOfWork.ProductRepository.ExplicitLoad(item, i => i.ProductType);
                if (item?.ProductType is null)
                {
                    return ProductError.ProductTypeNotFound;
                }

                var convertedItem = item.ProjectToEntity<Product, ProductGetListDTOs>();
                if (convertedItem is null)
                {
                    return Error.ConvertedError;
                }

                res.Add(convertedItem);
                res.Last().Type = new ProductTypeGetListDTOs
                {
                    Code = item.ProductType.Code,
                    Name = item.ProductType.Name,
                    Desc = item.ProductType.Desc,
                };

                // 
                if (userRequest is not null)
                {
                    // find data in ProductOnSale
                    var dataResponse = await _productOnSaleService.GetById(item.Id, userRequest.BranchID!);

                    res.Last().IsOnSale = dataResponse.IsSuccess switch
                    {
                        false => res.Last().IsOnSale = false,
                        true => res.Last().IsOnSale = dataResponse.Value!.IsOnSale
                    };
                }
            }

            return new PageList<ProductGetListDTOs>
            {
                PageData = res.Skip(pageSize * pageIndex).Take(pageSize),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRow = products.Count,
            };
        }

        public async Task<Result<PageList<ProductGetListDTOs>>> Search(
            int pageIndex, int pageSize, string? username = null, string searchString = "", int categoryCode = 0)
        {
            // get user
            User? userRequest = null;
            if (username is not null)
            {
                var getUserWithRoleResponse = await _userService.GetUserWithRole(username);
                if (getUserWithRoleResponse.IsFailure)
                {
                    return Result.Failure<PageList<ProductGetListDTOs>>(getUserWithRoleResponse.Error);
                }
                userRequest = getUserWithRoleResponse.Value;
            }

            var products = await _unitOfWork.ProductRepository
                .GetPagedByCondition(u =>
                    u.Name.ToLower().Contains(searchString.ToLower() ?? "") && // search by name
                    (categoryCode != 0 ? u.ProductTypeCode == categoryCode : true), // search by category
                pageIndex, pageSize);

            if (products is null)
            {
                return Result.Failure<PageList<ProductGetListDTOs>>(Error.NullVal);
            }

            List<ProductGetListDTOs> res = [];
            foreach (var item in products.PageData)
            {
                if (item is null)
                {
                    return Result.Failure<PageList<ProductGetListDTOs>>(Error.NullVal);
                }

                _unitOfWork.ProductRepository.ExplicitLoad(item, i => i.ProductType);
                if (item?.ProductType is null)
                {
                    return Result.Failure<PageList<ProductGetListDTOs>>(ProductError.ProductTypeNotFound);
                }

                var convertedItem = item.ProjectToEntity<Product, ProductGetListDTOs>();
                if (convertedItem is null)
                {
                    return Result.Failure<PageList<ProductGetListDTOs>>(Error.ConvertedError);
                }

                res.Add(convertedItem);
                res.Last().Type = new ProductTypeGetListDTOs
                {
                    Code = item.ProductType.Code,
                    Name = item.ProductType.Name,
                    Desc = item.ProductType.Desc,
                };

                // 
                if (userRequest is not null)
                {
                    // find data in ProductOnSale
                    var dataResponse = await _productOnSaleService.GetById(item.Id, userRequest.BranchID!);
                    if (dataResponse.IsFailure)
                    {
                        return Result.Failure<PageList<ProductGetListDTOs>>(dataResponse.Error);
                    }

                    res.Last().IsOnSale = dataResponse.Value switch
                    {
                        null => res.Last().IsOnSale = false,
                        _ => res.Last().IsOnSale = dataResponse.Value.IsOnSale
                    };
                }
            }

            return Result.Success(new PageList<ProductGetListDTOs>
            {
                PageData = res,
                PageIndex = products.PageIndex,
                PageSize = products.PageSize,
                TotalRow = products.TotalRow,
            });
        }

        public async Task<Result<Product?>> GetByCode(int code)
        {
            Product? response = await _unitOfWork.ProductRepository.GetByCondition(x => x.Code == code);
            return response;
        }

        public async Task<Result<ProductGetListDTOs>> GetProductDTOByCode(int code)
        {
            var response = await GetByCode(code);
            if (response.IsSuccess && response.Value != null)
            {
                var convertedResponse = response.Value.ProjectToEntity<Product, ProductGetListDTOs>();
                if (convertedResponse is null)
                {
                    return Error.ConvertedError;
                }
                return convertedResponse;
            }
            return Error.NullVal;
        }

        public async Task<Result> AddProduct(ProductCreateDTO Product, string username)
        {
            // check valid data
            var validResponse = CheckValidDataInput(Product);
            if (validResponse.IsFailure)
            {
                return validResponse;
            }

            // check if Username valid
            var userExists = await _unitOfWork.UserRepository.GetByCondition(x => x.Username == username);
            if (userExists == null)
            {
                return Result.Failure<PageList<ProductGetListDTOs>>(UserError.UsernameNotFound);
            }

            // check valid ProductType
            var typeExists = await _unitOfWork.ProductTypeRepository.GetByCondition(x => x.Code == Product.ProductTypeCode);
            if (typeExists == null)
            {
                return Result.Failure<PageList<ProductGetListDTOs>>(ProductError.ProductTypeNotFound);
            }

            //var branchExists = await _unitOfWork.BranchRepository.GetByCondition(x => x.Code == Product.BranchCode);
            //if (branchExists is null)
            //{
            //    return Result.Failure<PageList<ProductGetListDTOs>>(ProductError.BranchNotFound);
            //}

            // check if Product's Name exists
            var ProductExists = await IsDataExistsInCache(x => x.Name == Product.Name);
            if (ProductExists != null)
            {
                //ProductExists.BranchId = userExists.BranchID ?? branchExists.Id ?? throw new Exception("Branch Id is missing");
                ProductExists = Product.ProjectToEntity<ProductCreateDTO, Product>(ProductExists);
                ProductExists.IsDeleted = false;
                ProductExists.CreatorUsername = username;
                _unitOfWork.ProductRepository.Update(ProductExists);
            }
            else
            {
                var convertedItem = Product.ProjectToEntity<ProductCreateDTO, Product>();
                if (convertedItem is null)
                {
                    return Result.Failure<PageList<ProductGetListDTOs>>(Error.NullVal);
                }

                Product res = convertedItem;
                res.Id = Guid.NewGuid().ToString();
                //res.BranchId = userExists.BranchID ?? branchExists.Id ?? throw new Exception("Branch Id is missing");
                res.CreatorUsername = username;
                res.CreateAt = DateTime.UtcNow;
                await _unitOfWork.ProductRepository.Add(res);

                // update cache
                await UpdateDataInCache("product", res);
            }

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateProduct(ProductCreateDTO data, string username)
        {
            var validResponse = CheckValidDataInput(data);
            if (validResponse.IsFailure)
            {
                return validResponse;
            }

            var ProductExists = await IsDataExistsInCache(x => x.Code == data.Code);
            if (ProductExists == null)
            {
                return ProductError.ProductNotFound;
            }

            var userExists = await _unitOfWork.UserRepository.GetByCondition(x => x.Username == username);
            if (userExists == null)
            {
                return UserError.UsernameNotFound;
            }

            var typeExists = await _unitOfWork.ProductTypeRepository.GetByCondition(
                x => x.Code == data.ProductTypeCode);
            if (typeExists == null)
            {
                return ProductError.ProductTypeNotFound;
            }

            ProductExists = data.ProjectToEntity<ProductCreateDTO, Product>(ProductExists);
            await _unitOfWork.SaveChangesAsync();

            // update cache
            await UpdateDataInCache("product", ProductExists, "Update");

            return true;
        }

        public async Task<Result> RemoveProduct(int code, string username)
        {
            var ProductExists = await IsDataExistsInCache(x => x.Code == code);
            if (ProductExists == null)
            {
                return ProductError.ProductNotFound;
            }

            // check user branch
            User? user = await _unitOfWork.UserRepository.GetByCondition(u => u.Username == username);
            if (user is null)
            {
                return UserError.UserNotFound;
            }

            //if (!_unitOfWork.UserRepository.IsUserRole(user, ["Admin"]))
            //{
            //    if (user.BranchID != ProductExists.BranchId)
            //    {
            //        throw new UnauthorizedAccessException("Manager cannot remove Product that not belong to other Branch");
            //    }
            //}

            _unitOfWork.ProductRepository.Remove(ProductExists);
            await _unitOfWork.SaveChangesAsync();

            await UpdateDataInCache("product", ProductExists, "Remove");

            return true;
        }

        public static Result CheckValidDataInput(ProductCreateDTO data)
        {
            return Result.Success(data)
                .Ensure(x => x.Quantity > 0 && x.Price > 0, ProductError.NegativeData)
                .Ensure(x => x.Price > 1000, ProductError.PriceLessThanThousand)
                .Ensure(x => x.Name is not null, ProductError.NameNotFound);
        }

        public async Task<Result> UpdateOnSaleState(int productCode, string username)
        {
            // Find user that change state
            var getUserWithRoleResponse = await _userService.GetUserWithRole(username);
            if (getUserWithRoleResponse.IsFailure)
            {
                return getUserWithRoleResponse;
            }
            User user = getUserWithRoleResponse.Value;

            if (user.BranchID is null)
            {
                throw new UnauthorizedAccessException();
            }

            // Check if data exists in ProductOnSale table
            var getProductByCodeResponse = await GetByCode(productCode);
            if (getProductByCodeResponse.Value is null)
            {
                return ProductError.ProductNotFound;
            }
            Product productExists = getProductByCodeResponse.Value;

            var getDataResponse = await _productOnSaleService.GetById(productExists.Id, user.BranchID);
            if (getDataResponse.IsFailure)
            {
                return getDataResponse;
            }

            ProductOnSale? dataExists = getDataResponse.Value;
            var dataCreate = new ProductOnSale
            {
                ProductId = productExists.Id,
                BranchId = user.BranchID,
                IsOnSale = true,
                UpdatedAt = DateTime.UtcNow,
            };

            if (dataExists is null)
            {
                // add new if it null
                await _unitOfWork.ProductOnSaleRepository.Add(dataCreate);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                dataCreate.IsOnSale = !dataExists!.IsOnSale;
            }

            _unitOfWork.ProductOnSaleRepository.Update(dataCreate);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task UpdateDataInCache(string key, Product value, string action = "Add")
        {
            List<Product>? cacheValue = await _cacheService.GetAsync<List<Product>>(key);
            if (cacheValue is null)
            {
                await _cacheService.SetAsync(key, cacheValue);
            }
            else
            {
                switch (action)
                {
                    case "Add":
                        cacheValue.Add(value);
                        break;
                    case "Update":
                        cacheValue = cacheValue.Select(x =>
                        {
                            return x.Id == value.Id ? value : x;
                        }).ToList();
                        break;
                    case "Remove":
                        cacheValue = cacheValue.Where(x => x.Id != value.Id).ToList();
                        break;
                    default:
                        break;
                }

                await _cacheService.SetAsync(key, cacheValue);
            }
        }

        private async Task<Product?> IsDataExistsInCache(Func<Product, bool> predicate)
        {
            List<Product>? cacheValue = await _cacheService.GetAsync<List<Product>>("product");
            if (cacheValue is null)
            {
                return null;
            }

            return cacheValue.FirstOrDefault(predicate);
        }

        public async Task UpdateDataInCache()
        {
            var responses = await _unitOfWork.ProductRepository.GetAll(0, -1);
            if (responses is null || responses.PageData is null)
            {
                return;
            }
            await _cacheService.UpdateNewDataInCache<List<Product>>("product", responses.PageData.ToList());
        }
    }
}
