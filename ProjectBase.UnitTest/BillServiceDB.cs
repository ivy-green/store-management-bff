using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProjectBase.Application.Services;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Insfracstructure.Data;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using ProjectBase.Insfracstructure.Services.Message.SQS;

namespace ProjectBase.UnitTest
{
    public class BillServiceTestDB
    {
        private ServiceProvider _serviceProvider;

        private Mock<ISnsMessage> _mockSnsService;
        private Mock<ISqsMessage> _mockSqsService;
        private AppSettingConfiguration setting;
        private Mock<IUserService> _mockUserService;

        BillService billService;
        IUnitOfWork unitOfWork;

        BillCreateDTO dataCreate;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

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

            _mockSnsService = new Mock<ISnsMessage>();
            _mockSqsService = new Mock<ISqsMessage>();
            _mockUserService = new Mock<IUserService>();

            dataCreate = new BillCreateDTO
            {
                Id = "testtt",
                Username = "test",
                TotalPrice = 12000,
                DiscountPrice = 0,
                BillDetailsRequest = [
                    new BillDetailsCreateDTO {
                        ProductName = "test",
                        Price = 20000,
                        Quantity = 2,
                    },
                ]
            };

        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        #region add

        [Test]
        public async Task AddBill_Should_Add_Bill_And_Details()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            billService = new BillService(
                unitOfWork,
                _mockSnsService.Object,
                _mockUserService.Object,
                setting);

            // Act
            await billService.AddBill(dataCreate, "");

            // Assert
            var bill = await unitOfWork.BillRepository.GetByCondition(
                item => (item.CreateAt == dataCreate.CreateAt &&
                item.Username == dataCreate.Username));
            Assert.IsNotNull(bill);
            Assert.That(bill.CreateAt, Is.EqualTo(dataCreate.CreateAt));

            var billDetails = await unitOfWork.BillDetailsRepository.GetListByCondition(
                item => item.BillId == bill.Id);
            Assert.IsNotNull(billDetails);
        }

        #endregion

        #region getPagedList

        [Test]
        public async Task GetPagedList_Should_Get_PageList()
        {
            // Arrange
            using var scope = _serviceProvider.CreateScope();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            billService = new BillService(
                unitOfWork,
                _mockSnsService.Object,
                _mockUserService.Object,
                setting);

            var statuses = new BillFilter
            {
                Status = []
            };

            // Act
            var res = await billService.GetPagedList(0, 10, "test", statuses);

            // Assert
            Assert.IsNotNull(res);
            var bill = await unitOfWork.BillRepository.GetAll(0, 10);
            Assert.IsNotNull(bill);
            Assert.That(bill.PageData.Count(), Is.EqualTo(res.Value.PageData.Count()));
        }

        #endregion
    }
}