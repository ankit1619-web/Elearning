using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore; 

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetMockTestDetailQueryHandler : IRequestHandler<GetMockTestDetailQuery, MockTestDetailDto?>
    {
        private readonly IRepository<Test> _testRepo;

        public GetMockTestDetailQueryHandler(IRepository<Test> testRepo)
        {
            _testRepo = testRepo;
        }

        public async Task<MockTestDetailDto?> Handle(GetMockTestDetailQuery request, CancellationToken cancellationToken)
        {
            return await _testRepo.Query()
                .Where(t => t.TestId == request.TestId && t.IsPublished)
                .Select(t => new MockTestDetailDto
                {
                    TestId = t.TestId,
                    Title = t.TestName,
                    DurationMinutes = t.DurationMinutes,
                    TotalMarks = t.TotalMarks,
                    Questions = t.TestQuestions
                        .OrderBy(tq => tq.QuestionOrder)
                        .Select(tq => new QuestionDto // Fully Qualified
                        {
                            QuestionId = tq.QuestionId,
                            QuestionText = tq.Question.QuestionText,
                            QuestionTextHindi = tq.Question.QuestionTextHindi,
                            DisplayOrder = tq.QuestionOrder,
                            Options = tq.Question.Options
                                .OrderBy(o => o.DisplayOrder)
                                .Select(o => new QuestionOptionDto // Fully Qualified
                                {
                                    OptionId = o.OptionId,
                                    OptionLabel = o.OptionLabel,
                                    OptionText = o.OptionText,
                                    OptionTextHindi = o.OptionTextHindi ?? string.Empty
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
