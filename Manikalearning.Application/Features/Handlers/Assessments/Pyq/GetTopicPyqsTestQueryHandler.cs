using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Pyq
{
    public class GetTopicPyqsTestQueryHandler : IRequestHandler<GetTopicPyqsTestQuery, ResponseDto<TopicPyqListDto>>
    {
        private readonly IRepository<Manikalearning.Domain.Entities.CourseLecture> _lectureRepo;
        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<Bookmark> _bookmarkRepo;
        private readonly IRepository<StudentSubscription> _subscriptionRepo;

        public GetTopicPyqsTestQueryHandler(
            IRepository<Manikalearning.Domain.Entities.CourseLecture> lectureRepo,
            IRepository<Question> questionRepo,
            IRepository<Bookmark> bookmarkRepo,
            IRepository<StudentSubscription> subscriptionRepo)
        {
            _lectureRepo = lectureRepo;
            _questionRepo = questionRepo;
            _bookmarkRepo = bookmarkRepo;
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<ResponseDto<TopicPyqListDto>> Handle(GetTopicPyqsTestQuery request, CancellationToken cancellationToken)
        { 
            var lecture = await _lectureRepo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LectureId == request.LectureId, cancellationToken);

            if (lecture == null || !lecture.TopicId.HasValue)
            {
                return ResponseDto<TopicPyqListDto>.Success(
                    new TopicPyqListDto(),
                    "Is lecture ke sath koi topic mapped nahi hai."
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
             
            var pyqBaseQuery = _questionRepo.Query()
                .AsNoTracking()
                .Include(q => q.Options)  
                .Where(q => q.TopicId == lecture.TopicId.Value
                         && q.IsPYQ
                         && q.IsActive
                         && q.IsApproved)
                .OrderByDescending(q => q.PYQYear);

            var totalQuestions = await pyqBaseQuery.CountAsync(cancellationToken);
             
            bool isPreviewOnly = !isSubscribed;
            var rawQuestions = isPreviewOnly
                ? await pyqBaseQuery.Take(10).ToListAsync(cancellationToken)
                : await pyqBaseQuery.ToListAsync(cancellationToken);
             
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
                    ? $"You are currently viewing a maximum of 10 questions. Please log in or subscribe to access the full test series ({totalQuestions} questions)."
                    : null
            };

            return ResponseDto<TopicPyqListDto>.Success(responseData, "Topic PYQs loaded successfully.");
        }
    }
}
