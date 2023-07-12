using ActivelyApp.Models.Authentication.Email;

namespace ActivelyApp.Services.UserServices.EmailService
{
    public interface IEmailService
    {
        public Task SendEmail(EmailMessage message);
    }
}
