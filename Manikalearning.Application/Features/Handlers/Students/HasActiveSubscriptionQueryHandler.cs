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
        private readonly ISubscriptionChecker _c;
        public HasActiveSubscriptionQueryHandler(ISubscriptionChecker c) => _c = c;
        public Task<bool> Handle(HasActiveSubscriptionQuery r, CancellationToken ct) => _c.HasActiveAsync(r.UserId, ct);
    }
}
