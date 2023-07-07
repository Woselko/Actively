using PlayMakerApp.Models.Authentication.Email;

namespace PlayMakerApp.Services.UserServices.EmailService
{
    public interface IEmailService
    {
        public Task SendEmail(EmailMessage message);
    }
}
