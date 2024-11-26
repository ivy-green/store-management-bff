using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.Interfaces
{
    public interface IUser
    {
        ICollection<Bill>? Bills { get; set; }
        string? Bio { get; set; }
        DateTime CreateAt { get; set; }
        string Email { get; set; }
        string Fullname { get; set; }
        bool IsAccountBlocked { get; set; }
        bool IsEmailConfirmed { get; set; }
        string PasswordHash { get; set; }
        string PasswordSalt { get; set; }
        string PhoneNumber { get; set; }
        string? ResetPasswordToken { get; set; }
        DateTime? ResetPasswordTokenExpiredAt { get; set; }
        DateTime? TokenExpiredTime { get; set; }
        string Username { get; set; }
        ICollection<UserRole>? UserRoles { get; set; }
        string? VerifyToken { get; set; }
    }
}