using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Doubts
{
    public record GetStaffDoubtsQuery(string? Status = null, int Page = 1, int PageSize = 15) : IRequest<StaffDoubtListDto>;

    public class GetStaffDoubtsQueryValidator : AbstractValidator<GetStaffDoubtsQuery>
    {
        public GetStaffDoubtsQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 50);
            RuleFor(x => x.Status).MaximumLength(50);
        }
    }
}
