
using System.Net.Mail;

public interface IEmailSenderService
{
    Task SendEmailAsync(MailAddressCollection to, string subject, string body);
}