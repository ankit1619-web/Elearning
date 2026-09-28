using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Users
{
    public record GetManageUserRolesQuery(string UserId) : IRequest<ManageUserRolesViewModel?>;
}
