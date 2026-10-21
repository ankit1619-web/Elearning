using Manikalearning.Domain.Entities;

namespace Manikalearning.Application.Interface
{
    
    public interface IEmailService
    {
        Task SendAsync(ContactMessage m, CancellationToken ct);
        Task SendEmailAsync(string toEmail, string subject, string body, string? replyTo = null);
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken);
    }
}
