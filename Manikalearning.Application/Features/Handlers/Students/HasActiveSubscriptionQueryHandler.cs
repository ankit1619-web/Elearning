using FluentValidation;
using Manikalearning.Application.Interface;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record HasActiveSubscriptionQuery(Guid UserId) : IRequest<bool>;

    public class HasActiveSubscriptionQueryValidator : AbstractValidator<HasActiveSubscriptionQuery>
    {
        public HasActiveSubscriptionQueryValidator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public class HasActiveSubscriptionQueryHandler : IRequestHandler<HasActiveSubscriptionQuery, bool>
    {
        private readonly ISubscriptionChecker _checker;
        public HasActiveSubscriptionQueryHandler(ISubscriptionChecker checker) => _checker = checker;
        public Task<bool> Handle(HasActiveSubscriptionQuery req, CancellationToken ct) => _checker.HasActiveAsync(req.UserId, ct);
    }
}
