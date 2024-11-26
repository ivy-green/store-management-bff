namespace ProjectBase.Domain.DTOs.Requests
{
    public class LoggerCreateRequestDTO
    {
        public string Driver { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
