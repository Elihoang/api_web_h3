using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using API_WebH3.Models;
using System.Threading.Tasks;
using API_WebH3.Repository;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly IEmailRepository _emailRepository; // Thêm dòng này

    public EmailService(IConfiguration config, IEmailRepository emailRepository)
    {
        _config = config;
        _emailRepository = emailRepository; // Khởi tạo
    }
    
    public async Task SendPasswordResetEmailAsync(string receiverEmail, string subject, string message)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");
        string smtpServer = smtpSettings["Server"];
        string portString = smtpSettings["Port"];
        string username = smtpSettings["Username"];
        string password = smtpSettings["Password"];

        if (string.IsNullOrEmpty(portString))
        {
            AppLogger.LogError("Cổng SMTP không có trong appsettings.json.");
        }
        
        if (!int.TryParse(portString, out int smtpPort))
        {
            AppLogger.LogError($"Cổng SMTP không hợp lệ: '{portString}'. Vui lòng kiểm tra appsettings.json.");
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

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            AppLogger.LogSuccess($"Email đặt lại mật khẩu đã gửi đến {receiverEmail}");

            // Ghi email vào cơ sở dữ liệu
            var emailRecord = new Email
            {
                SenderEmail = username,
                ReceiverEmail = receiverEmail,
                Subject = subject,
                Message = message,
                SourceType = "PasswordReset",
                SentAt = DateTime.UtcNow,
                Status = "Sent"
            };
            await _emailRepository.AddEmailAsync(emailRecord);
        }
        catch (Exception ex)
        {
            // Ghi email với trạng thái thất bại
            var emailRecord = new Email
            {
                SenderEmail = username,
                ReceiverEmail = receiverEmail,
                Subject = subject,
                Message = message,
                SourceType = "PasswordReset",
                SentAt = DateTime.UtcNow,
                Status = "Failed"
            };
            await _emailRepository.AddEmailAsync(emailRecord);
            AppLogger.LogError($"Không gửi được email đặt lại mật khẩu: {ex.Message}");
            throw;
        }
    }
}