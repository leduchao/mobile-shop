using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MobileWeb.Services.EmailService;

public class EmailService :	IEmailService
{
	private readonly MailSettings _mailSettings;
	private readonly ILogger<EmailService> _logger;

	public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
	{
		_mailSettings = mailSettings.Value;
		_logger = logger;
		_logger.LogInformation("Email service created!");
	}

	public async Task SendMailAsync(string to, string subject, string body)
	{
		using var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
		{
			Credentials = new NetworkCredential(_mailSettings.From, _mailSettings.Password),
			EnableSsl = true,
		};

		try
		{
			var message = new MailMessage(new MailAddress(_mailSettings.From, _mailSettings.DisplayName), new MailAddress(to))
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			await client.SendMailAsync(message);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}
