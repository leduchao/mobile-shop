namespace MobileWeb.Services.EmailService;

public class MailSettings
{
    public string From { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
