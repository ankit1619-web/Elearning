using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Manikalearning.Application.Services
{
    public class SmtpSettings
    {
        public string Server { get; set; } = "";
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = "";
        public string SenderEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public string AppUrl { get; set; } = "";
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp; 
        private readonly ILogger<EmailService> _logger;

        public EmailService(SmtpSettings smtp, ILogger<EmailService> logger)
        {
            _smtp = smtp; 
            _logger = logger;
        }

        public async Task SendAsync(ContactMessage m, CancellationToken ct)
        { 
            try
            {
                await SendEmailAsync(_smtp.AdminEmail, $"[Contact] {Clean(m.Subject)}", AdminBody(m), replyTo: m.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contact admin mail failed for message {Id}", m.ContactMessageId);
            } 
            try
            {
                await SendEmailAsync(m.Email, "We received your message - Manika Learning", ConfirmBody(m));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contact confirmation mail failed for message {Id}", m.ContactMessageId);
            }
        }
         
        private static string Clean(string s) => (s ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
        private static string H(string s) => WebUtility.HtmlEncode(s ?? "");

        private static string AdminBody(ContactMessage m) => $@"
          <div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e5e7eb;border-radius:12px;overflow:hidden'>
            <div style='background:#4f46e5;color:#fff;padding:16px 20px;font-size:18px;font-weight:700'>New contact message</div>
            <div style='padding:20px;color:#111827'>
              <p style='margin:0 0 6px'><b>Name:</b> {H(m.Name)}</p>
              <p style='margin:0 0 6px'><b>Email:</b> <a href='mailto:{H(m.Email)}'>{H(m.Email)}</a></p>
              <p style='margin:0 0 6px'><b>Subject:</b> {H(m.Subject)}</p>
              <p style='margin:0 0 14px;color:#6b7280;font-size:12px'>Received {m.CreatedAt:dd MMM yyyy, HH:mm} UTC &middot; IP {H(m.IpAddress ?? "-")}</p>
              <div style='background:#f9fafb;border-radius:8px;padding:14px;white-space:pre-wrap'>{H(m.Message)}</div>
            </div>
          </div>";
       
               private static string ConfirmBody(ContactMessage m) => $@"
              <div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e5e7eb;border-radius:12px;overflow:hidden'>
                <div style='background:#4f46e5;color:#fff;padding:16px 20px;font-size:18px;font-weight:700'>Thanks for contacting us</div>
                <div style='padding:20px;color:#111827'>
                  <p>Hi {H(m.Name)},</p>
                  <p>We have received your message and our team will reply to you soon.</p>
                  <p style='margin:14px 0 6px;color:#6b7280;font-size:13px'>Your message:</p>
                  <div style='background:#f9fafb;border-radius:8px;padding:14px;white-space:pre-wrap'><b>{H(m.Subject)}</b><br/><br/>{H(m.Message)}</div>
                  <p style='margin-top:18px'>Regards,<br/>Manika Learning Support</p>
                </div>
              </div>";

        public async Task SendEmailAsync(string toEmail, string subject, string body, string? replyTo = null)
        {
            try
            {
                var smtpServer = _smtp.Server;
                var smtpPort = _smtp.Port;
                var senderEmail = _smtp.SenderEmail;
                var senderPassword = _smtp.Password;
                var senderName = string.IsNullOrEmpty(_smtp.SenderName) ? "Manika Learning Academy" : _smtp.SenderName;

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                if (!string.IsNullOrEmpty(replyTo))
                {
                    mailMessage.ReplyToList.Add(new MailAddress(replyTo));
                }

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken)
        {
            var resetLink = $"{_smtp.AppUrl}/Account/ResetPassword?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(resetToken)}";

            var subject = "Reset Your Password - Manika Learning Academy";
            var body = $@"
                <h3>Hello {userName},</h3>
                <p>You requested a password reset for your account.</p>
                <p>Please click the link below to reset your password:</p>
                <p><a href='{resetLink}' style='padding: 10px 15px; background-color: #0d6efd; color: white; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}