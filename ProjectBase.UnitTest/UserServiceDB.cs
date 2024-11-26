using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProjectBase.Application.Factories;
using ProjectBase.Application.Services;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Insfracstructure.Data;
using ProjectBase.Insfracstructure.Services.Azure.Blob;
using ProjectBase.Insfracstructure.Services.FileService;
using ProjectBase.Insfracstructure.Services.Mail;
using ProjectBase.Insfracstructure.Services.Message.SNS;

namespace ProjectBase.UnitTest
{
    public class UserServiceTestDB
    {
        private ServiceProvider _serviceProvider;
        private Mock<IHashService> _mockHashService;
        private Mock<IFileService> _mockFileService;
        private Mock<IBlobService> _mockIBlobService;
        private Mock<ISnsMessage> _mockSnsService;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IServiceProvider> _mockServiceProvider;
        UserFactory userFactory;

        UserService? UserService;
        IUnitOfWork? unitOfWork;

        UserCreateDTO dataCreate;
        UserCreateDTO dataUpdate;
        private AppSettingConfiguration setting;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            _mockSnsService = new Mock<ISnsMessage>();
            _mockIBlobService = new Mock<IBlobService>();
            _mockFileService = new Mock<IFileService>();
            _mockHashService = new Mock<IHashService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            // Add DbContext with in-memory database
            services.AddDbContext<AppDBContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            setting = new AppSettingConfiguration
            {
                AWSSection = new AWSSection
                {
                    BillTopic = "BillTopic",
                    LocalstackUrl = "http://localstack:4566"
                },
            };

            // Add UnitOfWork and repositories
            services.AddServices();
            services.AddAWSService(setting);

            _serviceProvider = services.BuildServiceProvider();

            #region ensure create database

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #endregion

            #region arrange data

            dataCreate = new UserCreateDTO
            {
                Username = "test",
                Password = "123456",
                Fullname = "test",
                Bio = "test",
                PhoneNumber = "0123456798",
                Email = "test@gmail.com",
                RoleCode = 1
            };

            dataUpdate = new UserCreateDTO
            {
                Username = "test",
                Fullname = "test update",
                RoleCode = 1
            };

            #endregion
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        #region seeding data
        public void UserSeeding()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.Users.Add(
                    new User
                    {
                        Username = "test",
                    });

                context.SaveChangesAsync();
            }
        }

        public void UserRoleSeeding()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.UserRoles.Add(
                    new UserRole
                    {
                        RoleCode = 1,
                        Username = "test",
                    });

                context.SaveChangesAsync();
            }
        }

        public void RoleSeeding()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.Roles.Add(
                    new Role
                    {
                        Code = 1,
                        RoleName = "Admin",
                    });

                context.SaveChangesAsync();
            }
        }

        public void DataClear()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.Database.EnsureDeleted();
            }
        }
        #endregion

        #region add
        [Test]
        public async Task AddUser_Should_Add_User()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            _mockHashService.Setup(x => x.CreatePasswordHashAndSalt(It.IsAny<string>()))
                            .Returns(("123", "123"));

            // Act
            await UserService.AddUser(dataCreate);

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNotNull(User);
        }

        [Test]
        public async Task AddUser_Should_UserExists()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            _mockHashService.Setup(x => x.CreatePasswordHashAndSalt(It.IsAny<string>()))
                            .Returns(("123", "123"));

            UserSeeding();

            // Act
            Assert.ThrowsAsync<UsernameExistsException>(async () =>
            {
                await UserService.AddUser(dataCreate);
            });

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNotNull(User);
        }

        [Test]
        public async Task AddUser_Should_Username_Null()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            dataCreate.Username = null;

            // Act
            Assert.ThrowsAsync<NullException>(async () =>
            {
                await UserService.AddUser(dataCreate);
            });

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNull(User);
        }
        #endregion

        #region update

        [Test]
        public async Task UpdateUser_Should_Update_User()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            UserSeeding();

            // Act
            await UserService.UpdateUser(dataUpdate);

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNotNull(User);
            Assert.That(User.Username, Is.EqualTo(dataUpdate.Username));
        }


        [Test]
        public async Task UpdateUser_Should_UserNotFound()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            _mockHashService.Setup(x => x.CreatePasswordHashAndSalt(It.IsAny<string>()))
                            .Returns(("123", "123"));

            // Act
            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
            {
                await UserService.UpdateUser(dataUpdate);
            });

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNull(User);
        }

        #endregion

        #region remove

        [Test]
        public async Task RemoveUser_Should_Remove_User()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            UserSeeding();

            // Act
            await UserService.RemoveUser(dataCreate.Username);

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNull(User);
        }

        [Test]
        public async Task RemoveUser_Should_UserNotFound()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            // Act
            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
            {
                await UserService.RemoveUser(dataCreate.Username);
            });

            // Assert
            var User = await unitOfWork.UserRepository.GetByCondition(
                item => item.Username == dataCreate.Username);
            Assert.IsNull(User);
        }

        #endregion

        #region getPagedList

        [Test]
        public async Task GetPagedList_Should_Get_PageList()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            userFactory = new AdminFactory(unitOfWork);

            UserService = new UserService(
                unitOfWork,
                _mockFileService.Object,
                setting,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object, _mockServiceProvider.Object);

            UserSeeding();

            // Act
            var res = await UserService.GetPagedList(0, 10, "test");

            // Assert
            Assert.IsNotNull(res);
            var User = await unitOfWork.UserRepository.GetAll(0, 10);
            Assert.IsNotNull(User);
            Assert.That(User.PageData.Count(), Is.EqualTo(res.Value.PageData.Count()));
        }

        #endregion
    }
}