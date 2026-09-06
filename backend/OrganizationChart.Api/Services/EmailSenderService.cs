using System.Net.Mail;
public class EmailSenderService:IEmailSenderService
{   
    private readonly IConfiguration _configuration;
    public EmailSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string to,string subject,string body)
    {
        using (var client = new SmtpClient(_configuration["Email:Smtp:Host"], int.Parse(_configuration["Email:Smtp:Port"])))
        {
            client.Credentials = new System.Net.NetworkCredential(_configuration["Email:Smtp:Username"], _configuration["Email:Smtp:Password"]);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);
            await client.SendMailAsync(mailMessage);
        }
    }
}