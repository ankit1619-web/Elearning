using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Pyq
{
    public class GetPYQQuestionsQueryHandler : IRequestHandler<GetPYQQuestionsQuery, ResponseDto<TopicPyqListDto>>
    {
        private readonly IRepository<Section> _section;
        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<Bookmark> _bookmarkRepo;
        private readonly IRepository<StudentSubscription> _subscriptionRepo;

        public GetPYQQuestionsQueryHandler(
            IRepository<Section> section,
            IRepository<Question> questionRepo,
            IRepository<Bookmark> bookmarkRepo,
            IRepository<StudentSubscription> subscriptionRepo)
        {
            _section = section;
            _questionRepo = questionRepo;
            _bookmarkRepo = bookmarkRepo;
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<ResponseDto<TopicPyqListDto>> Handle(GetPYQQuestionsQuery request, CancellationToken cancellationToken)
        {
            var lecture = await _section.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.SectionId == request.SectionId, cancellationToken);

            if (lecture == null || lecture.SectionId == Guid.Empty)
            {
                return ResponseDto<TopicPyqListDto>.Success(
                    new TopicPyqListDto(),
                    "No topic or section mapping was found for the specified request."
                );
            }

            bool isSubscribed = false;
            if (request.UserId.HasValue)
            {
                isSubscribed = await _subscriptionRepo.Query()
                    .AsNoTracking()
                    .AnyAsync(s => s.UserId == request.UserId.Value
                                && s.Status == "Active"
                                && s.EndDate >= DateTime.UtcNow, cancellationToken);
            }

            var bookmarkedQuestionIds = new List<Guid>();
            if (request.UserId.HasValue)
            {
                bookmarkedQuestionIds = await _bookmarkRepo.Query()
                    .AsNoTracking()
                    .Where(b => b.UserId == request.UserId.Value && b.QuestionId.HasValue)
                    .Select(b => b.QuestionId!.Value)
                    .ToListAsync(cancellationToken);
            }
            var pyqBaseQuery = await _questionRepo.Query()
               .AsNoTracking()
               .Include(q => q.Options)
               .Where(q => q.SectionId == request.SectionId 
                        && q.IsPYQ
                        && q.IsActive
                        && q.IsApproved)
               .OrderByDescending(q => q.PYQYear)
               .ToListAsync(cancellationToken);

            var totalQuestions =  pyqBaseQuery.Count();

            bool isPreviewOnly = !isSubscribed;
            var rawQuestions = isPreviewOnly
                ?  pyqBaseQuery.Take(10)
                :  pyqBaseQuery.ToList();

            var pyqDtos = rawQuestions.Select(q => new TopicPyqDto
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                QuestionTextHindi = q.QuestionTextHindi,
                Explanation = q.Explanation,
                ExplanationHindi = q.ExplanationHindi,
                PYQYear = q.PYQYear,
                PYQSession = q.PYQSession,
                ExamName = q.SourceName ?? "Competitive Exam",

                OptionA = q.Options.FirstOrDefault(o => o.DisplayOrder == 1)?.OptionText ?? "",
                OptionB = q.Options.FirstOrDefault(o => o.DisplayOrder == 2)?.OptionText ?? "",
                OptionC = q.Options.FirstOrDefault(o => o.DisplayOrder == 3)?.OptionText ?? "",
                OptionD = q.Options.FirstOrDefault(o => o.DisplayOrder == 4)?.OptionText ?? "",
                CorrectOption = q.Options.FirstOrDefault(o => o.IsCorrect)?.DisplayOrder.ToString() ?? "1",

                IsBookmarked = bookmarkedQuestionIds.Contains(q.QuestionId)
            }).ToList();

            var responseData = new TopicPyqListDto
            {
                Questions = pyqDtos,
                IsSubscribed = isSubscribed,
                IsPreviewOnly = isPreviewOnly,
                TotalAvailableQuestions = totalQuestions,
                BannerMessage = isPreviewOnly
                    ? $"You are currently viewing a preview of 10 questions. Please subscribe to unlock the full practice test ({totalQuestions} questions)."
                    : null
            };

            return ResponseDto<TopicPyqListDto>.Success(responseData, "Topic PYQs loaded successfully.");
        }
    }
}