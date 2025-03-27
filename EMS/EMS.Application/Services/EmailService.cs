using System.Net.Mail;
using System.Net;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentException("Recipient email cannot be null or empty.", nameof(toEmail));
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentException("Email subject cannot be empty.", nameof(subject));
            if (string.IsNullOrEmpty(body))
                throw new ArgumentException("Email body cannot be empty.", nameof(body));
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? throw new Exception("SMTP Host is not configured.");
                var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out int port) ? port : throw new Exception("Invalid SMTP Port.");
                var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? throw new Exception("SMTP User is not configured.");
                var smtpPass = _configuration["EmailSettings:SmtpPass"] ?? throw new Exception("SMTP Password is not configured.");
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? throw new Exception("Sender Email is not configured.");
                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Email sending failed", ex);
            }
        }
        public async Task SendEmailUser(User user, string resetLink)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var subject = "Your Account Has Been Created - Set Your Password";
            var body = $@"
                <p>Hello <b>{user.FirstName}</b>,</p>
                <p>Your account has been created successfully.</p>
                <p>Please click the link below to set your new password:</p>
                <p>{resetLink}</p>
                <p><b>Note:</b> This link is valid for 1 hour.</p>
                <p>If you did not request this, please ignore this email.</p>
                <p>Thanks,<br>EMS Team</p>";
            await SendEmailAsync(user.Email, subject, body);
        }
        public async Task ResetPasswordEmail(User user, string resetLink)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var subject = "Reset Your Password";
            var body = $@"
                <p>Hello <b>{user.FirstName}</b>,</p>
                <p>You requested to reset your password.</p>
                <p>{resetLink}</p>
                <p><b>Note:</b> This link will expire in 60 minutes.</p>";
            await SendEmailAsync(user.Email, subject, body);
        }
    }
}