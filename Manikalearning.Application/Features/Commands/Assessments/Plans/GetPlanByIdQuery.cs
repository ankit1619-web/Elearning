using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{ 
    public record GetPlanByIdQuery(Guid PlanId) : IRequest<ResponseDto<PlanDto>>;
    public class GetPlanByIdQueryValidator : AbstractValidator<GetPlanByIdQuery>
    {
        public GetPlanByIdQueryValidator()
        {
            RuleFor(x => x.PlanId)
                .NotEmpty().WithMessage("Valid Plan ID is required to getting data.");
        }
    }
}
