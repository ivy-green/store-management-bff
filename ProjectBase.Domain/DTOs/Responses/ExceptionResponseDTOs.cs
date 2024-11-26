namespace ProjectBase.Domain.DTOs.Responses
{
    public class GlobalExceptionDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = [];

    }
}
