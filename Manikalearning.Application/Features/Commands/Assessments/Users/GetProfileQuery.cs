using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Users
{
    public record GetProfileQuery(Guid UserId) : IRequest<ProfileDto?>;

    public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
    { public GetProfileQueryValidator() => RuleFor(x => x.UserId).NotEmpty(); }
}
