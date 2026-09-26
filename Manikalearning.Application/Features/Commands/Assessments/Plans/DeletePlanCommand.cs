using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{ 
    public record DeletePlanCommand(Guid PlanId) : IRequest<ResponseDto<bool>>;
    public class DeletePlanCommandValidator : AbstractValidator<DeletePlanCommand>
    {
        public DeletePlanCommandValidator()
        {
            RuleFor(x => x.PlanId)
                .NotEmpty().WithMessage("Valid Plan ID is required to perform deletion.");
        }
    }
}
