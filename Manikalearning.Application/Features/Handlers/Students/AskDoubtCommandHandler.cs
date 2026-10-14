using FluentValidation;
using FluentValidation.Results;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record AskDoubtCommand(Guid UserId, Guid? CourseId, string QuestionText) : IRequest<Guid>;

    public class AskDoubtCommandValidator : AbstractValidator<AskDoubtCommand>
    {
        public AskDoubtCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.QuestionText).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please write your doubt.")
                .Must(t => t.Trim().Length >= 10).WithMessage("Please describe your doubt in at least 10 characters.")
                .Must(t => t.Trim().Length <= 1000).WithMessage("Your doubt can be at most 1000 characters.");
        }
    }

    public class AskDoubtCommandHandler : IRequestHandler<AskDoubtCommand, Guid>
    {
        private const int MaxPerHour = 5;
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<Course> _courses;
        public AskDoubtCommandHandler(IRepository<DoubtEntity> doubts, IRepository<Course> courses) { _doubts = doubts; _courses = courses; }

        public async Task<Guid> Handle(AskDoubtCommand req, CancellationToken ct)
        {
            if (req.CourseId.HasValue &&
                !await _courses.Entity().AnyAsync(c => c.CourseId == req.CourseId && c.IsPublished, ct))
                throw new ValidationException(new[] { new ValidationFailure("CourseId", "Please choose a valid course.") });
             
            var since = DateTime.UtcNow.AddHours(-1);
            var recent = await _doubts.Entity().CountAsync(d => d.UserId == req.UserId && d.CreatedAt >= since, ct);
            if (recent >= MaxPerHour)
                throw new ValidationException(new[] { new ValidationFailure("QuestionText", "You can ask up to 5 doubts per hour. Please try again later.") });

            var doubt = new DoubtEntity
            {
                DoubtId = Guid.NewGuid(),
                UserId = req.UserId,
                CourseId = req.CourseId,
                LectureId = null,                           
                QuestionText = req.QuestionText.Trim(),
                Status = DoubtStatus.Open,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };
            await _doubts.AddAsync(doubt);
            await _doubts.SaveChangesAsync();
            return doubt.DoubtId;
        }
    }
}
