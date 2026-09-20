using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Courses
{
    public record GetCourseByIdQuery(Guid CourseId) : IRequest<ResponseDto<CourseDto>>; 
    public class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Valid Course ID is required to getting data.");
        }
    }
}
