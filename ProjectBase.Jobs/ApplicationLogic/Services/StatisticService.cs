using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using ProjectBase.Jobs.Core.Configuration;
using ProjectBase.Jobs.Core.DTOs.Requests;
using ProjectBase.Jobs.Core.DTOs.Responses;
using ProjectBase.Jobs.Core.Entities;
using ProjectBase.Jobs.Core.Interfaces;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Core.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;

namespace ProjectBase.Jobs.ApplicationLogic.Services
{
    [ExcludeFromCodeCoverage]
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqsMessage _sqsMessage;
        private readonly IDynamoRepository _dynamoRepository;
        private readonly AppSetting _setting;
        public StatisticService(
            IUnitOfWork unitOfWork,
            ISqsMessage sqsMessage,
            IDynamoRepository dynamoRepository,
            AppSetting setting)
        {
            _unitOfWork = unitOfWork;
            _sqsMessage = sqsMessage;
            _dynamoRepository = dynamoRepository;
            _setting = setting;
        }

        #region Update
        public async Task UpdateBillStatistic()
        {
            if (_setting.AWSSection is null || _setting.AWSSection.StatisticBillQueue is null)
            {
                throw new Exception("AWS setting is missing");
            }

            var today = DateTime.UtcNow;
            List<Bill> bills = [];

            // restore from DLQ
            await _sqsMessage.RestoreFromDeadLetterQueueAsync(_setting.AWSSection.StatisticBillQueue);

            // Read messages from queue
            while (true)
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var message = await _sqsMessage.ReceiveSingleSQSMessage(_setting.AWSSection.StatisticBillQueue, default);
                    if (message is null)
                    {
                        break; // no message found
                    }

                    // processing message
                    var convertedItem = JsonConvert.DeserializeObject<PublishRequest>(message.Body);
                    if (convertedItem is not null)
                    {
                        var billItem = JsonConvert.DeserializeObject<StatisticBillMessageDTO>(convertedItem.Message);
                        if (billItem is not null && billItem.Bill is not null)
                        {
                            bills.Add(billItem.Bill);
                        }
                    }

                    transaction.Complete();
                    await _sqsMessage.RemoveSQSMessage(_setting.AWSSection.StatisticBillQueue, message);
                }
            }

            if (bills.Count == 0)
            {
                return;
            }

            // Find if statistic exists
            var record = await GetByDate(today);
            if (record is null)
            {
                record = await AddBillStatistic(DateOnly.FromDateTime(today));
            }

            // update statistic data
            record.ProductQuantity += bills.Select(x => x.BillDetails.Sum(x => x.Quantity)).Sum();
            record.BillQuantity += bills.Count();
            record.Revenue += bills.Select(x => x.BillDetails.Sum(x => x.TotalPrice)).Sum();

            await _unitOfWork.SaveChangesAsync();
            _unitOfWork.StatisticBillRepository.Update(record);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<StatisticBill?> GetByDate(DateTime today)
        {
            try
            {
                return await _unitOfWork.StatisticBillRepository
                    .GetByCondition(x => x.Date == DateOnly.FromDateTime(today));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<StatisticBill> AddBillStatistic(DateOnly today)
        {
            StatisticBill data = new()
            {
                Date = today,
            };

            await _unitOfWork.StatisticBillRepository.Add(data);
            await _unitOfWork.SaveChangesAsync();

            return data;
        }
        #endregion

        #region General Statistic
        public async Task UpdateGeneralStatistic()
        {
            var today = DateTime.UtcNow;
            var bills = await _unitOfWork.BillRepository
                .GetListByCondition(x => x.CreateAt.Month == today.Month && x.CreateAt.Year == today.Year);
            var products = await _unitOfWork.ProductRepository.GetListByCondition(x => true);

            GeneralStatisticByMonthResponseDTO data = new()
            {
                Bills = bills.Count,
                Products = products.Count,
                Revenue = bills.Sum(x => x.TotalPrice),
                Driver = today.Year.ToString(),
                Team = today.Month.ToString()
            };

            if (_setting.DynamoDBTables is null)
            {
                throw new Exception("DynamoDB setting is missing");
            }

            try
            {
                // find if it exists
                var existsRecord = await _unitOfWork.DynamoRepository.GetById<GeneralStatisticByMonthResponseDTO>(
                    _setting.DynamoDBTables.GeneralStatisticTable, today.Year.ToString(), today.Month.ToString());

                await _unitOfWork.DynamoRepository.Update(_setting.DynamoDBTables.GeneralStatisticTable, data);
            }
            catch
            {
                await _unitOfWork.DynamoRepository.Add(_setting.DynamoDBTables.GeneralStatisticTable, data);
            }
        }
        #endregion
    }
}
