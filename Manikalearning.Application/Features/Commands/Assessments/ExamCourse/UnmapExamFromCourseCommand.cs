using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.ExamCourse
{
    public record UnmapExamFromCourseCommand(
          Guid ExamId,
          Guid CourseId
      ) : IRequest<ResponseDto<bool>>;
     
    public class UnmapExamFromCourseCommandValidator : AbstractValidator<UnmapExamFromCourseCommand>
    {
        public UnmapExamFromCourseCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam ID is required.");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required.");
        }
    }
}
