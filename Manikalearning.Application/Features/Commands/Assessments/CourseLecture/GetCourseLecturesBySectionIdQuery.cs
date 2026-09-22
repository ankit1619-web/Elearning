using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.CourseLecture
{
    public record GetCourseLecturesBySectionIdQuery(Guid SectionId) : IRequest<ResponseDto<List<CourseLectureDto>>>;
     
    public class GetCourseLecturesBySectionIdQueryValidator : AbstractValidator<GetCourseLecturesBySectionIdQuery>
    {
        public GetCourseLecturesBySectionIdQueryValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.");
        }
    }
}
