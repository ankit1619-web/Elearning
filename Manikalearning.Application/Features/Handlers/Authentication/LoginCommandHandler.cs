using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Authentication;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Authentication
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseDto<bool>>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<LoginCommandHandler> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ResponseDto<bool>.Failure("Invalid email or password.");
                }

                if (!user.IsActive)
                {
                    return ResponseDto<bool>.Failure("Your account has been deactivated. Please contact support.");
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    request.Password,
                    request.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", request.Email);
                    return ResponseDto<bool>.Success(true, "Login successful. Redirecting...");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} account locked out.", request.Email);
                    return ResponseDto<bool>.Failure("Account locked due to multiple failed attempts. Try again later.");
                }

                return ResponseDto<bool>.Failure("Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
                return ResponseDto<bool>.Failure("An unexpected error occurred during login.", new List<string> { ex.Message });
            }
        }
    }
}
