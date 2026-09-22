using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Papers
{
    public record GetPaperByIdQuery(Guid PaperId) : IRequest<ResponseDto<PaperDto>>;
    public class GetPaperByIdQueryValidator : AbstractValidator<GetPaperByIdQuery>
    {
        public GetPaperByIdQueryValidator()
        {
            RuleFor(x => x.PaperId)
                .NotEmpty().WithMessage("Paper id is required.");
        }
    }
}
