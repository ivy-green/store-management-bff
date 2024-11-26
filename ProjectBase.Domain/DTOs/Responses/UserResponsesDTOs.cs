namespace ProjectBase.Domain.DTOs.Responses
{
    public class UserResponsesDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ReportTo { get; set; } = string.Empty;   
        public bool? IsAccountBlocked { get; set; }
        public BranchCommonResponseDTO? BranchData { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }

    public class UserInBranchResponsesDTO
    {
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }

    public class UserProfileResponsesDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public bool IsAccountBlocked { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
}