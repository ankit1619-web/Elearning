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
    public record GetCourseSectionByIdQuery(Guid SectionId) : IRequest<ResponseDto<CourseSectionDto>>;

    public class GetCourseSectionByIdQueryValidator : AbstractValidator<GetCourseSectionByIdQuery>
    {
        public GetCourseSectionByIdQueryValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Valid Section ID is required.");
        }
    }
}
