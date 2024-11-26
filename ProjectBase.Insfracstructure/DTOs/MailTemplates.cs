using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.DTOs
{
    [ExcludeFromCodeCoverage]
    public static class MailTemplateID
    {
        public readonly static string UploadUserProfileMailTemplateID = "template_tbyt6vr";
    }

    public class UploadUserProfileMailTemplate
    {
        [JsonProperty("receiver")]
        public string? Receiver { get; set; }
        [JsonProperty("reply_to")]
        public string? ReplyTo { get; set; }
        [JsonProperty("user_name")]
        public string? Username { get; set; }
    }
}
