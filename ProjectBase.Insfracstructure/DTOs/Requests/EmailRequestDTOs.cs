using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.DTOs.Requests
{
    [ExcludeFromCodeCoverage]
    public class EmailRequestDTO<T>
    {
        [JsonProperty("service_id")]
        public string? ServiceId { get; set; }
        [JsonProperty("template_id")]
        public string? TemplateId { get; set; }
        [JsonProperty("template_params")]
        public T? TemplateParams { get; set; }
        [JsonProperty("user_id")]
        public string? UserId { get; set; }
        [JsonProperty("accessToken")]
        public string? AccessToken { get; set; }
    }
}
