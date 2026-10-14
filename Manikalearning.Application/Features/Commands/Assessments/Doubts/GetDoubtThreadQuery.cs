using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Doubts
{
    public record GetDoubtThreadQuery(Guid? UserId, Guid DoubtId) : IRequest<DoubtThreadDto?>;

    public class GetDoubtThreadQueryValidator : AbstractValidator<GetDoubtThreadQuery>
    {
        public GetDoubtThreadQueryValidator() => RuleFor(x => x.DoubtId).NotEmpty();
    }
}
