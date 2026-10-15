using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IRepository<User> _users;
        private readonly IRepository<StudentProfile> _profiles;
        private readonly IRepository<Exam> _exams;
        public UpdateProfileCommandHandler(IRepository<User> users, IRepository<StudentProfile> profiles, IRepository<Exam> exams)
        { _users = users; _profiles = profiles; _exams = exams; }

        public async Task<bool> Handle(UpdateProfileCommand req, CancellationToken ct)
        {
            var user = await _users.Entity().FirstOrDefaultAsync(u => u.Id == req.UserId, ct);     // tracked
            if (user == null) return false;

            var mobile = MobileNumber.Normalize(req.Mobile);
            if (mobile != null && await _users.Entity().AnyAsync(u => u.Id != req.UserId && u.Mobile == mobile, ct))
                throw new ValidationException(new[] { new ValidationFailure("Mobile", "This mobile number is already used by another account.") });

            if (req.TargetExamId.HasValue && !await _exams.Entity().AnyAsync(e => e.ExamId == req.TargetExamId && e.IsActive, ct))
                throw new ValidationException(new[] { new ValidationFailure("TargetExamId", "Please choose a valid exam.") });

            user.FullName = req.FullName.Trim();
            if (user.Mobile != mobile) { user.Mobile = mobile; user.IsMobileVerified = false; }      // a new number must be verified again
            user.UpdatedAt = DateTime.UtcNow;

            var p = await _profiles.Entity().FirstOrDefaultAsync(x => x.UserId == req.UserId, ct);
            if (p == null)
            {
                await _profiles.AddAsync(new StudentProfile
                {
                    UserId = req.UserId,
                    TargetExamId = req.TargetExamId,
                    TargetExamDate = req.TargetExamDate?.Date,
                    DailyGoal = req.DailyGoal,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                p.TargetExamId = req.TargetExamId;
                p.TargetExamDate = req.TargetExamDate?.Date;
                p.DailyGoal = req.DailyGoal;
                p.UpdatedAt = DateTime.UtcNow;
            }

            await _users.SaveChangesAsync();
            return true;
        }
    }
    public record UpdateProfileImageCommand(Guid UserId, string? ImagePath) : IRequest<string?>;

    public class UpdateProfileImageCommandValidator : AbstractValidator<UpdateProfileImageCommand>
    {
        public UpdateProfileImageCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ImagePath).MaximumLength(500)
                .Must(p => p == null || p.StartsWith("/uploads/profile/")).WithMessage("Invalid image path.");
        }
    }

    public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, string?>
    {
        private readonly IRepository<User> _users;
        public UpdateProfileImageCommandHandler(IRepository<User> users) => _users = users;

        public async Task<string?> Handle(UpdateProfileImageCommand req, CancellationToken ct)
        {
            var user = await _users.Entity().FirstOrDefaultAsync(u => u.Id == req.UserId, ct)
                       ?? throw new ValidationException(new[] { new ValidationFailure("UserId", "User not found.") });
            var old = user.ProfileImage;
            user.ProfileImage = req.ImagePath;
            user.UpdatedAt = DateTime.UtcNow;
            await _users.SaveChangesAsync();
            return old;
        }
    }
}
