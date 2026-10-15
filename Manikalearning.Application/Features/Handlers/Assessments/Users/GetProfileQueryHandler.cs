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
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto?>
    {
        private readonly IRepository<User> _users;
        private readonly IRepository<StudentProfile> _profiles;
        private readonly IRepository<Exam> _exams;
        public GetProfileQueryHandler(IRepository<User> users, IRepository<StudentProfile> profiles, IRepository<Exam> exams)
        { _users = users; _profiles = profiles; _exams = exams; }

        public async Task<ProfileDto?> Handle(GetProfileQuery req, CancellationToken ct)
        {
            var u = await _users.Entity().AsNoTracking().Where(x => x.Id == req.UserId).Select(x => new
            { x.FullName, x.Email, x.Mobile, x.IsMobileVerified, x.IsEmailVerified, x.ProfileImage, x.CreatedAt }).FirstOrDefaultAsync(ct);
            if (u == null) return null;

            var p = await _profiles.Entity().AsNoTracking().FirstOrDefaultAsync(x => x.UserId == req.UserId, ct);
            var exams = await _exams.Entity().AsNoTracking().Where(e => e.IsActive).OrderBy(e => e.DisplayOrder).ThenBy(e => e.ExamName)
                .Select(e => new ExamOptionDto { ExamId = e.ExamId, ExamName = e.ExamName }).ToListAsync(ct);

            return new ProfileDto
            {
                FullName = u.FullName,
                Email = u.Email,
                Mobile = u.Mobile,
                IsMobileVerified = u.IsMobileVerified,
                IsEmailVerified = u.IsEmailVerified,
                ProfileImage = u.ProfileImage,
                MemberSince = u.CreatedAt,
                TargetExamId = p?.TargetExamId,
                TargetExamDate = p?.TargetExamDate,
                DailyGoal = p?.DailyGoal ?? 30,
                Exams = exams
            };
        }
    }
}
