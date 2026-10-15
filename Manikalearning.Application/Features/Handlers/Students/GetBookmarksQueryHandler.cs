using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record GetBookmarksQuery(Guid UserId, string Type = BookmarkType.Question, string? Search = null, int Page = 1, int PageSize = 10) : IRequest<BookmarkListDto>;

    public class GetBookmarksQueryValidator : AbstractValidator<GetBookmarksQuery>
    {
        public GetBookmarksQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Type).Must(t => t == BookmarkType.Question || t == BookmarkType.Lecture).WithMessage("Unknown saved type.");
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 50);
            RuleFor(x => x.Search).MaximumLength(100);
        }
    }

    public class GetBookmarksQueryHandler : IRequestHandler<GetBookmarksQuery, BookmarkListDto>
    {
        private readonly IRepository<Bookmark> _bookmarks;
        private readonly IRepository<Question> _questions;
        private readonly IRepository<QuestionOption> _options;
        private readonly IRepository<CourseLecture> _lectures;
        private readonly IRepository<CourseSection> _sections;
        private readonly IRepository<Course> _courses;

        public GetBookmarksQueryHandler(IRepository<Bookmark> bookmarks, IRepository<Question> questions, IRepository<QuestionOption> options,
                                        IRepository<CourseLecture> lectures, IRepository<CourseSection> sections, IRepository<Course> courses)
        { _bookmarks = bookmarks; _questions = questions; _options = options; _lectures = lectures; _sections = sections; _courses = courses; }

        public async Task<BookmarkListDto> Handle(GetBookmarksQuery req, CancellationToken ct)
        {
            var search = req.Search?.Trim();
            var mine = _bookmarks.Entity().AsNoTracking().Where(b => b.UserId == req.UserId);

            var result = new BookmarkListDto
            {
                Type = req.Type,
                Search = search,
                Page = req.Page,
                PageSize = req.PageSize,
                QuestionCount = await mine.CountAsync(b => b.QuestionId != null, ct),
                LectureCount = await mine.CountAsync(b => b.LectureId != null, ct)
            };

            if (req.Type == BookmarkType.Lecture) await LoadLecturesAsync(req, search, result, ct);
            else await LoadQuestionsAsync(req, search, result, ct);
            return result;
        }

        private async Task LoadQuestionsAsync(GetBookmarksQuery req, string? search, BookmarkListDto result, CancellationToken ct)
        {
            var query = from b in _bookmarks.Entity().AsNoTracking()
                        where b.UserId == req.UserId && b.QuestionId != null
                        join q in _questions.Entity().AsNoTracking() on b.QuestionId equals (Guid?)q.QuestionId
                        select new { b.BookmarkId, b.QuestionId, q.QuestionText, q.Explanation, b.CreatedAt };

            if (!string.IsNullOrEmpty(search)) query = query.Where(x => x.QuestionText.Contains(search));

            result.TotalCount = await query.CountAsync(ct);
            var page = await query.OrderByDescending(x => x.CreatedAt)
                                  .Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync(ct);

            var ids = page.Select(x => x.QuestionId!.Value).ToList();
            var correct = await _options.Entity().AsNoTracking()
                .Where(o => ids.Contains(o.QuestionId) && o.IsCorrect)
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new { o.QuestionId, o.OptionLabel, o.OptionText }).ToListAsync(ct);
            var answerByQuestion = correct.GroupBy(c => c.QuestionId)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => $"{x.OptionLabel}. {x.OptionText}")));

            result.Items = page.Select(x => new BookmarkDto
            {
                BookmarkId = x.BookmarkId,
                ItemType = BookmarkType.Question,
                SavedAt = x.CreatedAt,
                QuestionId = x.QuestionId,
                QuestionText = x.QuestionText,
                Explanation = x.Explanation,
                CorrectAnswer = answerByQuestion.TryGetValue(x.QuestionId!.Value, out var a) ? a : null
            }).ToList();
        }

        private async Task LoadLecturesAsync(GetBookmarksQuery req, string? search, BookmarkListDto result, CancellationToken ct)
        {
            var query = from b in _bookmarks.Entity().AsNoTracking()
                        where b.UserId == req.UserId && b.LectureId != null
                        join l in _lectures.Entity().AsNoTracking() on b.LectureId equals (Guid?)l.LectureId
                        join s in _sections.Entity().AsNoTracking() on l.CourseSectionId equals s.CourseSectionId
                        join c in _courses.Entity().AsNoTracking() on s.CourseId equals c.CourseId
                        where l.IsPublished && c.IsPublished
                        select new { b.BookmarkId, l.LectureId, l.LectureTitle, l.Description, l.VideoDurationSeconds, c.CourseId, c.CourseName, b.CreatedAt };

            if (!string.IsNullOrEmpty(search)) query = query.Where(x => x.LectureTitle.Contains(search) || x.CourseName.Contains(search));

            result.TotalCount = await query.CountAsync(ct);
            var page = await query.OrderByDescending(x => x.CreatedAt)
                                  .Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync(ct);

            result.Items = page.Select(x => new BookmarkDto
            {
                BookmarkId = x.BookmarkId,
                ItemType = BookmarkType.Lecture,
                SavedAt = x.CreatedAt,
                LectureId = x.LectureId,
                LectureTitle = x.LectureTitle,
                LectureDescription = x.Description,
                DurationSeconds = x.VideoDurationSeconds,
                CourseId = x.CourseId,
                CourseName = x.CourseName
            }).ToList();
        }
    }

}
