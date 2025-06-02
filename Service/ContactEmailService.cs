using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Services
{
    public class ContactEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailRepository _emailRepository; // Thêm dòng này
        private readonly string _templatePath;

        public ContactEmailService(IConfiguration config, IWebHostEnvironment env, IEmailRepository emailRepository)
        {
            _config = config;
            _templatePath = Path.Combine(env.WebRootPath, "templates", "ContactEmailTemplate.html");
            _emailRepository = emailRepository; // Khởi tạo
        }

        public async Task SendEmailAsync(string senderEmail, string subject, string message)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            string smtpServer = smtpSettings["Server"];
            string portString = smtpSettings["Port"];
            string username = smtpSettings["Username"];
            string password = smtpSettings["Password"];
            string receiverEmail = smtpSettings["ReceiverEmail"];

            if (string.IsNullOrEmpty(portString) || !int.TryParse(portString, out int smtpPort))
            {
                throw new ArgumentException($"Cổng SMTP không hợp lệ: '{portString}'. Vui lòng kiểm tra appsettings.json.");
            }

            // Đọc template HTML
            if (!File.Exists(_templatePath))
            {
                throw new FileNotFoundException("Không tìm thấy template email liên hệ", _templatePath);
            }

            string template = await File.ReadAllTextAsync(_templatePath);
            string emailBody = template
                .Replace("{senderEmail}", senderEmail)
                .Replace("{subject}", subject)
                .Replace("{message}", message);

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
                Body = emailBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(receiverEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Đã gửi email liên hệ đến {receiverEmail} từ {senderEmail}");

                // Ghi email vào cơ sở dữ liệu
                var emailRecord = new Email
                {
                    SenderEmail = senderEmail,
                    ReceiverEmail = receiverEmail,
                    Subject = subject,
                    Message = emailBody,
                    SourceType = "Contact",
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
                    SenderEmail = senderEmail,
                    ReceiverEmail = receiverEmail,
                    Subject = subject,
                    Message = emailBody,
                    SourceType = "Contact",
                    SentAt = DateTime.UtcNow,
                    Status = "Failed"
                };
                await _emailRepository.AddEmailAsync(emailRecord);
                Console.WriteLine($"Không gửi được email liên hệ: {ex.Message}");
                throw;
            }
        }
    }
}