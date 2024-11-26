using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProjectBase.Application.Extensions;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.Constants;
using ProjectBase.Domain.DTOs.Message;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Enums;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Extensions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IFactories;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Domain.Pagination;
using ProjectBase.Insfracstructure.DTOs;
using ProjectBase.Insfracstructure.Services.FileService;
using ProjectBase.Insfracstructure.Services.Mail;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Application.Services
{

    public class UserService : IUserService
    {
        // private readonly IBlobService _blobService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly AppSettingConfiguration _setting;
        private readonly ISnsMessage _snsMessage;
        private readonly IHashService _hashService;
        private readonly IEmailService _mailService;
        private readonly IServiceProvider _serviceProvider;

        public UserService(
            // IBlobService blobService,
            IUnitOfWork unitOfWork,
            IFileService fileService,
            AppSettingConfiguration setting,
            ISnsMessage snsMessage,
            IHashService hashService,
            IEmailService mailService,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _setting = setting;
            _snsMessage = snsMessage;
            // _blobService = blobService;
            _hashService = hashService;
            _mailService = mailService;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<PageList<UserResponsesDTO>>> GetPagedList(
            int pageIndex,
            int pageSize,
            string currUsername)
        {
            var users = await _unitOfWork.UserRepository.GetAll(pageIndex, pageSize);
            if (users is null || users.PageData is null)
            {
                return Error.NullVal;
            }

            foreach (var user in users.PageData)
            {
                if (user is null)
                {
                    return Error.NullVal;
                }

                _unitOfWork.UserRepository.ExplicitLoadCollection(user, u => u.UserRoles);
                if (user.UserRoles is null)
                {
                    return Error.NullVal;
                }

                _unitOfWork.UserRepository.ExplicitLoad(user, u => u.Branch);

                foreach (var role in user.UserRoles)
                {
                    _unitOfWork.UserRoleRepository.ExplicitLoad(role, r => r.Role);
                }
            }

            var res = users.PageData.Select(u =>
            {
                return new UserResponsesDTO
                {
                    Username = u?.Username ?? "",
                    Fullname = u?.Fullname ?? "",
                    Email = u?.Email ?? "",
                    Roles = u?.UserRoles is not null
                        ? u?.UserRoles.Select(rs => rs.Role is null ? "" : rs.Role.RoleName).ToList()
                        : []
                };
            }).Where(u => u.Username != currUsername);

            return new PageList<UserResponsesDTO>
            {
                PageData = res,
                PageIndex = users.PageIndex,
                PageSize = users.PageSize,
                TotalRow = users.TotalRow,
            };
        }

        public async Task<Result<List<User>>> GetUserByRole(int pageIndex, int pageSize, int RoleCode)
        {
            List<User> users = await _unitOfWork.UserRepository
                .GetListByCondition(
                        user => !user.UserRoles!.Where(x => x.Role!.Code == RoleCode).IsNullOrEmpty(),
                        pageIndex, pageSize);

            return users;
        }

        public async Task<Result<User>> GetUser(string username)
        {
            var user = await _unitOfWork.UserRepository.GetByCondition(u => u.Username == username);
            return user is null ? UserError.UserNotFound : user;
        }

        public async Task<Result<User>> GetUserWithRole(string username)
        {
            var user = await _unitOfWork.UserRepository.GetByCondition(u => u.Username == username);
            if (user is null)
            {
                return UserError.UserNotFound;
            }

            _unitOfWork.UserRepository.LoadUserRole(ref user);

            return user;
        }

        public async Task<Result<UserProfileResponsesDTO>> GetProfile(string username)
        {
            var user = await _unitOfWork.UserRepository.GetByCondition(u => u.Username == username);
            if (user is null)
            {
                return UserError.UserNotFound;
            }

            // get profile image
            // var imageUrl = await GetProfileImage();

            _unitOfWork.UserRepository.ExplicitLoadCollection(user, u => u.UserRoles);
            if (user.UserRoles is null)
            {
                return UserError.UseRoleIsNull;
            }

            foreach (var role in user.UserRoles)
            {
                _unitOfWork.UserRoleRepository.ExplicitLoad(role, r => r.Role);
            }

            return new UserProfileResponsesDTO
            {
                Username = user.Username,
                Fullname = user.Fullname,
                Email = user.Email,
                Bio = user.Bio,
                IsEmailConfirmed = user.IsEmailConfirmed,
                IsAccountBlocked = user.IsAccountBlocked,
                // ProfileImage = imageUrl,
                Roles = user.UserRoles.Select(rs => rs.Role is null ? "" : rs.Role.RoleName)
                        .ToList()
            };
        }

        public async Task<Result> AddUser(UserCreateDTO data, bool isActivated = true)
        {
            var validResponse = CheckValidData(data);
            if (validResponse.IsFailure)
            {
                return validResponse;
            }

            var newUser = new User();
            var newUserRole = new UserRole();

            if (data.Username is null || data.Password is null)
            {
                return UserError.UsernameNotFound;
            }

            User? existsUser = await _unitOfWork.UserRepository
                .GetByCondition(x => x.Username == data.Username);
            if (existsUser is not null)
            {
                return UserError.UsernameExists;
            }

            var (password, salt) = _hashService.CreatePasswordHashAndSalt(data.Password);

            newUser = data.ProjectToEntity<UserCreateDTO, User>();
            newUser.PasswordHash = password;
            newUser.PasswordSalt = salt;


            var existsRole = await _unitOfWork.RoleRepository.GetByCondition(x => x.Code == data.RoleCode);
            if (existsRole is null || existsRole.RoleName is null)
            {
                return UserError.RoleNotFound;
            }

            // add user by role
            IUserFactory? userFactory = existsRole.RoleName switch
            {
                "Admin" => _serviceProvider.GetService<IAdminFactory>(),
                "Manager" => _serviceProvider.GetService<IManagerFactory>(),
                "Staff" => _serviceProvider.GetService<IStaffFactory>(),
                "Shipper" => _serviceProvider.GetService<IShipperFactory>(),
                "Customer" => _serviceProvider.GetService<ICustomerFactory>(),
                _ => throw new ArgumentException("Invalid role: ", nameof(existsRole.RoleName))
            };

            if (userFactory is null)
            {
                return Error.NullVal;
            }

            // need to be approved by admin when shipper register
            if (!isActivated) newUser.IsAccountBlocked = true;

            var res = await userFactory.CreateUser(newUser, data);
            if (!res.IsSuccess)
            {
                return res;
            }

            newUserRole = new UserRole
            {
                RoleCode = data.RoleCode,
                Username = data.Username
            };

            await _unitOfWork.UserRoleRepository.Add(newUserRole);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result> UpdateProfile(string username, UserCreateDTO user)
        {
            if (user.Username != username && user.RoleCode != 1)
            {
                return UserError.SameUser;
            }

            return await UpdateUser(user);
        }

        public async Task<Result> UpdateCustomerType(string username)
        {
            var getUserResponse = await GetUserWithRole(username);
            if (!getUserResponse.IsSuccess)
            {
                return UserError.CustomerNotFound;
            }

            User user = getUserResponse.Value;
            if (user.UserRoles!.Any(x => x.Role!.RoleName == "Customer"))
            {
                if (user.Type is null)
                {
                    return Error.NullVal;
                }
                try
                {
                    var nextType = CustomerType.FromValue(user.Type ?? 0)?.NextType;
                    user.Type = nextType?.Value;
                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception) { throw; }
            }

            return true;
        }

        public async Task<Result> UpdateUser(UserCreateDTO dataUpdate)
        {
            var validResponse = CheckValidData(dataUpdate, isUpdate: true);
            if (validResponse.IsFailure)
            {
                return validResponse;
            }

            // get User with Roles
            var getUserWithRoleResponse = await GetUserWithRole(dataUpdate.Username!);
            if (getUserWithRoleResponse.IsFailure)
            {
                return getUserWithRoleResponse;
            }
            User userExists = getUserWithRoleResponse.Value;

            Role? userRole = userExists.UserRoles!.FirstOrDefault()!.Role ?? null;
            if (userRole == null)
            {
                return Error.NullVal;
            }

            IUserFactory? userFactory = userRole.RoleName switch
            {
                "Admin" => _serviceProvider.GetService<IAdminFactory>(),
                "Manager" => _serviceProvider.GetService<IManagerFactory>(),
                "Staff" => _serviceProvider.GetService<IStaffFactory>(),
                "Shipper" => _serviceProvider.GetService<IShipperFactory>(),
                "Customer" => _serviceProvider.GetService<ICustomerFactory>(),
                _ => throw new ArgumentException("Invalid role: ", nameof(userRole.RoleName))
            };

            if (userFactory is null)
            {
                return Error.NullVal;
            }

            var res = await userFactory.UpdateUser(userExists, dataUpdate);
            if (!res.IsSuccess)
            {
                return res;
            }

            //// staff
            //if (userExists.UserRoles!.Where(x => x.RoleCode == 3).ToList().Count() > 0)
            //{
            //    var manager = await _unitOfWork.UserRepository.GetByCondition(x => x.Username == dataUpdate.ReportToPersonUsername);
            //    if (manager == null)
            //    {
            //        return UserError.ManagerNotFound;
            //    }
            //    userExists.ReportTo = manager;
            //}

            //// staff || manager
            //if (userExists.UserRoles!.Where(x => x.RoleCode == 3 || x.RoleCode == 2).ToList().Count() > 0)
            //{
            //    var branch = await _unitOfWork.BranchRepository.GetByCondition(x => x.Code == dataUpdate.BranchCode);
            //    if (branch == null)
            //    {
            //        return Result.Failure(UserError.ManagerNotFound);
            //    }
            //    userExists.BranchID = branch.Id;
            //}

            //userExists = dataUpdate.ProjectToEntity<UserCreateDTO, User>(userExists);
            //userExists.IsAccountBlocked = dataUpdate.SetAccountBlocked ?? userExists.IsAccountBlocked;

            //_unitOfWork.UserRepository.Update(userExists);
            //await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Result> RemoveUser(string username)
        {
            // check if username exists
            var userExists = await _unitOfWork.UserRepository.GetByCondition(u => u.Username == username);
            if (userExists == null)
            {
                return Result.Failure(UserError.UsernameNotFound);
            }

            // remove userRole first
            var userRoles = await _unitOfWork.UserRoleRepository.GetListByCondition(x => x.Username == username);
            _unitOfWork.UserRoleRepository.RemoveRange(userRoles);
            await _unitOfWork.SaveChangesAsync();

            _unitOfWork.UserRepository.Remove(userExists);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        /// <summary>
        /// Searches for users based on the provided criteria and returns a paginated list of user response DTOs.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the page to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="currUsername">The current username performing the search to exclude from the results.</param>
        /// <param name="searchString">The search string to filter users by username or email. Optional, defaults to an empty string.</param>
        /// <param name="branchCode">The branch code to filter users by branch. Optional, defaults to -1 indicating no filtering by branch.</param>
        /// <param name="roles">A comma-separated list of roles to filter users. Optional, defaults to an empty string.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains a <see cref="Result{T}"/> 
        /// with a <see cref="PageList{UserResponsesDTO}"/> if the operation is successful. If no users are found or any user 
        /// has invalid data, a failure result is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the search result or any user data is null.
        /// </exception>
        /// <example>
        /// <code>
        /// var result = await Search(0, 10, "currentUsername", "searchTerm", 123, "Admin,User");
        /// if (result.IsSuccess)
        /// {
        ///     // Process the result
        /// }
        /// </code>
        /// </example>
        public async Task<Result<PageList<UserResponsesDTO>>> Search(
            int pageIndex, int pageSize,
            string currUsername, string searchString = "", int branchCode = -1, string roles = "")
        {
            List<int> roleCodes = roles.Split(",").ToList().Where(x =>
            {
                int num;
                return Int32.TryParse(x, out num);
            }).Select(x => Int32.Parse(x)).ToList();

            var users = await _unitOfWork.UserRepository
                .GetPagedByCondition(u =>
                        (u.Username.Contains(searchString) || u.Email.Contains(searchString))
                        && u.Username != currUsername,
                    pageIndex, pageSize);

            if (users is null || users.PageData is null)
            {
                return Result.Failure<PageList<UserResponsesDTO>>(Error.NullVal);
            }

            foreach (var user in users.PageData)
            {
                if (user is null)
                {
                    return Result.Failure<PageList<UserResponsesDTO>>(Error.NullVal);
                }

                _unitOfWork.UserRepository.ExplicitLoadCollection(user, u => u.UserRoles);
                if (user.UserRoles is null)
                {
                    return Result.Failure<PageList<UserResponsesDTO>>(UserError.UseRoleIsNull);
                }

                _unitOfWork.UserRepository.ExplicitLoad(user, u => u.Branch);

                foreach (var role in user.UserRoles)
                {
                    _unitOfWork.UserRoleRepository.ExplicitLoad(role, r => r.Role);
                }

                if (user.UserRoles.Any(x => x.Role!.RoleName == "Staff"))
                {
                    _unitOfWork.UserRepository.ExplicitLoad(user, x => x.ReportTo);
                }
            }

            var res = users.PageData
                // filter by branchCode - Admin dont have branchCode
                .Where(u => branchCode != -1
                    ? u?.Branch is not null && u.Branch.Code == branchCode
                    : true)
                .Where(u => roleCodes.Count <= 0
                    || u!.UserRoles!.Any(x => roleCodes.Any(r => r == x.RoleCode)))
                .Select(u =>
                {
                    return new UserResponsesDTO
                    {
                        Username = u?.Username ?? "",
                        Fullname = u?.Fullname ?? "",
                        Email = u?.Email ?? "",
                        PhoneNumber = u?.PhoneNumber ?? "",
                        IsAccountBlocked = u?.IsAccountBlocked,
                        ReportTo = u?.ReportTo?.Username ?? "",
                        BranchData = u?.Branch is not null
                            ? u?.Branch.ProjectToEntity<Branch, BranchCommonResponseDTO>()
                            : null,
                        Roles = u?.UserRoles is not null
                            ? u?.UserRoles.Select(rs => rs.Role is null ? "" : rs.Role.RoleName)
                            .ToList() : []
                    };
                });

            return Result.Success(new PageList<UserResponsesDTO>
            {
                PageData = res,
                PageIndex = users.PageIndex,
                PageSize = users.PageSize,
                TotalRow = res.Count(),
            });
        }

        [ExcludeFromCodeCoverage]
        public async Task<Result> UploadProfileImage(IFormFile file, string mail)
        {
            // upload file
            await _fileService.UploadFileS3(_setting.AWSSection?.UserFileBucket ?? "", file);

            // data for message
            var data = new UploadProfileMessage
            {
                Mail = mail,
                Message = "Upload new user profile successfully"
            };

            var content = new StatisticBillMessageDTO
            {
                Type = MessageType.UPLOAD_PROFILE_TYPE,
                Content = JsonConvert.SerializeObject(data),
            };

            // check if topic nonexists then create
            /*var topicArnExists = await _snsMessage.GetTopicArnByName("UploadUserProfile");
            if (topicArnExists == null)
            {
                topicArnExists = await _snsMessage.CreateTopic("UploadUserProfile");
            }

            // send sns 
            await _snsMessage.PublishMessage(topicArnExists, content);*/

            //send mail
            var mailContent = new UploadUserProfileMailTemplate
            {
                Receiver = mail,
                ReplyTo = "",
                Username = "thaomy" // TODO: Change this spot
            };
            await _mailService.SendEmailJSAsync(_setting.EmailJs?.NotiUploadProfileSuccessEmailTemplateId ?? "", mailContent);
            return Result.Success();
        }

        [ExcludeFromCodeCoverage]
        public async Task<(MemoryStream, string)> DownloadProfileImage(string fileName)
        {
            var (res, contentType) = await _fileService.DownloadFileFromBucket(_setting.AWSSection?.UserFileBucket ?? "", fileName);
            return (res, contentType);
        }

        [ExcludeFromCodeCoverage]
        public async Task<Result<string>> GetProfileImage()
        {
            var urlImage = await _fileService.GetFileUrlS3(_setting.AWSSection?.UserFileBucket ?? "");
            return urlImage.ToString();
        }

        public static Result CheckValidData(UserCreateDTO data, bool isUpdate = false)
        {
            return Result.Success(data)
                .Ensure(x => isUpdate || x.Password is not null && x.Password.Length >= 8, UserError.InvalidPassword)
                .Ensure(x => x.Username is not null && x.Username?.Length > 3, UserError.UsernameNotFound)
                .Ensure(x => x.Fullname is not null && x.Fullname?.Length > 3, UserError.FullnameNotFound);
        }
    }
}
