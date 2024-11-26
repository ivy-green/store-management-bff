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
    public class ProductTypeServiceTest
    {
        private IProductTypeService _ProductTypeService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IProductTypeRepository> _mockProductTypeRepository;
        Product type;
        Product typeNull;
        User user;
        User userNull;
        ProductType ProductTypeNull;
        ProductType ProductType;
        PageList<ProductType> ProductTypes;
        PageList<ProductType> ProductTypesNull;
        ProductTypeCreateDTO dataCreate;
        ProductTypeUpdateDTO dataUpdate;

        [SetUp]
        public void Setup()
        {
            ProductTypeNull = null;
            userNull = null;
            typeNull = null;
            ProductTypesNull = null;

            dataCreate = new ProductTypeCreateDTO
            {
                Name = "test",
            };

            dataUpdate = new ProductTypeUpdateDTO
            {
                Name = "test",
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

            ProductType = new ProductType
            {
                Code = 1,
                Name = "test"
            };

            ProductTypes = new PageList<ProductType>()
            {
                PageIndex = 0,
                PageSize = 10,
                PageData = [ProductType]
            };


            _mockProductTypeRepository = new Mock<IProductTypeRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork.SetupGet(x => x.ProductTypeRepository).Returns(_mockProductTypeRepository.Object);
            _ProductTypeService = new ProductTypeService(_unitOfWork.Object);
        }

        #region get list
        [Test]
        public async Task GetList_Valid()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(ProductTypes);

            // act
            var res = await _ProductTypeService.GetPagedList(It.IsAny<int>(), It.IsAny<int>());

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
            Assert.NotNull(res);
        }


        [Test]
        public void GetList_Invalid()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(ProductTypesNull);

            // act
            Assert.ThrowsAsync<ProductTypeNotFoundException>(async () =>
            {
                await _ProductTypeService.GetPagedList(It.IsAny<int>(), It.IsAny<int>());
            });

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
        }
        #endregion

        #region create ProductType
        [Test]
        public async Task AddProductType_Valid()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(ProductTypeNull);


            // act
            await _ProductTypeService.AddProductType(dataCreate);

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.Add(It.IsAny<ProductType>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void AddProductType_Failed_ProductTypenameExists()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(ProductType);

            // act
            Assert.ThrowsAsync<ProductTypeExistsException>(async () =>
            {
                await _ProductTypeService.AddProductType(dataCreate);
            });

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.Add(It.IsAny<ProductType>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public void AddProductType_Failed_ProductTypeExists()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(ProductType);

            // act
            Assert.ThrowsAsync<ProductTypeExistsException>(async () =>
            {
                await _ProductTypeService.AddProductType(dataCreate);
            });

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.Add(It.IsAny<ProductType>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion

        #region update ProductType
        [Test]
        public async Task UpdateProductType_Valid()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), true, false))
                .ReturnsAsync(ProductType);

            // act
            await _ProductTypeService.UpdateProductType(dataUpdate);

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                            It.IsAny<Expression<Func<ProductType, bool>>>(), true, false), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void UpdateProductType_Failed_ProductTypeNotFound()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), true, false))
                .ReturnsAsync(ProductTypeNull);

            // act
            Assert.ThrowsAsync<ProductTypeNotFoundException>(async () =>
            {
                await _ProductTypeService.UpdateProductType(dataUpdate);
            });

            // assert
            _unitOfWork.Verify(u =>
            u.ProductTypeRepository.GetByCondition(
                            It.IsAny<Expression<Func<ProductType, bool>>>(), true, false),
                            Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion

        #region remove ProductType
        [Test]
        public async Task RemoveProductType_Valid()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(ProductType);

            // act
            await _ProductTypeService.RemoveProductType(1);

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                            It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductTypeRepository.Remove(It.IsAny<ProductType>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void RemoveProductType_Failed_ProductTypeNotFound()
        {
            // arrange
            _mockProductTypeRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<ProductType, bool>>>(), false, false))
                .ReturnsAsync(ProductTypeNull);

            // act
            Assert.ThrowsAsync<ProductTypeNotFoundException>(async () =>
            {
                await _ProductTypeService.RemoveProductType(1);
            });

            // assert
            _unitOfWork.Verify(u => u.ProductTypeRepository.GetByCondition(
                            It.IsAny<Expression<Func<ProductType, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.ProductTypeRepository.Remove(It.IsAny<ProductType>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion

    }
}