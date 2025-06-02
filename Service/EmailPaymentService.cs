using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Services
{
    public class EmailPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailRepository _emailRepository; // Thêm dòng này

        public EmailPaymentService(IConfiguration configuration, IEmailRepository emailRepository)
        {
            _configuration = configuration;
            _emailRepository = emailRepository; // Khởi tạo
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                Console.WriteLine($"🔹 Bắt đầu gửi email đến: {toEmail}");

                var smtpServer = _configuration["SmtpSettings:Server"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                var smtpUser = _configuration["SmtpSettings:Username"];
                var smtpPass = _configuration["SmtpSettings:Password"];
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                email.Body = bodyBuilder.ToMessageBody();

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    throw new InvalidOperationException("Cấu hình SMTP không đầy đủ. Vui lòng kiểm tra SmtpSettings.");
                }

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpUser, smtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                Console.WriteLine($"Email đã gửi thành công đến {toEmail}");

                // Ghi email vào cơ sở dữ liệu
                var emailRecord = new Email
                {
                    SenderEmail = senderEmail,
                    ReceiverEmail = toEmail,
                    Subject = subject,
                    Message = body,
                    SourceType = "Payment",
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
                    SenderEmail = _configuration["SmtpSettings:SenderEmail"],
                    ReceiverEmail = toEmail,
                    Subject = subject,
                    Message = body,
                    SourceType = "Payment",
                    SentAt = DateTime.UtcNow,
                    Status = "Failed"
                };
                await _emailRepository.AddEmailAsync(emailRecord);
                Console.WriteLine($"Lỗi khi gửi email đến {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}