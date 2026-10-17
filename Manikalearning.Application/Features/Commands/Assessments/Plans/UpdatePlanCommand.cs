using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{
    public record UpdatePlanCommand(
         Guid PlanId,
         string PlanName,
         decimal Price,
         int DurationDays,
         string? Description,
         bool IsActive, List<string> Features
     ) : IRequest<ResponseDto<bool>>;

    public class UpdatePlanCommandValidator : AbstractValidator<UpdatePlanCommand>
    {
        public UpdatePlanCommandValidator()
        {
            RuleFor(x => x.PlanId).NotEmpty().WithMessage("Plan Id is required.");
            RuleFor(x => x.PlanName).NotEmpty().WithMessage("Plan Name is required.").MaximumLength(150);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be 0 or greater.");
            RuleFor(x => x.DurationDays).GreaterThan(0).WithMessage("Duration must be at least 1 day."); 
        }
    }
}
