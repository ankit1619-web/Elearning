using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record CheckMediaAccessQuery(Guid UserId, string FileName) : IRequest<CourseAccessDto>;
    public class CheckMediaAccessQueryValidator : AbstractValidator<CheckMediaAccessQuery>
    { public CheckMediaAccessQueryValidator() { RuleFor(x => x.UserId).NotEmpty(); RuleFor(x => x.FileName).NotEmpty().MaximumLength(300); } }
    public class CheckMediaAccessQueryHandler : IRequestHandler<CheckMediaAccessQuery, CourseAccessDto>
    {
        private readonly ISubscriptionChecker _c;
        public CheckMediaAccessQueryHandler(ISubscriptionChecker c) => _c = c;
        public Task<CourseAccessDto> Handle(CheckMediaAccessQuery r, CancellationToken ct) => _c.CheckMediaAsync(r.UserId, r.FileName, ct);
    }
}
