//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using ProjectBase.Application.Services;
//using ProjectBase.Domain.Configuration;
//using ProjectBase.Domain.DTOs.Requests;
//using ProjectBase.Domain.Entities;
//using ProjectBase.Domain.Exceptions;
//using ProjectBase.Domain.Interfaces;
//using ProjectBase.Insfracstructure.Data;

//namespace ProjectBase.UnitTest
//{
//    public class ProductServiceTestDB
//    {
//        private ServiceProvider _serviceProvider;

//        ProductService ProductService;
//        IUnitOfWork unitOfWork;

//        ProductCreateDTO dataCreate;
//        ProductCreateDTO dataUpdate;
//        private AppSettingConfiguration setting;

//        [SetUp]
//        public void Setup()
//        {
//            var services = new ServiceCollection();

//            // Add DbContext with in-memory database
//            services.AddDbContext<AppDBContext>(options =>
//                options.UseInMemoryDatabase("TestDatabase"));

//            setting = new AppSettingConfiguration
//            {
//                AWSSection = new AWSSection
//                {
//                    BillTopic = "BillTopic",
//                    LocalstackUrl = "http://localstack:4566"
//                },
//            };

//            // Add UnitOfWork and repositories
//            services.AddServices();
//            services.AddAWSService(setting);

//            _serviceProvider = services.BuildServiceProvider();

//            #region ensure create database

//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
//                context.Database.EnsureDeleted();
//                context.Database.EnsureCreated();
//            }

//            #endregion

//            #region arrange data

//            dataCreate = new ProductCreateDTO
//            {
//                // Id = "testtt",
//                Code = 1,
//                Name = "test",
//                Quantity = 1,
//                Price = 12000,
//                ProductTypeCode = 1,
//            };

//            dataUpdate = new ProductCreateDTO
//            {
//                // Id = "testtt",
//                Code = 1,
//                Name = "testUpdate",
//                Quantity = 1,
//                Price = 12000,
//                ProductTypeCode = 1,
//            };

//            #endregion
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _serviceProvider.Dispose();
//        }

//        #region seeding data

//        public void UserSeeding()
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
//                context.Users.Add(
//                    new User
//                    {
//                        Username = "test",
//                    });

//                context.SaveChangesAsync();
//            }
//        }
//        public void ProductTypeSeeding()
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
//                context.ProductTypes.Add(
//                    new ProductType
//                    {
//                        Code = 1,
//                        Name = "test",
//                    });

//                context.SaveChangesAsync();
//            }
//        }

//        public void ProductSeeding()
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
//                context.Products.Add(
//                    new Product
//                    {
//                        Id = "testtt",
//                        Code = 1,
//                        Name = "test",
//                        Quantity = 1,
//                        Price = 12000,
//                        ProductTypeCode = 1,
//                        CreatorUsername = "test",
//                    });

//                context.SaveChangesAsync();
//            }
//        }

//        public void DataClear()
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
//                context.Database.EnsureDeleted();
//            }
//        }

//        #endregion

//        #region add

//        [Test]
//        public async Task AddProduct_Should_Add_Product()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();
//            ProductTypeSeeding();

//            // Act
//            await ProductService.AddProduct(dataCreate, "");

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Name == dataCreate.Name);
//            Assert.IsNotNull(Product);
//        }

//        [Test]
//        public async Task AddProduct_Should_ProductTypeNotFound()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();

//            // Act
//            Assert.ThrowsAsync<ProductTypeNotFoundException>(async () =>
//            {
//                await ProductService.AddProduct(dataCreate, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Name == dataCreate.Name);
//            Assert.IsNull(Product);
//        }

//        [Test]
//        public async Task AddProduct_Should_UserNotFound()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            // Act
//            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
//            {
//                await ProductService.AddProduct(dataCreate, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Name == dataCreate.Name);
//            Assert.IsNull(Product);
//        }

//        [Test]
//        public async Task AddProduct_Should_ProductExists()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();
//            ProductTypeSeeding();
//            ProductSeeding();

//            // Act
//            Assert.ThrowsAsync<ProductExistsException>(async () =>
//            {
//                await ProductService.AddProduct(dataCreate, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Name == dataCreate.Name);
//            Assert.IsNotNull(Product);
//        }

//        #endregion

//        #region update

//        [Test]
//        public async Task UpdateProduct_Should_Update_Product()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();
//            ProductTypeSeeding();
//            ProductSeeding();

//            // Act
//            await ProductService.UpdateProduct(dataUpdate, "");

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Code == dataCreate.Code);
//            Assert.IsNotNull(Product);
//            Assert.That(Product.Name, Is.EqualTo(dataUpdate.Name));
//        }

//        [Test]
//        public async Task UpdateProduct_Should_ProductTypeNotFound()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();
//            ProductSeeding();

//            // Act
//            Assert.ThrowsAsync<ProductTypeNotFoundException>(async () =>
//            {
//                await ProductService.UpdateProduct(dataUpdate, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Code == dataCreate.Code);
//            Assert.IsNotNull(Product);
//            Assert.That(Product.Name, Is.EqualTo(dataCreate.Name));
//        }

//        [Test]
//        public async Task UpdateProduct_Should_UserNotFound()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            ProductSeeding();

//            // Act
//            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
//            {
//                await ProductService.UpdateProduct(dataUpdate, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Code == dataCreate.Code);
//            Assert.IsNotNull(Product);
//            Assert.That(Product.Name, Is.EqualTo(dataCreate.Name));
//        }

//        [Test]
//        public async Task UpdateProduct_Should_ProductExists()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();
//            ProductTypeSeeding();

//            // Act
//            Assert.ThrowsAsync<ProductNotFoundException>(async () =>
//            {
//                await ProductService.UpdateProduct(dataUpdate, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Code == dataCreate.Code);
//            Assert.IsNull(Product);
//        }

//        #endregion

//        #region remove

//        [Test]
//        public async Task RemoveProduct_Should_Remove_Product()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            ProductSeeding();

//            // Act
//            await ProductService.RemoveProduct(dataCreate.Code, "");

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Code == dataCreate.Code);
//            Assert.IsNull(Product);
//        }

//        [Test]
//        public async Task RemoveProduct_Should_ProductNotFound()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            // Act
//            Assert.ThrowsAsync<ProductNotFoundException>(async () =>
//            {
//                await ProductService.RemoveProduct(dataCreate.Code, "");
//            });

//            // Assert
//            var Product = await unitOfWork.ProductRepository.GetByCondition(
//                item => item.Code == dataCreate.Code);
//            Assert.IsNull(Product);
//        }

//        #endregion

//        #region getPagedList

//        [Test]
//        public async Task GetPagedList_Should_Get_PageList()
//        {
//            // Arrange
//            using var scope = _serviceProvider.CreateScope();
//            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//            ProductService = new ProductService(unitOfWork);

//            UserSeeding();
//            ProductTypeSeeding();
//            ProductSeeding();

//            // Act
//            var res = await ProductService.GetPagedList(0, 10);

//            // Assert
//            Assert.IsNotNull(res);
//            var Product = await unitOfWork.ProductRepository.GetAll(0, 10);
//            Assert.IsNotNull(Product);
//            Assert.That(Product.PageData.Count(), Is.EqualTo(res.Value.PageData.Count()));
//        }

//        #endregion
//    }
//}