using Manikalearning.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Users
{
    public record UpdateUserRoleCommand(Guid UserId, Guid? SelectedRoleId) : IRequest<ResponseDto<bool>>;
}
