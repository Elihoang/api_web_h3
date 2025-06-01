using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace API_WebH3.Services
{
    public class ContactEmailService
    {
        private readonly IConfiguration _config;
        private readonly string _templatePath;

        public ContactEmailService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _templatePath = Path.Combine(env.WebRootPath, "templates", "ContactEmailTemplate.html");
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
                throw new ArgumentException($"Invalid SMTP Port: '{portString}'. Please check appsettings.json.");
            }

            // Đọc template HTML
            if (!File.Exists(_templatePath))
            {
                throw new FileNotFoundException("Contact email template not found", _templatePath);
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

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"Contact email sent to {receiverEmail} from {senderEmail}");
        }
    }
}