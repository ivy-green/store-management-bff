namespace ProjectBase.Jobs.Core.DTOs.Responses
{
    public class GeneralStatisticByMonthResponseDTO
    {
        public string? Driver { get; set; }
        public string? Team { get; set; }
        public double Revenue { get; set; }
        public int Bills { get; set; }
        public int Products { get; set; }
    }
}
