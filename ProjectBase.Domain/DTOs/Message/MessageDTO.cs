namespace ProjectBase.Domain.DTOs.Message
{
    public abstract class MessageDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
    }
}
