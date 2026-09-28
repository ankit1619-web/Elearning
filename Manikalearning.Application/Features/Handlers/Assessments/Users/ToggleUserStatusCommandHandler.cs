using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public ToggleUserStatusCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;  

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
