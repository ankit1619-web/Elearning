using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Authentication;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Authentication
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ResponseDto<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                // Security Rule: Return success even if user isn't found to prevent email enumeration
                if (user == null)
                {
                    return ResponseDto<bool>.Success(true, "If an account exists with that email, a password reset link has been sent.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Send Reset Email
                await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FullName, token);

                _logger.LogInformation("Password reset token generated for user: {Email}", request.Email);
                return ResponseDto<bool>.Success(true, "If an account exists with that email, a password reset link has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during forgot password for: {Email}", request.Email);
                return ResponseDto<bool>.Failure("An error occurred while processing your request.", new List<string> { ex.Message });
            }
        }
    }
}
