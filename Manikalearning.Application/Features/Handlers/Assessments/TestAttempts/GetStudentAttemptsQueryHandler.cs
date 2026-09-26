using Manikalearning.Application.Features.Commands.Assessments.TestAttempts;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manikalearning.Application.DTOs;

namespace Manikalearning.Application.Features.Handlers.Assessments.TestAttempts
{
    public class GetStudentAttemptsQueryHandler : IRequestHandler<GetStudentAttemptsQuery, List<TestAttemptDto>>
    {
        private readonly IRepository<TestAttempt> _attemptRepo;

        public GetStudentAttemptsQueryHandler(IRepository<TestAttempt> attemptRepo)
        {
            _attemptRepo = attemptRepo;
        }

        public async Task<List<TestAttemptDto>> Handle(GetStudentAttemptsQuery request, CancellationToken cancellationToken)
        {
            var query = _attemptRepo.Entity()
                .Include(a => a.Test)
                .Include(a => a.User)
                .AsNoTracking();

            if (request.TestId.HasValue && request.TestId.Value != Guid.Empty)
            {
                query = query.Where(a => a.TestId == request.TestId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(a => a.User.UserName!.ToLower().Contains(search)
                                      || a.User.Email!.ToLower().Contains(search)
                                      || a.Test.TestName.ToLower().Contains(search));
            }

            return await query
                .OrderByDescending(a => a.StartedAt)
                .Select(a => new TestAttemptDto
                {
                    AttemptId = a.AttemptId,
                    TestId = a.TestId,
                    TestTitle = a.Test != null ? a.Test.TestName : "N/A",
                    UserId = a.UserId,
                    UserName = a.User != null ? a.User.UserName! : "Student",
                    UserEmail = a.User != null ? a.User.Email! : "",
                    StartedAt = a.StartedAt,
                    SubmittedAt = a.SubmittedAt,
                    Status = a.Status ?? "In Progress",
                    TotalQuestions = a.TotalQuestions,
                    AttemptedQuestions = a.AttemptedQuestions,
                    CorrectAnswers = a.CorrectAnswers,
                    WrongAnswers = a.WrongAnswers,
                    SkippedQuestions = a.SkippedQuestions,
                    TotalMarks = a.TotalMarks,
                    Score = a.Score,
                    Accuracy = a.Accuracy,
                    TimeTakenSeconds = a.TimeTakenSeconds
                })
                .ToListAsync(cancellationToken);
        }
    }
}
