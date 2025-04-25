using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string senderEmail, string subject, string message)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");

        string smtpServer = smtpSettings["Server"];
        string portString = smtpSettings["Port"];
        string username = smtpSettings["Username"];
        string password = smtpSettings["Password"];
        string receiverEmail = smtpSettings["ReceiverEmail"];

        // ✅ Kiểm tra giá trị Port có rỗng không
        if (string.IsNullOrEmpty(portString))
        {
            throw new ArgumentException("❌ SMTP Port is missing in appsettings.json.");
        }

        // ✅ Chuyển đổi Port từ string -> int
        if (!int.TryParse(portString, out int smtpPort))
        {
            throw new ArgumentException($"❌ Invalid SMTP Port: '{portString}'. Please check appsettings.json.");
        }

        var smtpClient = new SmtpClient(smtpServer)
        {
            Port = smtpPort,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(receiverEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendPasswordResetEmailAsync(string receiverEmail, string subject, string message)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");
        string smtpServer = smtpSettings["Server"];
        string portString = smtpSettings["Port"];
        string username = smtpSettings["Username"];
        string password = smtpSettings["Password"];

        // ✅ Kiểm tra giá trị Port có rỗng không
        if (string.IsNullOrEmpty(portString))
        {
            throw new ArgumentException(" SMTP Port is missing in appsettings.json.");
        }

        // ✅ Chuyển đổi Port từ string -> int
        if (!int.TryParse(portString, out int smtpPort))
        {
            throw new ArgumentException($" Invalid SMTP Port: '{portString}'. Please check appsettings.json.");
        }

        var smtpClient = new SmtpClient(smtpServer)
        {
            Port = smtpPort,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(username),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(receiverEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }
}