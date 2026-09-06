
public interface IEmailSenderService
{
    Task SendEmailAsync(string to, string subject, string body);
}