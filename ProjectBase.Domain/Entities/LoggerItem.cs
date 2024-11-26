namespace ProjectBase.Domain.Entities
{
    public class LoggerItem
    {
        public string Action { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
