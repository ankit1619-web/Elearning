using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Coursesection
{
    public record GetCourseSectionsByCourseIdQuery(Guid CourseId) : IRequest<ResponseDto<List<CourseSectionDto>>>;

    public class GetCourseSectionsByCourseIdQueryValidator : AbstractValidator<GetCourseSectionsByCourseIdQuery>
    {
        public GetCourseSectionsByCourseIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Valid Course ID is required to fetch sections.");
        }
    }
}
