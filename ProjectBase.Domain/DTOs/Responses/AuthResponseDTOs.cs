namespace ProjectBase.Domain.DTOs.Responses
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
        
        public int? BranchCode { get; set; }
        public string? BranchName { get; set; }
    }

    public class RoleResponseDTO
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
