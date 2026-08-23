using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Models
{
    public class AccountVM
    {
    }
    public class LoginVM
    {
        [Required]
        public string EmailOrMobile { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }


    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool AcceptTerms { get; set; }
    }


    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }


    public class ResetPasswordVM
    {
        public string UserId { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class ContactVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Mobile { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
