using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.CourseLecture
{
    public record GetCourseLectureByIdQuery(Guid Id) : IRequest<ResponseDto<CourseLectureDto>>;
     
    public class GetCourseLectureByIdQueryValidator : AbstractValidator<GetCourseLectureByIdQuery>
    {
        public GetCourseLectureByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Lecture ID is required.");
        }
    }
}
