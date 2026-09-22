using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.CourseLecture
{
    public record DeleteCourseLectureCommand(Guid Id) : IRequest<ResponseDto<bool>>;
     
    public class DeleteCourseLectureCommandValidator : AbstractValidator<DeleteCourseLectureCommand>
    {
        public DeleteCourseLectureCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Lecture ID is required.");
        }
    }
}
