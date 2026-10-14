using FluentValidation;
using Manikalearning.Application.Interface;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record GetAccessibleCourseIdsQuery(Guid UserId) : IRequest<HashSet<Guid>>;

    public class GetAccessibleCourseIdsQueryValidator : AbstractValidator<GetAccessibleCourseIdsQuery>
    {
        public GetAccessibleCourseIdsQueryValidator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public class GetAccessibleCourseIdsQueryHandler : IRequestHandler<GetAccessibleCourseIdsQuery, HashSet<Guid>>
    {
        private readonly ISubscriptionChecker _checker;
        public GetAccessibleCourseIdsQueryHandler(ISubscriptionChecker checker) => _checker = checker;
        public Task<HashSet<Guid>> Handle(GetAccessibleCourseIdsQuery req, CancellationToken ct) => _checker.AccessibleCourseIdsAsync(req.UserId, ct);
    }
}
