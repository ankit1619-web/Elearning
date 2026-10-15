using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record CheckLectureAccessQuery(Guid UserId, Guid LectureId) : IRequest<CourseAccessDto>;
    public class CheckLectureAccessQueryValidator : AbstractValidator<CheckLectureAccessQuery>
    { public CheckLectureAccessQueryValidator() { RuleFor(x => x.UserId).NotEmpty(); RuleFor(x => x.LectureId).NotEmpty(); } }
    public class CheckLectureAccessQueryHandler : IRequestHandler<CheckLectureAccessQuery, CourseAccessDto>
    {
        private readonly ISubscriptionChecker _c;
        public CheckLectureAccessQueryHandler(ISubscriptionChecker c) => _c = c;
        public Task<CourseAccessDto> Handle(CheckLectureAccessQuery r, CancellationToken ct) => _c.CheckLectureAsync(r.UserId, r.LectureId, ct);
    }

}
