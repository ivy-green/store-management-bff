namespace ProjectBase.Domain.DTOs.Responses
{
    public class GeneralStatisticByMonthResponseDTO
    {
        public string Driver { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public double Revenue { get; set; }
        public int Bills { get; set; }
        public int Products { get; set; }
    }
}
