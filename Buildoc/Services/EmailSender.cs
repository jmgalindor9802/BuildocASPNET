using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;

    public EmailSender(IConfiguration configuration)
    {
        _smtpClient = new SmtpClient
        {
            Host = configuration["EmailSettings:Host"],
            Port = int.Parse(configuration["EmailSettings:Port"]),
            EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"]),
            Credentials = new NetworkCredential(
                configuration["EmailSettings:Username"],
                configuration["EmailSettings:Password"])
        };
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress("buildoc2@gmail.com", "Buildoc Support"), // Alias para el remitente
            Subject = subject,
            Body = htmlMessage, // Mensaje en formato HTML
            IsBodyHtml = true // Indicar que el cuerpo es HTML
        };
        mailMessage.To.Add(email);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}
