using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Courses
{
    public record CreateCourseCommand(string CourseName, string Description, decimal Price)
    : IRequest<ResponseDto<Guid>>;

    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.CourseName)
                .NotEmpty().WithMessage("Course Name is required.")
                .MinimumLength(3).WithMessage("Course Name must be at least 3 characters long.")
                .MaximumLength(150).WithMessage("Course Name cannot exceed 150 characters.");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.")
                .LessThan(100000).WithMessage("Price seems unusually high. Max allowed is 99,999.");
        }
    }
}
