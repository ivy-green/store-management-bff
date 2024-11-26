using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;
using System.Diagnostics.CodeAnalysis;
using Error = ProjectBase.Domain.Abstractions.Error;

namespace ProjectBase.Application.Services
{
    [ExcludeFromCodeCoverage]
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettingConfiguration _setting;
        private readonly DynamoService.Repositories.Interfaces.IDynamoRepository _dynamoRepository;
        public StatisticService(
            IUnitOfWork unitOfWork,
            AppSettingConfiguration setting,
            DynamoService.Repositories.Interfaces.IDynamoRepository dynamoRepository)
        {
            _unitOfWork = unitOfWork;
            _setting = setting;
            _dynamoRepository = dynamoRepository;
        }

        #region Update, Get Revenue
        public async Task<Result<PageList<StatisticBill>>> GetRevenue(int pageIndex, int pageSize)
        {
            var res = await _unitOfWork.StatisticBillRepository.GetAll(pageIndex, pageSize);
            return res;
        }

        public async Task<Result<PageList<StatisticBill>>> GetRevenueByDays(string startDate, int numberOfDays)
        {
            bool conversionSuccessful = DateTime.TryParseExact(
                startDate,
                "yyyy/MM/dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime result);

            if (conversionSuccessful)
            {
                var response = await _unitOfWork.StatisticBillRepository
                    .GetPagedByCondition(x => x.Date >= DateOnly.FromDateTime(result.Date) &&
                                              x.Date < DateOnly.FromDateTime(result.Date.AddDays(numberOfDays)));

                return response;
            }

            return StatisticError.InvalidDateTime;
        }
        #endregion

        #region General Statistic
        public async Task<Result<GeneralStatisticByMonthResponseDTO?>> GetGeneralStatistic(int month)
        {
            try
            {
                if (_setting.DynamoDBTables is null)
                {
                    return Error.NullVal;
                }

                var today = DateTime.UtcNow;

                var existsRecord = await _dynamoRepository.GetById<GeneralStatisticByMonthResponseDTO>(
                    _setting.DynamoDBTables.GeneralStatisticTable,
                    today.Year.ToString(),
                    month.ToString());

                return existsRecord;
            }
            catch
            {
                return Error.None;
            }
        }
        #endregion
    }
}
