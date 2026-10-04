using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, ResponseDto<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UpdateUserRoleCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return ResponseDto<bool>.Failure("User not found.");
            }
             
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                user.UserName = !string.IsNullOrWhiteSpace(user.Email) ? user.Email : user.Id.ToString();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return ResponseDto<bool>.Failure($"Failed to update username: {errors}");
                }
            }
             
            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }
             
            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    return ResponseDto<bool>.Failure(errors);
                }
            }
             
            if (request.SelectedRoleId.HasValue && request.SelectedRoleId != Guid.Empty)
            {
                var role = await _roleManager.FindByIdAsync(request.SelectedRoleId.Value.ToString());
                if (role != null && !string.IsNullOrEmpty(role.Name))
                {
                    var addResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!addResult.Succeeded)
                    {
                        var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                        return ResponseDto<bool>.Failure(errors);
                    }
                }
            }

            return ResponseDto<bool>.Success(true, "User role updated successfully.");
        }
    }
}