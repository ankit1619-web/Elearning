using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetPerformanceQuery(Guid UserId, int Days = 30) : IRequest<PerformanceDto>;

    public class GetPerformanceQueryValidator : AbstractValidator<GetPerformanceQuery>
    {
        public static readonly int[] AllowedDays = { 7, 30, 90, 365 };

        public GetPerformanceQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Days).Must(d => AllowedDays.Contains(d)).WithMessage("Days must be 7, 30, 90 or 365.");
        }
    }
}
