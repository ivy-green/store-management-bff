using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Message;
using ProjectBase.Domain.DTOs.Models;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Enums;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using System.Transactions;

namespace ProjectBase.Application.Services
{
    public class BillService : IBillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISnsMessage _snsMessage;
        private readonly IUserService _userService;
        private readonly AppSettingConfiguration _setting;
        public BillService(IUnitOfWork unitOfWork,
            ISnsMessage snsMessage,
            IUserService userService,
            AppSettingConfiguration setting)
        {
            _unitOfWork = unitOfWork;
            _snsMessage = snsMessage;
            _userService = userService;
            _setting = setting;
        }

        public async Task<Result<PageList<BillGetListDTO>>> GetPagedList(
            int pageIndex,
            int pageSize,
            string username,
            BillFilter statuses)
        {
            // filter bill by search conditions
            var billsListResponse = await _unitOfWork.BillRepository.GetPagedByCondition(
                item => (statuses.Status.IsNullOrEmpty() || statuses.Status.Any(x => x == (int)item.Status)),
                pageIndex, pageSize);
            if (billsListResponse is null)
            {
                return Error.NullVal;
            }

            // get user request's information
            User? user = await _unitOfWork.UserRepository.GetByCondition(u => u.Username == username);
            if (user is null)
            {
                return UserError.UserNotFound;
            }

            _unitOfWork.UserRepository.ExplicitLoad(user, u => u.Branch);
            _unitOfWork.UserRepository.LoadUserRole(ref user);

            // get BillDetails 
            foreach (var bill in billsListResponse.PageData)
            {
                if (bill is null)
                {
                    return BillError.NotFound;
                }

                _unitOfWork.BillRepository.ExplicitLoadCollection(bill, x => x.BillDetails);
                if (bill.BillDetails is null)
                {
                    return BillError.BillDetailsNotFound;
                }
            }

            var res = billsListResponse.PageData?
                .Where(x =>
                {
                    // shipper: Get all accepted bills and their shipping bills
                    if (user.UserRoles!.Any(x => x.Role!.RoleName == "Shipper"))
                    {
                        return x!.Status == BillStatus.Accepted || x.ShipperUsername == user.Username;
                    }

                    // customer: Get all bills that they've created
                    if (user.UserRoles!.Any(x => x.Role!.RoleName == "Customer"))
                    {
                        return x!.Username == user.Username;
                    }
                    return true;
                })
                .Select(x =>
                {
                    var data = x?.ProjectToEntity<Bill, BillGetListDTO>();
                    if (data is null)
                    {
                        throw new NullException("Error on converting data");
                    }

                    data.BillDetailsRequest = x!.BillDetails.Select(item =>
                        item.ProjectToEntity<BillDetails, BillDetailsGetListDTO>()).ToList();

                    data.TrackingLogger = JsonConvert.DeserializeObject<List<BillLogger>>(x.Log ?? "[]") ?? [];
                    data.TrackingLogger = data.TrackingLogger.OrderBy(x => x.TimeSpan).ToList();

                    return data;
                }).ToList() ?? [];

            return new PageList<BillGetListDTO>
            {
                PageData = res,
                PageIndex = billsListResponse.PageIndex,
                PageSize = billsListResponse.PageSize,
                TotalRow = res.Count
            };
        }

        public async Task<Result> AddBill(BillCreateDTO bill, string username)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (bill.BillDetailsRequest is null || bill.BillDetailsRequest.Count == 0)
                {
                    return BillError.BillDetailsNotFound;
                }

                if (_setting.AWSSection is null || _setting.AWSSection.BillTopic is null)
                {
                    return Error.AWSNotFound;
                }

                // get user
                var getUserResponse = await _userService.GetUser(username);
                if (!getUserResponse.IsSuccess)
                {
                    return getUserResponse;
                }

                User user = getUserResponse.Value;

                // Project DTO to Entity
                Bill res = bill.ProjectToEntity<BillCreateDTO, Bill>();
                res.CreateAt = DateTime.UtcNow;
                res.Status = BillStatus.New;
                res.Id = Guid.NewGuid().ToString();

                //_unitOfWork.UserRepository.ExplicitLoad(user, u => u.Branch);
                //if (user.Branch is not null)
                //{
                //    res.BranchName = user.Branch.Name;
                //}
                //return Result.Failure(BranchError.NotFound);

                await _unitOfWork.BillRepository.Add(res);
                await _unitOfWork.SaveChangesAsync();

                // Add bill details
                var billDetails = bill.BillDetailsRequest
                    .Select(x =>
                    {
                        var detail = x.ProjectToEntity<BillDetailsCreateDTO, BillDetails>();
                        detail.BillId = res.Id!;
                        return detail;
                    }).ToList();

                await _unitOfWork.BillDetailsRepository.AddRange(billDetails);
                await _unitOfWork.SaveChangesAsync();

                // Send message to SNS
                var message = new StatisticBillMessageDTO
                {
                    Bill = res,
                    Type = "Create",
                    Content = "Create Bill",
                    CreateTime = DateTime.UtcNow,
                };

                await _snsMessage.PublishMessage(_setting.AWSSection.BillTopic, message);

                await UpdateBillLogger(res, new BillLogger
                {
                    Action = "Create",
                    TimeSpan = DateTime.UtcNow,
                    Username = username,
                    Note = "Create new bill"
                });

                // Complete the transaction
                transaction.Complete();
            }
            return true;
        }

        public async Task<Result> UpdateStatus(string billId, int status, string username)
        {
            var getUserWithRoleResponse = await _userService.GetUserWithRole(username);
            if (!getUserWithRoleResponse.IsSuccess)
            {
                return UserError.UserNotFound;
            }

            User user = getUserWithRoleResponse.Value;

            // customer cannot change the status (TODO: may cancel later - incomming feature)
            // shipper can only set Deliver status for the bill
            if (user.UserRoles!.Any(x => x.Role!.RoleName == "Customer") ||
                (user.UserRoles!.Any(x => x.Role!.RoleName == "Shipper") &&
                status != (int)BillStatus.Delivering))
            {
                throw new UnauthorizedAccessException();
            }

            Bill? billExists = await _unitOfWork.BillRepository
                .GetByCondition(x => x.Id == billId, trackChange: true);
            if (billExists is null)
            {
                return BillError.NotFound;
            }

            billExists.Status = (BillStatus)status;
            if (status == (int)BillStatus.Delivering)
            {
                billExists.ShipperUsername = user.Username;
            }
            await _unitOfWork.SaveChangesAsync();

            // upgrade customer's type base on bill revenue
            if (status == (int)BillStatus.Finished && billExists.CustomerUsername is not null)
            {
                await _userService.UpdateCustomerType(billExists.CustomerUsername);
            }

            await UpdateBillLogger(billExists, new BillLogger
            {
                Action = "Update",
                TimeSpan = DateTime.UtcNow,
                Username = username,
                Note = $"Update bill {billExists.Id} status to {(BillStatus)billExists.Status}"
            });

            return true;
        }

        public async Task<Result> UpdateBillLogger(Bill bill, BillLogger data)
        {
            var listConverted = JsonConvert.DeserializeObject<List<BillLogger>>(bill.Log ?? "[]") ?? [];
            listConverted.Add(data);

            var jsonData = JsonConvert.SerializeObject(listConverted);
            bill.Log = jsonData;

            _unitOfWork.BillRepository.Update(bill);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
