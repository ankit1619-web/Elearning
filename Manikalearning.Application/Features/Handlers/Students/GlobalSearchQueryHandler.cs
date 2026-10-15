using FluentValidation;
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
    public class SearchCourseDto { public Guid CourseId { get; set; } public string CourseName { get; set; } = ""; public bool IsFree { get; set; } public bool CanOpen { get; set; } }
    public class SearchExamDto { public Guid ExamId { get; set; } public string ExamName { get; set; } = ""; public string ExamCode { get; set; } = ""; }
    public class SearchLectureDto
    {
        public Guid LectureId { get; set; }
        public string LectureTitle { get; set; } = "";
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = ""; public bool CanOpen { get; set; }
    }
    public class SearchResultsDto
    {
        public string Query { get; set; } = "";
        public List<SearchCourseDto> Courses { get; set; } = new();
        public List<SearchExamDto> Exams { get; set; } = new();
        public List<SearchLectureDto> Lectures { get; set; } = new();
        public int Total => Courses.Count + Exams.Count + Lectures.Count;
    }

    public record GlobalSearchQuery(Guid UserId, string Query) : IRequest<SearchResultsDto>;

    public class GlobalSearchQueryValidator : AbstractValidator<GlobalSearchQuery>
    {
        public GlobalSearchQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Query).NotEmpty().Must(q => q.Trim().Length >= 2).WithMessage("Type at least 2 letters.").MaximumLength(100);
        }
    }

    public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, SearchResultsDto>
    {
        private const int PerGroup = 8;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<CourseLecture> _lectures;
        private readonly IRepository<CourseSection> _sections;
        private readonly ISubscriptionChecker _checker;

        public GlobalSearchQueryHandler(IRepository<Course> courses, IRepository<Exam> exams, IRepository<CourseLecture> lectures,
                                        IRepository<CourseSection> sections, ISubscriptionChecker checker)
        { _courses = courses; _exams = exams; _lectures = lectures; _sections = sections; _checker = checker; }

        public async Task<SearchResultsDto> Handle(GlobalSearchQuery req, CancellationToken ct)
        {
            var q = req.Query.Trim();
            var open = await _checker.AccessibleCourseIdsAsync(req.UserId, ct);       // courses he can open right now

            var courses = await _courses.Entity().AsNoTracking().Where(c => c.IsPublished && c.CourseName.Contains(q))
                .OrderBy(c => c.CourseName).Take(PerGroup).Select(c => new { c.CourseId, c.CourseName, c.IsFree }).ToListAsync(ct);

            var exams = await _exams.Entity().AsNoTracking().Where(e => e.IsActive && (e.ExamName.Contains(q) || e.ExamCode.Contains(q)))
                .OrderBy(e => e.DisplayOrder).Take(PerGroup)
                .Select(e => new SearchExamDto { ExamId = e.ExamId, ExamName = e.ExamName, ExamCode = e.ExamCode }).ToListAsync(ct);

            var lectures = await (from l in _lectures.Entity().AsNoTracking()
                                  join s in _sections.Entity().AsNoTracking() on l.CourseSectionId equals s.CourseSectionId
                                  join c in _courses.Entity().AsNoTracking() on s.CourseId equals c.CourseId
                                  where l.IsPublished && s.IsActive && c.IsPublished && l.LectureTitle.Contains(q)
                                  orderby l.LectureTitle
                                  select new { l.LectureId, l.LectureTitle, l.IsFreePreview, c.CourseId, c.CourseName })
                                 .Take(PerGroup).ToListAsync(ct);

            return new SearchResultsDto
            {
                Query = q,
                Exams = exams,
                Courses = courses.Select(c => new SearchCourseDto { CourseId = c.CourseId, CourseName = c.CourseName, IsFree = c.IsFree, CanOpen = open.Contains(c.CourseId) }).ToList(),
                Lectures = lectures.Select(l => new SearchLectureDto
                {
                    LectureId = l.LectureId,
                    LectureTitle = l.LectureTitle,
                    CourseId = l.CourseId,
                    CourseName = l.CourseName,
                    CanOpen = l.IsFreePreview || open.Contains(l.CourseId)           // a free-preview lecture opens for everybody
                }).ToList()
            };
        }
    }
}
