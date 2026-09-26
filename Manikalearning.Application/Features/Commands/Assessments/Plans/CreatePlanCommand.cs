using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{
    public record CreatePlanCommand(string PlanName, string Description, decimal Price,int DurationDays,bool IsActive)
    : IRequest<ResponseDto<Guid>>;

    public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
    {
        public CreatePlanCommandValidator()
        {
            RuleFor(x => x.PlanName).NotEmpty().WithMessage("Plan Name is required.").MaximumLength(150);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be 0 or greater.");
            RuleFor(x => x.DurationDays).GreaterThan(0).WithMessage("Duration must be at least 1 day."); 
        }
    }
}
