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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseDto<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            { 
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return ResponseDto<bool>.Failure("An account with this email address already exists.");
                }
                 
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = request.Email,
                    PhoneNumber = request.Mobile,
                    Mobile = request.Mobile,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                 
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResponseDto<bool>.Failure("User registration failed.", errors);
                }
                 
                string roleToAssign = string.IsNullOrWhiteSpace(request.RoleName) ? "Student" : request.RoleName;
                if (!await _roleManager.RoleExistsAsync(roleToAssign))
                {
                    await _roleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = roleToAssign });
                }

                await _userManager.AddToRoleAsync(user, roleToAssign);

                _logger.LogInformation("User {Email} registered successfully with role {Role}.", user.Email, roleToAssign);
                return ResponseDto<bool>.Success(true, "Registration successful! You can now log in.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for email: {Email}", request.Email);
                return ResponseDto<bool>.Failure("An unexpected error occurred during registration.", new List<string> { ex.Message });
            }
        }
    }
}
