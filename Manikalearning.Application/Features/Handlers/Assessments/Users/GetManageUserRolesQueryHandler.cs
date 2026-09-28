using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public class GetManageUserRolesQueryHandler : IRequestHandler<GetManageUserRolesQuery, ManageUserRolesViewModel?>
    {
        private readonly UserManager<User> _userManager; // ApplicationUser -> User
        private readonly RoleManager<Role> _roleManager;   // IdentityRole -> Role

        public GetManageUserRolesQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ManageUserRolesViewModel?> Handle(GetManageUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return null;

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                FullName = user.FullName ?? user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

            foreach (var role in roles)
            {
                model.UserRoles.Add(new UserRoleSelectionDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name!,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name!)
                });
            }

            return model;
        }
    }
}
