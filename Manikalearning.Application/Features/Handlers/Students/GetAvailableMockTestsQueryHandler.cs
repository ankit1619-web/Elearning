using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetAvailableMockTestsQueryHandler : IRequestHandler<GetAvailableMockTestsQuery, List<MockTestDto>>
    {
        private readonly IRepository<Test> _testRepo;

        public GetAvailableMockTestsQueryHandler(IRepository<Test> testRepo)
        {
            _testRepo = testRepo;
        }

        public async Task<List<MockTestDto>> Handle(GetAvailableMockTestsQuery request, CancellationToken cancellationToken)
        {
            return await _testRepo.Query()
                .Where(t => t.IsPublished)
                .OrderBy(t => t.CreatedAt)
                .Select(t => new MockTestDto
                {
                    TestId = t.TestId,
                    Title = t.TestName,
                    TotalQuestions = t.TestQuestions.Count,
                    DurationMinutes = t.DurationMinutes,
                    TotalMarks = t.TotalMarks,
                    IsAttempted = t.TestAttempts.Any(a => a.UserId == request.UserId),
                    LastScore = t.TestAttempts
                        .Where(a => a.UserId == request.UserId)
                        .OrderByDescending(a => a.StartedAt)
                        .Select(a => (int?)a.Score)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
