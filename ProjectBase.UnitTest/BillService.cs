using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectBase.Application.Services;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;
using ProjectBase.Insfracstructure.Data;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using ProjectBase.Insfracstructure.Services.Message.SQS;

namespace ProjectBase.UnitTest
{
    public class BillServiceTest
    {
        private IBillService _BillService;
        private ISqsMessage _sqsService;

        private Mock<ISnsMessage> _mockSnsService;
        private Mock<ISqsMessage> _mockSqsService;

        private Mock<IUserService> _mockUserService;

        private Mock<IAmazonSQS> _mockAmazonSQS;

        private Mock<AppSettingConfiguration> _setting;
        private Mock<AWSSection> _awsSetting;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IBillRepository> _mockBillRepository;
        private Mock<IBillDetailsRepository> _mockBillDetailsRepository;

        Product type;
        Product? typeNull;
        User user;
        User? userNull;
        Bill? BillNull;
        Bill Bill;
        BillDetails BillDetails;
        PageList<Bill> Bills;
        PageList<Bill> BillsNull;
        BillCreateDTO dataCreate;
        BillCreateDTO dataCreateInvalid;
        AppSettingConfiguration setting;
        DbContextOptions options;

        [SetUp]
        public void Setup()
        {
            BillNull = null;
            userNull = null;
            typeNull = null;
            BillsNull = new PageList<Bill>();

            #region arrange data

            dataCreate = new BillCreateDTO
            {
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

            dataCreateInvalid = new BillCreateDTO
            {
                Username = "test",
                TotalPrice = 12000,
                DiscountPrice = 0,
                BillDetailsRequest = null
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

            Bill = new Bill
            {
            };

            Bills = new PageList<Bill>()
            {
                PageIndex = 0,
                PageSize = 10,
                PageData = [Bill]
            };

            #endregion

            _mockBillRepository = new Mock<IBillRepository>();
            _mockBillDetailsRepository = new Mock<IBillDetailsRepository>();
            _mockSnsService = new Mock<ISnsMessage>();
            _mockSqsService = new Mock<ISqsMessage>();
            _mockUserService = new Mock<IUserService>();

            _mockAmazonSQS = new Mock<IAmazonSQS>();

            _setting = new Mock<AppSettingConfiguration>();
            _awsSetting = new Mock<AWSSection>();
            _unitOfWork = new Mock<IUnitOfWork>();

            setting = new AppSettingConfiguration
            {
                AWSSection = new AWSSection
                {
                    BillTopic = "BillTopic"
                },
            };

            var db = GetMemoryContext();

            _unitOfWork.SetupGet(x => x.BillRepository).Returns(_mockBillRepository.Object);
            _unitOfWork.SetupGet(x => x.BillDetailsRepository).Returns(
                _mockBillDetailsRepository.Object);

            _BillService = new BillService(
                _unitOfWork.Object,
                _mockSnsService.Object,
                _mockUserService.Object,
                setting);

            _sqsService = new SqsMessage(_mockAmazonSQS.Object, setting);
        }

        public AppDBContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            return new AppDBContext(options);
        }

        #region get list

        [Test]
        public void GetList_Invalid()
        {
            // arrange
            _mockBillRepository.Setup(x => x.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false))
                .ReturnsAsync(BillsNull);

            var status = new BillFilter
            {
                Status = []
            };

            // act
            Assert.ThrowsAsync<NullException>(async () =>
            {
                await _BillService.GetPagedList(It.IsAny<int>(), It.IsAny<int>(), "test", status);
            });

            // assert
            _unitOfWork.Verify(u => u.BillRepository.GetAll(
                It.IsAny<int>(), It.IsAny<int>(), false), Times.Once);
        }

        #endregion

        #region create Bill

        [Test]
        public void AddBill_Failed_BillDetailsEmpty()
        {
            // arrange && act
            var res = Assert.ThrowsAsync<NullException>(async () =>
            {
                await _BillService.AddBill(dataCreateInvalid, "");
            });

            // assert
            _unitOfWork.Verify(u => u.BillRepository.Add(It.IsAny<Bill>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region SNS message

        //[Test]
        public async Task AddBill_Valid_GetMessage()
        {
            // arrange && act
            await _BillService.AddBill(dataCreate, "");

            var res = await _sqsService.ReceiveSQSMessage(
                setting.AWSSection.BillTopic,
                default,
                false);

            // assert
            _unitOfWork.Verify(u => u.BillRepository.Add(It.IsAny<Bill>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeast(2));
            Assert.Pass();
        }

        #endregion

    }
}