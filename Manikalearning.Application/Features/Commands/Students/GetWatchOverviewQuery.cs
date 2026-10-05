using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetWatchOverviewQuery(Guid UserId,Guid CourseId) : IRequest<WatchOverviewViewModel>;
    public class GetWatchOverviewQueryValidator : AbstractValidator<GetWatchOverviewQuery>
    {
        public GetWatchOverviewQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course ID is required.");
        }
    }
}
