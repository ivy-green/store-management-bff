
namespace ProjectBase.Insfracstructure.Services.Mail
{
    public interface IEmailService
    {
        Task SendEmailJSAsync<T>(string templateId, T args);
    }
}