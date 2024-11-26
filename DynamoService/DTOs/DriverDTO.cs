using System.Text.Json.Serialization;

namespace DynamoService.DTOs
{
    public class DriverDTO
    {
        // Partition Key
        [JsonPropertyName("driverName")]
        // Sort Key
        public string DriverName { get; set; } = string.Empty;
        [JsonPropertyName("teamName")]
        public string TeamName { get; set; } = string.Empty;
        [JsonPropertyName("driverNumber")]
        public string DriverNumber { get; set; } = string.Empty;
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
    }
}
