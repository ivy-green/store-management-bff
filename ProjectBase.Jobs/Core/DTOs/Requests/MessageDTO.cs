namespace ProjectBase.Jobs.Core.DTOs.Requests
{
    public abstract class MessageDTO
    {
        public string? Type { get; set; }
        public string? Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
