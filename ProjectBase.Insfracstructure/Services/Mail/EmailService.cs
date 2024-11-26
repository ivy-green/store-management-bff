using Newtonsoft.Json;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Insfracstructure.DTOs.Requests;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ProjectBase.Insfracstructure.Services.Mail
{
    [ExcludeFromCodeCoverage]
    public class EmailService : IEmailService
    {
        private readonly AppSettingConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public EmailService(AppSettingConfiguration configuration, IHttpClientFactory httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient.CreateClient();
        }
        public Task SendEmailJSAsync<T>(string templateId, T args)
        {
            if (_configuration.EmailJs is null || _configuration.EmailJs.ServiceId is null)
            {
                throw new NullException("EmailJs setting is missing");
            }

            var mailRequest = new EmailRequestDTO<T>
            {
                ServiceId = _configuration.EmailJs.ServiceId,
                TemplateId = templateId,
                TemplateParams = args,
                UserId = _configuration.EmailJs.UserId,
                AccessToken = _configuration.EmailJs.AccessToken,
            };

            var json = JsonConvert.SerializeObject(mailRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _httpClient.PostAsync(_configuration.EmailJs.Url, content);
        }
    }
}
