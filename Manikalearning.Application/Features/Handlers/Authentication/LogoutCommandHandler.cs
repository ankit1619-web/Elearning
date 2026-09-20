using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Authentication;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Authentication
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResponseDto<bool>>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            SignInManager<User> signInManager,
            ILogger<LogoutCommandHandler> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully.");
                return ResponseDto<bool>.Success(true, "Logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout.");
                return ResponseDto<bool>.Failure("An error occurred while logging out.", new List<string> { ex.Message });
            }
        }
    }
}
