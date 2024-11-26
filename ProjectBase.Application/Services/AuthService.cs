using Newtonsoft.Json;
using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.DTOs.Responses;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Errors;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Insfracstructure.Services.Jwts;
using System.Data;
using System.Security.Claims;

namespace ProjectBase.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IHashService _hashService;
        private readonly IUserService _userService;
        private readonly AppSettingConfiguration _setting;
        public AuthService(IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IHashService hashService,
            IUserService userService,
            AppSettingConfiguration setting)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _hashService = hashService;
            _userService = userService;
            _setting = setting;
        }

        #region RefreshToken
        public async Task<LoginResponseDTO> RefreshToken(string refreshToken)
        {
            // get refreshToken request
            RefreshToken? refreshTokenRequestObj = JsonConvert
                .DeserializeObject<RefreshToken>(_hashService.Base64Decode(refreshToken));
            if (refreshTokenRequestObj is null)
            {
                throw new UnauthorizedAccessException();
            }

            // verify refreshToken
            var refreshTokenTime = refreshTokenRequestObj.CreateAt.AddMinutes(refreshTokenRequestObj.Duration);
            var currentDateTime = DateTime.UtcNow;
            if ((refreshTokenTime - currentDateTime).TotalMinutes < 0)
            {
                throw new UnauthorizedAccessException();
            }

            User? user = await _unitOfWork.UserRepository.GetByCondition(x => x.Username == refreshTokenRequestObj.Username);
            if (user == null)
            {
                throw new UsernameNotfoundException();
            }

            var (claims, roles) = GenerateClaimFromUser(user);
            var (jwtToken, newRefreshToken) = GenerateTokens(claims, user.Username);

            user.VerifyToken = jwtToken;
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDTO
            {
                AccessToken = jwtToken,
                RefreshToken = newRefreshToken,
                Roles = roles,
            };
        }
        #endregion

        #region Logout
        public async Task Logout(string? accessToken)
        {
            if (accessToken is null)
            {
                throw new AuthorizationException();
            }

            // Add userName and accessToken to BlackList
            Blacklist bl = new Blacklist
            {
                Token = accessToken,
                Action = Domain.Entities.Action.Logout
            };

            await _unitOfWork.BlacklistRepository.Add(bl);
            await _unitOfWork.SaveChangesAsync();
        }
        #endregion

        #region Register
        public async Task<Result> Register(UserCreateDTO data)
        {
            return await _userService.AddUser(data);
            // TODO: add more register methods later
        }
        #endregion

        #region Login
        public async Task<Result<LoginResponseDTO>> Login(LoginRequestDTO data)
        {
            // check username if exixts
            User? user = await _unitOfWork.UserRepository.GetByCondition(x => x.Username == data.Username, true);
            if (user is null)
            {
                return UserError.UsernameNotFound;
            }

            // check hash password
            var verifyUser = _hashService.VerifyPasswordHash(user.PasswordSalt, user.PasswordHash, data.Password);
            if (!verifyUser)
            {
                return AuthError.WrongPassword;
            }

            if (user.IsAccountBlocked == true)
            {
                return AuthError.UserBlocked;
            }

            var (claims, roles) = GenerateClaimFromUser(user);
            var (jwtToken, refreshToken) = GenerateTokens(claims, user.Username);

            // save current verify token
            user.VerifyToken = jwtToken;
            await _unitOfWork.SaveChangesAsync();

            // get branch info
            if (user.BranchID != null)
            {
                _unitOfWork.UserRepository.ExplicitLoad(user, user => user.Branch);
            }

            return new LoginResponseDTO
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken,
                Roles = roles,
                BranchCode = user.Branch is not null ? user.Branch.Code : null,
                BranchName = user.Branch is not null ? user.Branch.Name : null,
            };
        }
        private (string, string) GenerateTokens(IEnumerable<Claim> accessTokenClaims, string username)
        {
            string accessToken = _jwtService.GenerateSecurityToken(accessTokenClaims);
            var refreshToken = GenerateRefreshToken(username);
            return (accessToken, refreshToken);
        }

        private string GenerateRefreshToken(string username)
        {
            var refreshToken = new RefreshToken
            {
                Username = username,
                CreateAt = DateTime.UtcNow,
                Duration = _setting.RefreshTokenDuration
            };

            string json = JsonConvert.SerializeObject(refreshToken);
            string hash = _hashService.Base64Encode(json);
            return hash;
        }
        #endregion

        private (List<Claim>, List<string>) GenerateClaimFromUser(User user)
        {
            // Load roles of user
            _unitOfWork.UserRepository.ExplicitLoadCollection(user, u => u.UserRoles);
            if (user.UserRoles == null)
            {
                throw new NullException();
            }

            foreach (var role in user.UserRoles)
            {
                _unitOfWork.UserRoleRepository.ExplicitLoad(role, r => r.Role);
            }

            // Generate user's claims
            var roles = user.UserRoles.Select(r => r.Role!.RoleName).ToList();
            string userRole = string.Join(",", roles);

            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Username.ToString()),
            ];

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            return (claims, roles);
        }
    }
}
