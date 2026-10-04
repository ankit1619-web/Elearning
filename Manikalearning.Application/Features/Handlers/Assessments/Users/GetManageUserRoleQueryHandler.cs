using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public class GetManageUserRoleQueryHandler : IRequestHandler<GetManageUserRoleQuery, ManageUserRoleViewModel?>
    {
        private readonly UserManager<User> _userManager; // ApplicationUser -> User
        private readonly RoleManager<Role> _roleManager;   // IdentityRole -> Role

        public GetManageUserRoleQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ManageUserRoleViewModel?> Handle(GetManageUserRoleQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return null;

            var userRoles = await _userManager.GetRolesAsync(user);
            var currentRoleName = userRoles.FirstOrDefault();

            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
            var currentRole = roles.FirstOrDefault(r => r.Name == currentRoleName);

            return new ManageUserRoleViewModel
            {
                UserId = user.Id,
                FullName = user.FullName ?? user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                SelectedRoleId = currentRole?.Id,
                AvailableRoles = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name!
                }).ToList()
            };
        }
    }
}
