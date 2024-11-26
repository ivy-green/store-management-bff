namespace ProjectBase.Domain.DTOs.Requests
{
    public class LoginRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDTO
    {
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleCode { get; set; }
        public string? OauthToken { get; set; } // for email login
    }
}
