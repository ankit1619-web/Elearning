using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Users
{
    public record GetManageUserRoleQuery(Guid UserId) : IRequest<ManageUserRoleViewModel?>;
}
