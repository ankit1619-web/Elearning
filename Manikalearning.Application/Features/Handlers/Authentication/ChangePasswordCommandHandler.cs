using System.Security.Claims;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Authentication;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
namespace Manikalearning.Application.Features.Handlers.Authentication
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResponseDto<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseDto<bool>.Failure("User is not authenticated.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ResponseDto<bool>.Failure("User not found.");
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserId} changed their password successfully.", userId);
                    return ResponseDto<bool>.Success(true, "Password changed successfully.");
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto<bool>.Failure("Failed to change password.", errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for logged-in user.");
                return ResponseDto<bool>.Failure("An unexpected error occurred while changing password.", new List<string> { ex.Message });
            }
        }
    }
}
