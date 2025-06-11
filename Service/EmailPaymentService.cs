using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Services
{
    public class EmailPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailRepository _emailRepository;

        public EmailPaymentService(IConfiguration configuration, IEmailRepository emailRepository)
        {
            _configuration = configuration;
            _emailRepository = emailRepository;
            ValidateSmtpConfiguration();
        }

        private void ValidateSmtpConfiguration()
        {
            var requiredKeys = new[] { "SmtpSettings:Server", "SmtpSettings:Port", "SmtpSettings:Username", 
                                      "SmtpSettings:Password", "SmtpSettings:SenderName", "SmtpSettings:SenderEmail" };
            foreach (var key in requiredKeys)
            {
                if (string.IsNullOrEmpty(_configuration[key]))
                {
                    AppLogger.LogError($"Missing SMTP configuration: {key}");
                }
            }
            if (!int.TryParse(_configuration["SmtpSettings:Port"], out _))
            {
                throw new InvalidOperationException($"Invalid SMTP port: {_configuration["SmtpSettings:Port"]}");
            }
            AppLogger.LogSuccess("SMTP configuration validated successfully.");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                AppLogger.LogInfo($"🔹 Starting to send email to: {toEmail}, Subject: {subject}");

                var smtpServer = _configuration["SmtpSettings:Server"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                var smtpUser = _configuration["SmtpSettings:Username"];
                var smtpPass = _configuration["SmtpSettings:Password"];
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];

                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                AppLogger.LogSuccess($"Connecting to SMTP server: {smtpServer}:{smtpPort}");
                await smtpClient.SendMailAsync(mailMessage);
                AppLogger.LogSuccess($"Email sent successfully to {toEmail}");

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
                AppLogger.LogError($"Failed to send email to {toEmail}: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
    }
}