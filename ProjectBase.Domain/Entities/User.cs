using ProjectBase.Domain.Interfaces;

namespace ProjectBase.Domain.Entities
{
    public class User : IUser
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public string? VerifyToken { get; set; } = string.Empty; // for email verify token 
        public DateTime CreateAt { get; set; }
        public string? ResetPasswordToken { get; set; } = null;
        public DateTime? ResetPasswordTokenExpiredAt { get; set; } = null;
        public DateTime? TokenExpiredTime { get; set; }

        public int? Type { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public bool IsAccountBlocked { get; set; } = false;

        public string? BranchID { get; set; }
        public virtual Branch? Branch { get; set; }

        public virtual string? ReportToId { get; set; }
        public virtual User? ReportTo { get; set; }

        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<Bill>? Bills { get; set; }

    }
}
