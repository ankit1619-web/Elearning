using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Courses
{
    public record DeleteCourseCommand(Guid CourseId) : IRequest<ResponseDto<bool>>;
    public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Valid Course ID is required to perform deletion.");
        }
    }
}
