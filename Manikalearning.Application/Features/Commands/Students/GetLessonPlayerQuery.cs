using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetLessonPlayerQuery(
        Guid UserId,
        Guid CourseId,
        Guid LessonId
    ) : IRequest<LessonPlayerViewModel>;
    public class GetLessonPlayerQueryValidator : AbstractValidator<GetLessonPlayerQuery>
    {
        public GetLessonPlayerQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course ID is required.");

            RuleFor(x => x.LessonId)
                .NotEmpty()
                .WithMessage("Lesson ID is required.");
        }
    }
}
