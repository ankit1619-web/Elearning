using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Courses
{
    public record UpdateCourseCommand(
     Guid CourseId,
     string CourseName,
     string Description,
     decimal Price,
     bool IsPublished
     ) : IRequest<ResponseDto<Guid>>;
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required for updating.");

            RuleFor(x => x.CourseName)
                .NotEmpty().WithMessage("Course Name is required.")
                .MinimumLength(3).WithMessage("Course Name must be at least 3 characters long.")
                .MaximumLength(150).WithMessage("Course Name cannot exceed 150 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
        }
    }
}
