using FluentValidation;
using Manikalearning.Application.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record HasFeatureQuery(Guid UserId, string Feature) : IRequest<bool>;
    public class HasFeatureQueryValidator : AbstractValidator<HasFeatureQuery>
    { public HasFeatureQueryValidator() { RuleFor(x => x.UserId).NotEmpty(); RuleFor(x => x.Feature).NotEmpty().MaximumLength(50); } }
    public class HasFeatureQueryHandler : IRequestHandler<HasFeatureQuery, bool>
    {
        private readonly ISubscriptionChecker _c;
        public HasFeatureQueryHandler(ISubscriptionChecker c) => _c = c;
        public Task<bool> Handle(HasFeatureQuery r, CancellationToken ct) => _c.HasFeatureAsync(r.UserId, r.Feature, ct);
    }

    public record GetMyFeaturesQuery(Guid UserId) : IRequest<HashSet<string>>;
    public class GetMyFeaturesQueryValidator : AbstractValidator<GetMyFeaturesQuery>
    { public GetMyFeaturesQueryValidator() => RuleFor(x => x.UserId).NotEmpty(); }
    public class GetMyFeaturesQueryHandler : IRequestHandler<GetMyFeaturesQuery, HashSet<string>>
    {
        private readonly ISubscriptionChecker _c;
        public GetMyFeaturesQueryHandler(ISubscriptionChecker c) => _c = c;
        public Task<HashSet<string>> Handle(GetMyFeaturesQuery r, CancellationToken ct) => _c.GetFeaturesAsync(r.UserId, ct);
    }
}
