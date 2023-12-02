namespace MobileWeb.Services.EmailService;

public interface IEmailService
{
    Task SendMailAsync(string to, string subject, string body);
}
