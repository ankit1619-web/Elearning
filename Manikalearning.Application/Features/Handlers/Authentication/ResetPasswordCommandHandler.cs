using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Authentication;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Authentication
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResponseDto<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ResponseDto<bool>.Failure("Invalid request.");
                }

                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successfully for user: {Email}", request.Email);
                    return ResponseDto<bool>.Success(true, "Password reset successfully. Please log in with your new password.");
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto<bool>.Failure("Failed to reset password.", errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {Email}", request.Email);
                return ResponseDto<bool>.Failure("An unexpected error occurred while resetting password.", new List<string> { ex.Message });
            }
        }
    }
}
