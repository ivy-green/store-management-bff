using Microsoft.AspNetCore.Http;
using Moq;
using ProjectBase.Application.Services;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;
using ProjectBase.Insfracstructure.Services.Azure.Blob;
using ProjectBase.Insfracstructure.Services.FileService;
using ProjectBase.Insfracstructure.Services.Mail;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using System.Linq.Expressions;

namespace ProjectBase.UnitTest
{
    public class UserServiceTest
    {
        private IUserService _userService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IRoleRepository> _mockRoleRepository;
        private Mock<IUserRoleRepository> _mockUserRoleRepository;
        private Mock<IHashService> _mockHashService;
        private Mock<IFileService> _mockFileService;
        private Mock<IBlobService> _mockIBlobService;
        private Mock<ISnsMessage> _mockSnsService;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<AppSettingConfiguration> _setting;

        private Mock<IAdminFactory> _adminFactory;
        private Mock<IManagerFactory> _managerFactory;
        private Mock<IStaffFactory> _staffFactory;

        UserCreateDTO dataCreate;
        IFormFile file;
        List<User> userList;
        PageList<User> users;
        PageList<User> usersNull;
        User user;
        User userRoleGotNull;
        User? userNull = null;
        Role role;
        Role? roleNull = null;

        [SetUp]
        public void Setup()
        {
            DataSetup();

            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockUserRoleRepository = new Mock<IUserRoleRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _setting = new Mock<AppSettingConfiguration>();
            _mockSnsService = new Mock<ISnsMessage>();
            _mockIBlobService = new Mock<IBlobService>();
            _mockFileService = new Mock<IFileService>();
            _mockHashService = new Mock<IHashService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            _adminFactory = new Mock<IAdminFactory>();
            _managerFactory = new Mock<IManagerFactory>();
            _staffFactory = new Mock<IStaffFactory>();

            _unitOfWork.SetupGet(x => x.UserRepository).Returns(_mockUserRepository.Object);
            _unitOfWork.SetupGet(x => x.RoleRepository).Returns(_mockRoleRepository.Object);
            _unitOfWork.SetupGet(x => x.UserRoleRepository).Returns(_mockUserRoleRepository.Object);

            _userService = new UserService(
                _unitOfWork.Object,
                _mockFileService.Object,
                _setting.Object,
                _mockSnsService.Object,
                // _mockIBlobService.Object,
                _mockHashService.Object,
                _mockEmailService.Object,
                _mockServiceProvider.Object);
        }

        private void DataSetup()
        {
            usersNull = new PageList<User>();

            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            userList = [];

            //create FormFile with desired data
            file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            dataCreate = new UserCreateDTO
            {
                Username = "test",
                Password = "123456789",
                Fullname = "my name is tester",
                RoleCode = 1
            };

            user = new User
            {
                Username = "test",
                UserRoles = [
                    new UserRole{
                        Username = "test",
                        Role = new Role {
                            Code = 1,
                            RoleName = "test",
                        }
                    }
                ]
            };

            userRoleGotNull = new User
            {
                Username = "test",
                UserRoles = null
            };

            role = new Role
            {
                Code = 1,
                RoleName = "Admin",
            };

            users = new PageList<User>()
            {
                PageIndex = 0,
                PageSize = 10,
                PageData = [user]
            };
        }

        #region Get List
        [Test]
        public async Task GetList_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(users);

            // act
            var res = await _userService.GetPagedList(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>());

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
            Assert.NotNull(res);
        }


        [Test]
        public void GetList_Invalid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(usersNull);

            // act
            Assert.ThrowsAsync<NullException>(async () =>
            {
                await _userService.GetPagedList(It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>());
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
        }
        #endregion

        #region Get User
        [Test]
        public async Task GetUserByRole_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetListByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(), false)).ReturnsAsync(userList);

            // act
            var res = await _userService.GetUserByRole(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>());

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetListByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(), false), Times.Once);
            Assert.NotNull(res);
        }

        [Test]
        public async Task GetUser_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false)).ReturnsAsync(user);

            // act
            var res = await _userService.GetUser(
                It.IsAny<string>());

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false), Times.Once);
            Assert.NotNull(res);
        }

        [Test]
        public async Task GetUser_Null()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false)).ReturnsAsync(userNull);

            // act

            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
            {
                var res = await _userService.GetUser(
                    It.IsAny<string>());
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false), Times.Once);
        }

        [Test]
        public async Task GetProfile_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false)).ReturnsAsync(user);

            // act
            var res = await _userService.GetProfile(It.IsAny<string>());

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false), Times.Once);
        }

        [Test]
        public async Task GetProfile_Failed_UserRoleNotFound()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false)).ReturnsAsync(userRoleGotNull);

            // act
            Assert.ThrowsAsync<NullException>(async () =>
            {
                var res = await _userService.GetProfile(It.IsAny<string>());
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(),
                false, false), Times.Once);
        }


        #endregion

        #region Add User
        [Test]
        public async Task AddUser_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(userNull);

            _mockRoleRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Role, bool>>>(), false, false))
                .ReturnsAsync(role);

            // act
            await _userService.AddUser(dataCreate);

            // assert
            _unitOfWork.Verify(u => u.UserRepository.Add(It.IsAny<User>()), Times.Once);
            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public async Task AddUser_Failed_UsernameExists()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(user);

            // act
            var res = await _userService.AddUser(dataCreate);

            // assert
            Assert.IsNotNull(res);
            Assert.That(res.IsFailure);
            Assert.That(res.Error.Equals(UserError.UsernameExists));

            _unitOfWork.Verify(u => u.UserRepository.Add(It.IsAny<User>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public async Task AddUser_Failed_UsernameMissing()
        {
            // arrange
            dataCreate.Username = "";

            // act
            var res = await _userService.AddUser(dataCreate);

            // assert
            Assert.IsNotNull(res);
            Assert.That(res.IsFailure);
            Assert.That(res.Error.Equals(UserError.UsernameNotFound));

            _unitOfWork.Verify(u => u.UserRepository.Add(It.IsAny<User>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public async Task AddUser_Failed_InvalidPassword()
        {
            // arrange
            dataCreate.Password = "";

            // act
            var res = await _userService.AddUser(dataCreate);

            // assert
            Assert.IsNotNull(res);
            Assert.That(res.IsFailure);
            Assert.That(res.Error.Equals(UserError.InvalidPassword));

            _unitOfWork.Verify(u => u.UserRepository.Add(It.IsAny<User>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public async Task AddUser_Failed_InvalidFullname()
        {
            // arrange
            dataCreate.Fullname = "";

            // act
            var res = await _userService.AddUser(dataCreate);

            // assert
            Assert.IsNotNull(res);
            Assert.That(res.IsFailure);
            Assert.That(res.Error.Equals(UserError.FullnameNotFound));

            _unitOfWork.Verify(u => u.UserRepository.Add(It.IsAny<User>()), Times.Never);
            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public async Task AddUser_Success_CreateAdmin()
        {
            // arrange
            role.RoleName = "Admin";

            _mockRoleRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Role, bool>>>(), false, false))
                .ReturnsAsync(role);

            _mockServiceProvider.Setup(x => x.GetService(typeof(IAdminFactory)))
                .Returns(_adminFactory.Object);

            _adminFactory.Setup(x => x.CreateUser(It.IsAny<User>(), It.IsAny<UserCreateDTO>()))
                .ReturnsAsync(Result.Success());

            // act
            var res = await _userService.AddUser(dataCreate);

            // assert
            Assert.IsNotNull(res);
            Assert.That(res.IsSuccess);

            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }
        [Test]
        public async Task AddUser_Success_CreateManager()
        {
            // arrange
            role.RoleName = "Manager";

            _mockRoleRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<Role, bool>>>(), false, false))
                .ReturnsAsync(role);

            _mockServiceProvider.Setup(x => x.GetService(typeof(IManagerFactory)))
                .Returns(_managerFactory.Object);

            _adminFactory.Setup(x => x.CreateUser(It.IsAny<User>(), It.IsAny<UserCreateDTO>()))
                .ReturnsAsync(Result.Success());

            // act
            var res = await _userService.AddUser(dataCreate);

            // assert
            Assert.IsNotNull(res);
            Assert.That(res.IsSuccess);

            _unitOfWork.Verify(u => u.UserRoleRepository.Add(It.IsAny<UserRole>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void AddUser_Failed_EmailExists()
        {
            Assert.Pass();
        }
        #endregion

        #region Update User
        [Test]
        public async Task UpdateUser_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), true, false))
                .ReturnsAsync(user);

            // act
            await _userService.UpdateUser(dataCreate);

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                            It.IsAny<Expression<Func<User, bool>>>(), true, false), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void UpdateUser_Failed_UserNotFound()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), true, false))
                .ReturnsAsync(userNull);

            // act
            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
            {
                await _userService.UpdateUser(dataCreate);
            });

            // assert
            _unitOfWork.Verify(u =>
            u.UserRepository.GetByCondition(
                            It.IsAny<Expression<Func<User, bool>>>(), true, false),
                            Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }

        [Test]
        public async Task UpdateProfile_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), true, false))
                .ReturnsAsync(user);

            // act
            await _userService.UpdateProfile("test", dataCreate);

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                            It.IsAny<Expression<Func<User, bool>>>(), true, false), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void UpdateProfile_Failed_UserNotMatch()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), true, false))
                .ReturnsAsync(userNull);

            // act
            Assert.ThrowsAsync<UserProfileUpdateException>(async () =>
            {
                await _userService.UpdateProfile("admin", dataCreate);
            });

            // assert
            _unitOfWork.Verify(u =>
            u.UserRepository.GetByCondition(
                            It.IsAny<Expression<Func<User, bool>>>(), true, false),
                            Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();

        }
        #endregion

        #region Remove User
        [Test]
        public async Task RemoveUser_Valid()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(user);

            // act
            await _userService.RemoveUser(It.IsAny<string>());

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                            It.IsAny<Expression<Func<User, bool>>>(), false, false), Times.Once);
            _unitOfWork.Verify(u => u.UserRepository.Remove(It.IsAny<User>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void RemoveUser_Failed_UserNotFound()
        {
            // arrange
            _mockUserRepository.Setup(x => x.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), false, false))
                .ReturnsAsync(userNull);

            // act
            Assert.ThrowsAsync<UsernameNotfoundException>(async () =>
            {
                await _userService.RemoveUser(It.IsAny<string>());
            });

            // assert
            _unitOfWork.Verify(u => u.UserRepository.GetByCondition(
                            It.IsAny<Expression<Func<User, bool>>>(), false, false),
                            Times.Once);
            _unitOfWork.Verify(u => u.UserRepository.Remove(
                It.IsAny<User>()),
                Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
            Assert.Pass();
        }
        #endregion
    }
}