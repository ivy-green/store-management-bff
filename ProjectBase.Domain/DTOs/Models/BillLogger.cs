namespace ProjectBase.Domain.DTOs.Models
{
    public class BillLogger
    {
        public string Action { get; set; } = string.Empty;
        public DateTime TimeSpan { get; set; } = DateTime.UtcNow;
        public string Username { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
