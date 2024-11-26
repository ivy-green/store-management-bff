namespace ProjectBase.Domain.DTOs.Requests
{
    public class UserCreateDTO
    {
        public string Fullname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int RoleCode { get; set; }
        public bool? SetAccountBlocked { get; set; } = null;
        public int BranchCode { get; set; }
        public string? ReportToPersonUsername { get; set; }
    }
}
