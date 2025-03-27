using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailUser(User user, string resetLink);
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task ResetPasswordEmail(User user, string resetLink);
    }
}