using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.ExamCourse
{
    public record GetExamsByCourseIdQuery(Guid CourseId) : IRequest<ResponseDto<List<ExamCourseDto>>>;
     
    public class GetExamsByCourseIdQueryValidator : AbstractValidator<GetExamsByCourseIdQuery>
    {
        public GetExamsByCourseIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required.");
        }
    }
}
