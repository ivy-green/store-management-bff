using Moq;
using ProjectBase.Application.Services;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;
using System.Linq.Expressions;

namespace ProjectBase.UnitTest
{
    public class ProductServiceTest
    {
        private IProductService _productService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IProductOnSaleService> _mockProductOnSaleService;
        private Mock<IProductTypeRepository> _mockProductTypeRepository;
        private Mock<IUserService> _mockUserService;
        private Mock<ICacheService> _mockCacheService;
        ProductType type;
        ProductType? typeNull;
        User user;
        User? userNull;
        Product? productNull;
        Product product;
        PageList<Product> products;
        PageList<Product> productsNull;
        ProductCreateDTO dataCreate;

        [SetUp]
        public void Setup()
        {
            productNull = null;
            userNull = null;
            typeNull = null;
            productsNull = null;

            dataCreate = new ProductCreateDTO
            {
                Name = "test",
                Quantity = 1,
                Price = 12000,
                ProductTypeCode = 1,
            };

            type = new ProductType
            {
                Name = "test"
            };

            user = new User
            {
                Username = "test",
                PasswordHash = "test",
                PasswordSalt = "test",
                Email = "test",
                IsAccountBlocked = false,
                IsEmailConfirmed = false,
            };

            product = new Product
            {
                Name = "test",
                Price = 12000,
                Quantity = 12,
                ProductTypeCode = 1,
                ProductType = new ProductType
                {
                    Name = "test"
                },
            };

            products = new PageList<Product>()
            {
                PageIndex = 0,
                PageSize = 10,
                PageData = [product]
            };

            _mockUserRepository = new Mock<IUserRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockProductTypeRepository = new Mock<IProductTypeRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockProductOnSaleService = new Mock<IProductOnSaleService>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork.SetupGet(x => x.ProductRepository).Returns(_mockProductRepository.Object);
            _unitOfWork.SetupGet(x => x.ProductTypeRepository).Returns(_mockProductTypeRepository.Object);
            _unitOfWork.SetupGet(x => x.UserRepository).Returns(_mockUserRepository.Object);
            _productService = new ProductService(_unitOfWork.Object, _mockUserService.Object, _mockProductOnSaleService.Object, _mockCacheService.Object);
        }

        #region get list
        [Test]
        public async Task GetList_Valid()
        {
            // arrange
            _mockProductRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(products);

            // act
            var res = await _productService.Search(It.IsAny<int>(), It.IsAny<int>());

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
            Assert.NotNull(res);
        }


        [Test]
        public void GetList_Invalid()
        {
            // arrange
            _mockProductRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(productsNull);

            // act
            Assert.ThrowsAsync<NullException>(async () =>
            {
                await _productService.Search(It.IsAny<int>(), It.IsAny<int>());
            });

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
        }
        #endregion

        #region valid data
        [Test]
        public void CheckValidData_Invalid_QuantityNegative()
        {
            // arrange
            var data = new ProductCreateDTO
            {
                Name = "test",
                Quantity = -1,
                Price = 1,
                ProductTypeCode = 1,
            };

            // act
            Assert.ThrowsAsync<NegativeDataException>(async () =>
            {
                await _productService.AddProduct(data, "");
            });

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public void CheckValidData_Invalid_PriceNegative()
        {
            // arrange
            var data = new ProductCreateDTO
            {
                Name = "test",
                Quantity = 1,
                Price = -1,
                ProductTypeCode = 1,
            };

            // act
            Assert.ThrowsAsync<NegativeDataException>(async () =>
            {
                await _productService.AddProduct(data, "");
            });

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public void CheckValidData_Invalid_PriceLessThan1000()
        {
            // arrange
            var data = new ProductCreateDTO
            {
                Name = "test",
                Quantity = 1,
                Price = 900,
                ProductTypeCode = 1,
            };

            // act
            Assert.ThrowsAsync<PriceLessThan1000Exception>(async () =>
            {
                await _productService.AddProduct(data, "");
            });

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion

        #region create product
        [Test]
        public async Task AddProduct_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(user);

            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(type);

            _mockProductRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Product, bool>>>(), false, false))
                .ReturnsAsync(productNull);


            // act
            await _productService.AddProduct(dataCreate, "");

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Once);
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void AddProduct_Failed_ProductnameExists()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(user);

            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(type);

            _mockProductRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Product, bool>>>(), false, false))
                .ReturnsAsync(product);


            // act
            Assert.ThrowsAsync<ProductExistsException>(async () =>
            {
                await _productService.AddProduct(dataCreate, "");
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public void AddProduct_Failed_UserExists()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(userNull);

            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(type);

            _mockProductRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Product, bool>>>(), false, false))
                .ReturnsAsync(productNull);


            // act
            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
            {
                await _productService.AddProduct(dataCreate, "");
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Never);
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public void AddProduct_Failed_ProductTypeExists()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(user);

            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(typeNull);

            _mockProductRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Product, bool>>>(), false, false))
                .ReturnsAsync(productNull);


            // act
            Assert.ThrowsAsync<ProductTypeNotFoundException>(async () =>
            {
                await _productService.AddProduct(dataCreate, "");
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductRepository.Add(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion

        #region remove product
        [Test]
        public async Task RemoveProduct_Valid()
        {
            // arrange
            _mockProductRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Product, bool>>>(), false, false))
                .ReturnsAsync(product);

            // act
            await _productService.RemoveProduct(1, "");

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.GetByCondition(
                            It.IsAny<Expression<Func<Product, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductRepository.Remove(It.IsAny<Product>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void RemoveProduct_Failed_ProductNotFound()
        {
            // arrange
            _mockProductRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Product, bool>>>(), false, false))
                .ReturnsAsync(productNull);

            // act
            Assert.ThrowsAsync<ProductNotFoundException>(async () =>
            {
                await _productService.RemoveProduct(1, "");
            });

            // assert
            _unitOfWork.Verify(u => u.ProductRepository.GetByCondition(
                            It.IsAny<Expression<Func<Product, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductRepository.Remove(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion

    }
}