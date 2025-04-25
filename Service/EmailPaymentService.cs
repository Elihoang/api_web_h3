using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace API_WebH3.Services;

public class EmailPaymentService
{
    private readonly IConfiguration _configuration;

    public EmailPaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            Console.WriteLine($"🔹 Bắt đầu gửi email đến: {toEmail}");

            // Log cấu hình SMTP để kiểm tra
            var smtpServer = _configuration["SmtpSettings:Server"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var smtpUser = _configuration["SmtpSettings:Username"];
            var smtpPass = _configuration["SmtpSettings:Password"];
            var senderName = _configuration["SmtpSettings:SenderName"];
            var senderEmail = _configuration["SmtpSettings:SenderEmail"];

            Console.WriteLine($"🔹 Cấu hình SMTP: Server={smtpServer}, Port={smtpPort}, Username={smtpUser}, SenderName={senderName}, SenderEmail={senderEmail}");

            // Tạo MimeMessage
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            email.Body = bodyBuilder.ToMessageBody();

            // Kiểm tra cấu hình
            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                throw new InvalidOperationException("Cấu hình SMTP không đầy đủ. Vui lòng kiểm tra SmtpSettings.");
            }

            // Kết nối SMTP
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);

            // Gửi email
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            Console.WriteLine($"✅ Email đã gửi thành công đến {toEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi gửi email đến {toEmail}: {ex.Message}");
            throw;
        }
    }
}