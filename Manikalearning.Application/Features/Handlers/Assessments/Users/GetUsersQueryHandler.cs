using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly UserManager<User> _userManager;

        public GetUsersQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(term)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.Mobile != null && u.Mobile.Contains(term))
                );
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync(cancellationToken);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Mobile = user.Mobile ?? user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified || user.EmailConfirmed,
                    IsMobileVerified = user.IsMobileVerified || user.PhoneNumberConfirmed,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return userDtos;
        }
    }
}
