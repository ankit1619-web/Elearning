using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public class GetStudentDashboardQuery : IRequest<StudentDashboardViewModel>
    {
        public Guid UserId { get; set; } 
    }
    public class GetStudentDashboardQueryValidator : AbstractValidator<GetStudentDashboardQuery>
    {
        public GetStudentDashboardQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Invalid User.");
        }
    }
}
