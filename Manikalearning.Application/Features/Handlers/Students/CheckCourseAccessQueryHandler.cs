using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record CheckCourseAccessQuery(Guid UserId, Guid CourseId) : IRequest<CourseAccessDto>;

    public class CheckCourseAccessQueryValidator : AbstractValidator<CheckCourseAccessQuery>
    {
        public CheckCourseAccessQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CourseId).NotEmpty();
        }
    }  
    public class CheckCourseAccessQueryHandler : IRequestHandler<CheckCourseAccessQuery, CourseAccessDto>
    {
        private readonly ISubscriptionChecker _c;
        public CheckCourseAccessQueryHandler(ISubscriptionChecker c) => _c = c;
        public Task<CourseAccessDto> Handle(CheckCourseAccessQuery r, CancellationToken ct) => _c.CheckCourseAsync(r.UserId, r.CourseId, ct);
    }
}
