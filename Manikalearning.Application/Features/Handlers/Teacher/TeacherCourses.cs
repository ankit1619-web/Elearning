using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    public class TeacherCourses
    {
    }
    public class TeacherCourseListItem
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsFree { get; set; }
        public bool IsPublished { get; set; }
        public int SectionCount { get; set; }
        public int LectureCount { get; set; }
        public int StudentCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TeacherCourseForm
    {
        public Guid? CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }   
        public bool IsFree { get; set; }
    }

    public class CourseLectureRow
    {
        public Guid LectureId { get; set; }
        public string LectureTitle { get; set; } = "";
        public bool HasVideo { get; set; }
        public bool HasNotes { get; set; }
        public int? VideoDurationSeconds { get; set; }
        public bool IsFreePreview { get; set; }
        public bool IsPublished { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CourseSectionRow
    {
        public Guid CourseSectionId { get; set; }
        public string SectionName { get; set; } = "";
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public List<CourseLectureRow> Lectures { get; set; } = new();
    }

    public class CourseManageDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public bool IsPublished { get; set; }
        public List<CourseSectionRow> Sections { get; set; } = new();
        public int PublishedLectures => Sections.Sum(s => s.Lectures.Count(l => l.IsPublished));
    }

    public class TeacherSectionForm
    {
        public Guid? CourseSectionId { get; set; }
        public Guid CourseId { get; set; }
        public string SectionName { get; set; } = "";
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TeacherLectureForm
    {
        public Guid? LectureId { get; set; }
        public Guid CourseSectionId { get; set; }
        public string LectureTitle { get; set; } = "";
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public int? VideoDurationMinutes { get; set; }
        public string? NotesUrl { get; set; }
        public string? Notes { get; set; }
        public bool IsFreePreview { get; set; }
        public bool IsPublished { get; set; }
        public Guid? TopicId { get; set; }
        public int DisplayOrder { get; set; }

        // Sirf edit form me dropdown pre-select karne ke liye (save nahi hote)
        public Guid? PickSubjectId { get; set; }
        public Guid? PickSectionId { get; set; }
    }

    public enum LectureFlag { Publish, FreePreview }

     
    public static class CourseAccess
    {
        public static async Task<TCourseSection?> OwnedSection(this IRepository<TCourseSection> sections,
            IRepository<TCourse> courses, Guid sectionId, Guid teacherId, CancellationToken ct)
        {
            var s = await sections.Entity().AsNoTracking().FirstOrDefaultAsync(x => x.CourseSectionId == sectionId, ct);
            if (s == null) return null;
            return await courses.OwnsCourse(s.CourseId, teacherId, ct) ? s : null;
        }

        public static async Task<TLecture?> OwnedLecture(this IRepository<TLecture> lectures,
            IRepository<TCourseSection> sections, IRepository<TCourse> courses, Guid lectureId, Guid teacherId, CancellationToken ct)
        {
            var l = await lectures.Entity().FirstOrDefaultAsync(x => x.LectureId == lectureId, ct);  
            if (l == null) return null;
            return await sections.OwnedSection(courses, l.CourseSectionId, teacherId, ct) != null ? l : null;
        }

        public static bool HasContent(string? videoUrl, string? notesUrl, string? notes)
            => !string.IsNullOrWhiteSpace(videoUrl) || !string.IsNullOrWhiteSpace(notesUrl) || !string.IsNullOrWhiteSpace(notes);
    }
     
    public record GetTeacherCoursesQuery(Guid TeacherId) : IRequest<List<TeacherCourseListItem>>;

    public class GetTeacherCoursesQueryHandler : IRequestHandler<GetTeacherCoursesQuery, List<TeacherCourseListItem>>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        private readonly IRepository<TStudentCourse> _studentCourses;

        public GetTeacherCoursesQueryHandler(IRepository<TCourse> c, IRepository<TCourseSection> s,
            IRepository<TLecture> l, IRepository<TStudentCourse> sc)
        { _courses = c; _sections = s; _lectures = l; _studentCourses = sc; }

        public async Task<List<TeacherCourseListItem>> Handle(GetTeacherCoursesQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId;
            var list = await _courses.Entity().AsNoTracking()
                .Where(c => c.CreatedBy == tid)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new TeacherCourseListItem
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Price = c.Price,
                    DiscountPrice = c.DiscountPrice,
                    IsFree = c.IsFree,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt
                }).ToListAsync(ct);

            var ids = list.Select(c => c.CourseId).ToList();
            if (ids.Count == 0) return list;

            var sectionCourse = await _sections.Entity().AsNoTracking()
                .Where(s => ids.Contains(s.CourseId)).Select(s => s.CourseId).ToListAsync(ct);

            var lectureCourse = await (from l in _lectures.Entity().AsNoTracking()
                                       from s in _sections.Entity().AsNoTracking()
                                       where l.CourseSectionId == s.CourseSectionId && ids.Contains(s.CourseId)
                                       select s.CourseId).ToListAsync(ct);

            var students = await _studentCourses.Entity().AsNoTracking()
                .Where(x => x.IsActive && ids.Contains(x.CourseId))
                .Select(x => new { x.CourseId, x.UserId }).Distinct().ToListAsync(ct);

            var secMap = sectionCourse.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var lecMap = lectureCourse.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var stuMap = students.GroupBy(x => x.CourseId).ToDictionary(g => g.Key, g => g.Count());

            foreach (var c in list)
            {
                c.SectionCount = secMap.GetValueOrDefault(c.CourseId);
                c.LectureCount = lecMap.GetValueOrDefault(c.CourseId);
                c.StudentCount = stuMap.GetValueOrDefault(c.CourseId);
            }
            return list;
        }
    }
     
    public record GetTeacherCourseFormQuery(Guid TeacherId, Guid CourseId) : IRequest<TeacherCourseForm?>;

    public class GetTeacherCourseFormQueryHandler : IRequestHandler<GetTeacherCourseFormQuery, TeacherCourseForm?>
    {
        private readonly IRepository<TCourse> _courses;
        public GetTeacherCourseFormQueryHandler(IRepository<TCourse> c) { _courses = c; }

        public async Task<TeacherCourseForm?> Handle(GetTeacherCourseFormQuery r, CancellationToken ct)
            => await _courses.Entity().AsNoTracking()
                .Where(c => c.CourseId == r.CourseId && c.CreatedBy == r.TeacherId)
                .Select(c => new TeacherCourseForm
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Price = c.Price,
                    DiscountPrice = c.DiscountPrice,
                    IsFree = c.IsFree
                }).FirstOrDefaultAsync(ct);
    }

    public record SaveTeacherCourseCommand(Guid TeacherId, TeacherCourseForm Form) : IRequest<TeacherActionResult>;

    public class SaveTeacherCourseCommandHandler : IRequestHandler<SaveTeacherCourseCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        public SaveTeacherCourseCommandHandler(IRepository<TCourse> c) { _courses = c; }

        public async Task<TeacherActionResult> Handle(SaveTeacherCourseCommand r, CancellationToken ct)
        {
            var f = r.Form;
            if (string.IsNullOrWhiteSpace(f.CourseName)) return TeacherActionResult.Fail("Course name zaroori hai.");
            if (f.CourseName.Trim().Length > 250) return TeacherActionResult.Fail("Course name 250 characters se chhota rakhein.");

            if (f.IsFree) { f.Price = 0; f.DiscountPrice = 0; }
            else
            {
                if (f.Price <= 0) return TeacherActionResult.Fail("Price 0 se zyada rakho, ya course ko Free mark karo.");
                if (f.DiscountPrice < 0 || f.DiscountPrice > f.Price)
                    return TeacherActionResult.Fail("Discount price, price se zyada ya negative nahi ho sakta.");
            }

            var isNew = f.CourseId == null || f.CourseId == Guid.Empty;
            if (isNew)
            {
                var c = new TCourse
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = f.CourseName.Trim(),
                    Description = f.Description?.Trim() ?? "",
                    ThumbnailUrl = f.ThumbnailUrl ?? "",
                    Price = f.Price,
                    DiscountPrice = f.DiscountPrice,
                    IsFree = f.IsFree,
                    IsPublished = false,               
                    CreatedBy = r.TeacherId,
                    CreatedAt = DateTime.UtcNow
                };
                await _courses.AddRow(c);
                return TeacherActionResult.Ok("Course created. Ab sections aur lectures add karein.", c.CourseId);
            }

            var id = f.CourseId!.Value;
            var course = await _courses.Entity().FirstOrDefaultAsync(c => c.CourseId == id && c.CreatedBy == r.TeacherId, ct);
            if (course == null) return TeacherActionResult.Fail("Course not found.");

            course.CourseName = f.CourseName.Trim();
            course.Description = f.Description?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(f.ThumbnailUrl)) course.ThumbnailUrl = f.ThumbnailUrl;
            course.Price = f.Price; course.DiscountPrice = f.DiscountPrice; course.IsFree = f.IsFree;
            await _courses.UpdateRow(course);
            return TeacherActionResult.Ok("Course updated.", course.CourseId);
        }
    }

    public record ToggleCoursePublishCommand(Guid TeacherId, Guid CourseId) : IRequest<TeacherActionResult>;

    public class ToggleCoursePublishCommandHandler : IRequestHandler<ToggleCoursePublishCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;

        public ToggleCoursePublishCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s, IRepository<TLecture> l)
        { _courses = c; _sections = s; _lectures = l; }

        public async Task<TeacherActionResult> Handle(ToggleCoursePublishCommand r, CancellationToken ct)
        {
            var course = await _courses.Entity().FirstOrDefaultAsync(c => c.CourseId == r.CourseId && c.CreatedBy == r.TeacherId, ct);
            if (course == null) return TeacherActionResult.Fail("Course not found.");

            if (!course.IsPublished)
            {
                var courseId = course.CourseId;
                var hasLecture = await (from l in _lectures.Entity().AsNoTracking()
                                        from s in _sections.Entity().AsNoTracking()
                                        where l.CourseSectionId == s.CourseSectionId && s.CourseId == courseId
                                              && s.IsActive && l.IsPublished
                                        select l.LectureId).AnyAsync(ct);
                if (!hasLecture)
                    return TeacherActionResult.Fail("Publish karne se pehle kam se kam ek active section me ek published lecture chahiye.");
            }

            course.IsPublished = !course.IsPublished;
            await _courses.UpdateRow(course);
            return TeacherActionResult.Ok(course.IsPublished ? "Course published." : "Course unpublished.");
        }
    }

    public record DeleteTeacherCourseCommand(Guid TeacherId, Guid CourseId) : IRequest<TeacherActionResult>;

    public class DeleteTeacherCourseCommandHandler : IRequestHandler<DeleteTeacherCourseCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        private readonly IRepository<TStudentCourse> _studentCourses;

        public DeleteTeacherCourseCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s,
            IRepository<TLecture> l, IRepository<TStudentCourse> sc)
        { _courses = c; _sections = s; _lectures = l; _studentCourses = sc; }

        public async Task<TeacherActionResult> Handle(DeleteTeacherCourseCommand r, CancellationToken ct)
        {
            var course = await _courses.Entity().FirstOrDefaultAsync(c => c.CourseId == r.CourseId && c.CreatedBy == r.TeacherId, ct);
            if (course == null) return TeacherActionResult.Fail("Course not found.");

            var courseId = course.CourseId;
            if (await _studentCourses.Entity().AsNoTracking().AnyAsync(x => x.CourseId == courseId, ct))
                return TeacherActionResult.Fail("Is course me students enrolled hain, isliye delete nahi ho sakta. Unpublish kar dein.");

            var sections = await _sections.Entity().Where(s => s.CourseId == courseId).ToListAsync(ct);
            var sectionIds = sections.Select(s => s.CourseSectionId).ToList();
            var lectures = await _lectures.Entity().Where(l => sectionIds.Contains(l.CourseSectionId)).ToListAsync(ct);

            try
            {
                await _lectures.DeleteRows(lectures);
                await _sections.DeleteRows(sections);
                await _courses.DeleteRow(course);
            }
            catch (DbUpdateException)
            {
                return TeacherActionResult.Fail("Course delete nahi ho paya, isse jude records maujood hain (plan/doubts). Unpublish kar dein.");
            }
            return TeacherActionResult.Ok("Course deleted.");
        }
    }
     
    public record GetCourseManageQuery(Guid TeacherId, Guid CourseId) : IRequest<CourseManageDto?>;

    public class GetCourseManageQueryHandler : IRequestHandler<GetCourseManageQuery, CourseManageDto?>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;

        public GetCourseManageQueryHandler(IRepository<TCourse> c, IRepository<TCourseSection> s, IRepository<TLecture> l)
        { _courses = c; _sections = s; _lectures = l; }

        public async Task<CourseManageDto?> Handle(GetCourseManageQuery r, CancellationToken ct)
        {
            var course = await _courses.Entity().AsNoTracking()
                .Where(c => c.CourseId == r.CourseId && c.CreatedBy == r.TeacherId)
                .Select(c => new { c.CourseId, c.CourseName, c.IsPublished }).FirstOrDefaultAsync(ct);
            if (course == null) return null;

            var courseId = course.CourseId;
            var sections = await _sections.Entity().AsNoTracking()
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.DisplayOrder).ThenBy(s => s.SectionName)
                .Select(s => new CourseSectionRow
                {
                    CourseSectionId = s.CourseSectionId,
                    SectionName = s.SectionName,
                    Description = s.Description,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = s.IsActive
                }).ToListAsync(ct);

            var sectionIds = sections.Select(s => s.CourseSectionId).ToList();
            var lectures = await _lectures.Entity().AsNoTracking()
                .Where(l => sectionIds.Contains(l.CourseSectionId))
                .OrderBy(l => l.DisplayOrder).ThenBy(l => l.CreatedAt)
                .Select(l => new
                {
                    l.CourseSectionId,
                    Row = new CourseLectureRow
                    {
                        LectureId = l.LectureId,
                        LectureTitle = l.LectureTitle,
                        HasVideo = l.VideoUrl != null && l.VideoUrl != "",
                        HasNotes = (l.NotesUrl != null && l.NotesUrl != "") || (l.Notes != null && l.Notes != ""),
                        VideoDurationSeconds = l.VideoDurationSeconds,
                        IsFreePreview = l.IsFreePreview,
                        IsPublished = l.IsPublished,
                        DisplayOrder = l.DisplayOrder
                    }
                }).ToListAsync(ct);

            foreach (var s in sections)
                s.Lectures = lectures.Where(x => x.CourseSectionId == s.CourseSectionId).Select(x => x.Row).ToList();

            return new CourseManageDto
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                IsPublished = course.IsPublished,
                Sections = sections
            };
        }
    }

    // =====================================================================
    //  SECTIONS
    // =====================================================================
    public record GetTeacherSectionFormQuery(Guid TeacherId, Guid SectionId) : IRequest<TeacherSectionForm?>;

    public class GetTeacherSectionFormQueryHandler : IRequestHandler<GetTeacherSectionFormQuery, TeacherSectionForm?>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        public GetTeacherSectionFormQueryHandler(IRepository<TCourse> c, IRepository<TCourseSection> s) { _courses = c; _sections = s; }

        public async Task<TeacherSectionForm?> Handle(GetTeacherSectionFormQuery r, CancellationToken ct)
        {
            var s = await _sections.OwnedSection(_courses, r.SectionId, r.TeacherId, ct);
            if (s == null) return null;
            return new TeacherSectionForm
            {
                CourseSectionId = s.CourseSectionId,
                CourseId = s.CourseId,
                SectionName = s.SectionName,
                Description = s.Description,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive
            };
        }
    }

    public record SaveTeacherSectionCommand(Guid TeacherId, TeacherSectionForm Form) : IRequest<TeacherActionResult>;

    public class SaveTeacherSectionCommandHandler : IRequestHandler<SaveTeacherSectionCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        public SaveTeacherSectionCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s) { _courses = c; _sections = s; }

        public async Task<TeacherActionResult> Handle(SaveTeacherSectionCommand r, CancellationToken ct)
        {
            var f = r.Form;
            if (string.IsNullOrWhiteSpace(f.SectionName)) return TeacherActionResult.Fail("Section name zaroori hai.");
            if (f.SectionName.Trim().Length > 250) return TeacherActionResult.Fail("Section name 250 characters se chhota rakhein.");

            var isNew = f.CourseSectionId == null || f.CourseSectionId == Guid.Empty;
            if (isNew)
            {
                if (!await _courses.OwnsCourse(f.CourseId, r.TeacherId, ct)) return TeacherActionResult.Fail("Course not found.");
                var courseId = f.CourseId;
                var max = await _sections.Entity().AsNoTracking().Where(s => s.CourseId == courseId)
                    .MaxAsync(s => (int?)s.DisplayOrder, ct) ?? 0;

                var sec = new TCourseSection
                {
                    CourseSectionId = Guid.NewGuid(),
                    CourseId = f.CourseId,
                    SectionName = f.SectionName.Trim(),
                    Description = f.Description?.Trim(),
                    DisplayOrder = f.DisplayOrder > 0 ? f.DisplayOrder : max + 1,
                    IsActive = f.IsActive
                };
                await _sections.AddRow(sec);
                return TeacherActionResult.Ok("Section added.", sec.CourseSectionId);
            }

            var id = f.CourseSectionId!.Value;
            if (await _sections.OwnedSection(_courses, id, r.TeacherId, ct) == null) return TeacherActionResult.Fail("Section not found.");
            var section = await _sections.Entity().FirstAsync(s => s.CourseSectionId == id, ct); // tracked

            section.SectionName = f.SectionName.Trim();
            section.Description = f.Description?.Trim();
            if (f.DisplayOrder > 0) section.DisplayOrder = f.DisplayOrder;
            section.IsActive = f.IsActive;
            await _sections.UpdateRow(section);
            return TeacherActionResult.Ok("Section updated.", section.CourseSectionId);
        }
    }

    public record DeleteTeacherSectionCommand(Guid TeacherId, Guid SectionId) : IRequest<TeacherActionResult>;

    public class DeleteTeacherSectionCommandHandler : IRequestHandler<DeleteTeacherSectionCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        public DeleteTeacherSectionCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s, IRepository<TLecture> l)
        { _courses = c; _sections = s; _lectures = l; }

        public async Task<TeacherActionResult> Handle(DeleteTeacherSectionCommand r, CancellationToken ct)
        {
            if (await _sections.OwnedSection(_courses, r.SectionId, r.TeacherId, ct) == null)
                return TeacherActionResult.Fail("Section not found.");

            var sid = r.SectionId;
            if (await _lectures.Entity().AsNoTracking().AnyAsync(l => l.CourseSectionId == sid, ct))
                return TeacherActionResult.Fail("Is section me lectures hain. Pehle lectures hatao.");

            var section = await _sections.Entity().FirstAsync(s => s.CourseSectionId == sid, ct);
            try { await _sections.DeleteRow(section); }
            catch (DbUpdateException) { return TeacherActionResult.Fail("Section delete nahi ho paya. Isse jude records maujood hain."); }
            return TeacherActionResult.Ok("Section deleted.");
        }
    }

    public record MoveCourseSectionCommand(Guid TeacherId, Guid SectionId, bool Up) : IRequest<TeacherActionResult>;

    public class MoveCourseSectionCommandHandler : IRequestHandler<MoveCourseSectionCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        public MoveCourseSectionCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s) { _courses = c; _sections = s; }

        public async Task<TeacherActionResult> Handle(MoveCourseSectionCommand r, CancellationToken ct)
        {
            var owned = await _sections.OwnedSection(_courses, r.SectionId, r.TeacherId, ct);
            if (owned == null) return TeacherActionResult.Fail("Section not found.");

            var courseId = owned.CourseId;
            var sibs = await _sections.Entity().Where(s => s.CourseId == courseId)
                .OrderBy(s => s.DisplayOrder).ThenBy(s => s.SectionName).ToListAsync(ct);

            var i = sibs.FindIndex(s => s.CourseSectionId == r.SectionId);
            var j = r.Up ? i - 1 : i + 1;
            if (i < 0 || j < 0 || j >= sibs.Count) return TeacherActionResult.Ok("Already at the edge.");

            (sibs[i], sibs[j]) = (sibs[j], sibs[i]);
            for (var k = 0; k < sibs.Count; k++)          // duplicate orders ko bhi theek karta hai
                if (sibs[k].DisplayOrder != k + 1) { sibs[k].DisplayOrder = k + 1; await _sections.UpdateRow(sibs[k]); }

            return TeacherActionResult.Ok("Order updated.");
        }
    }

    // =====================================================================
    //  LECTURES
    // =====================================================================
    public record GetTeacherLectureFormQuery(Guid TeacherId, Guid LectureId) : IRequest<TeacherLectureForm?>;

    public class GetTeacherLectureFormQueryHandler : IRequestHandler<GetTeacherLectureFormQuery, TeacherLectureForm?>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        private readonly IRepository<TTopic> _topics;
        private readonly IRepository<TSection> _curSections;

        public GetTeacherLectureFormQueryHandler(IRepository<TCourse> c, IRepository<TCourseSection> s,
            IRepository<TLecture> l, IRepository<TTopic> t, IRepository<TSection> cs)
        { _courses = c; _sections = s; _lectures = l; _topics = t; _curSections = cs; }

        public async Task<TeacherLectureForm?> Handle(GetTeacherLectureFormQuery r, CancellationToken ct)
        {
            var l = await _lectures.OwnedLecture(_sections, _courses, r.LectureId, r.TeacherId, ct);
            if (l == null) return null;

            var form = new TeacherLectureForm
            {
                LectureId = l.LectureId,
                CourseSectionId = l.CourseSectionId,
                LectureTitle = l.LectureTitle,
                Description = l.Description,
                VideoUrl = l.VideoUrl,
                VideoDurationMinutes = l.VideoDurationSeconds.HasValue ? (int?)Math.Max(1, (int)Math.Round(l.VideoDurationSeconds.Value / 60.0)) : null,
                NotesUrl = l.NotesUrl,
                Notes = l.Notes,
                IsFreePreview = l.IsFreePreview,
                IsPublished = l.IsPublished,
                TopicId = l.TopicId,
                DisplayOrder = l.DisplayOrder
            };

            var topicId = l.TopicId;
            if (topicId != null)
            {
                var pick = await (from t in _topics.Entity().AsNoTracking()
                                  from cs in _curSections.Entity().AsNoTracking()
                                  where t.TopicId == topicId && cs.SectionId == t.SectionId
                                  select new { cs.SectionId, cs.SubjectId }).FirstOrDefaultAsync(ct);
                if (pick != null) { form.PickSectionId = pick.SectionId; form.PickSubjectId = pick.SubjectId; }
            }
            return form;
        }
    }

    public record SaveTeacherLectureCommand(Guid TeacherId, TeacherLectureForm Form) : IRequest<TeacherActionResult>;

    public class SaveTeacherLectureCommandHandler : IRequestHandler<SaveTeacherLectureCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        private readonly IRepository<TTopic> _topics;

        public SaveTeacherLectureCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s,
            IRepository<TLecture> l, IRepository<TTopic> t)
        { _courses = c; _sections = s; _lectures = l; _topics = t; }

        public async Task<TeacherActionResult> Handle(SaveTeacherLectureCommand r, CancellationToken ct)
        {
            var f = r.Form;
            if (string.IsNullOrWhiteSpace(f.LectureTitle)) return TeacherActionResult.Fail("Lecture title zaroori hai.");
            if (f.LectureTitle.Trim().Length > 250) return TeacherActionResult.Fail("Lecture title 250 characters se chhota rakhein.");
            if (f.VideoUrl?.Length > 1000 || f.NotesUrl?.Length > 1000) return TeacherActionResult.Fail("URL 1000 characters se chhota rakhein.");
            if (f.VideoDurationMinutes is < 0 or > 1000) return TeacherActionResult.Fail("Duration 0 se 1000 minutes ke beech rakhein.");
            if (f.IsPublished && !CourseAccess.HasContent(f.VideoUrl, f.NotesUrl, f.Notes))
                return TeacherActionResult.Fail("Published lecture me video ya notes me se kuch to hona chahiye.");

            if (f.TopicId != null)
            {
                var tid = f.TopicId;
                if (!await _topics.Entity().AsNoTracking().AnyAsync(t => t.TopicId == tid, ct))
                    return TeacherActionResult.Fail("Selected topic valid nahi hai.");
            }

            var video = string.IsNullOrWhiteSpace(f.VideoUrl) ? null : f.VideoUrl.Trim();
            var notesUrl = string.IsNullOrWhiteSpace(f.NotesUrl) ? null : f.NotesUrl.Trim();
            var notes = string.IsNullOrWhiteSpace(f.Notes) ? null : f.Notes.Trim();
            var seconds = f.VideoDurationMinutes.HasValue ? f.VideoDurationMinutes * 60 : null;

            var isNew = f.LectureId == null || f.LectureId == Guid.Empty;
            if (isNew)
            {
                if (await _sections.OwnedSection(_courses, f.CourseSectionId, r.TeacherId, ct) == null)
                    return TeacherActionResult.Fail("Section not found.");

                var sid = f.CourseSectionId;
                var max = await _lectures.Entity().AsNoTracking().Where(l => l.CourseSectionId == sid)
                    .MaxAsync(l => (int?)l.DisplayOrder, ct) ?? 0;

                var lec = new TLecture
                {
                    LectureId = Guid.NewGuid(),
                    CourseSectionId = f.CourseSectionId,
                    LectureTitle = f.LectureTitle.Trim(),
                    Description = f.Description?.Trim(),
                    VideoUrl = video,
                    VideoDurationSeconds = seconds,
                    NotesUrl = notesUrl,
                    Notes = notes,
                    IsFreePreview = f.IsFreePreview,
                    IsPublished = f.IsPublished,
                    TopicId = f.TopicId,
                    DisplayOrder = f.DisplayOrder > 0 ? f.DisplayOrder : max + 1,
                    CreatedAt = DateTime.UtcNow
                };
                await _lectures.AddRow(lec);
                return TeacherActionResult.Ok("Lecture added.", lec.LectureId);
            }

            var l = await _lectures.OwnedLecture(_sections, _courses, f.LectureId!.Value, r.TeacherId, ct);
            if (l == null) return TeacherActionResult.Fail("Lecture not found.");

            l.LectureTitle = f.LectureTitle.Trim(); l.Description = f.Description?.Trim();
            l.VideoUrl = video; l.VideoDurationSeconds = seconds; l.NotesUrl = notesUrl; l.Notes = notes;
            l.IsFreePreview = f.IsFreePreview; l.IsPublished = f.IsPublished; l.TopicId = f.TopicId;
            if (f.DisplayOrder > 0) l.DisplayOrder = f.DisplayOrder;
            await _lectures.UpdateRow(l);
            return TeacherActionResult.Ok("Lecture updated.", l.LectureId);
        }
    }

    public record DeleteTeacherLectureCommand(Guid TeacherId, Guid LectureId) : IRequest<TeacherActionResult>;

    public class DeleteTeacherLectureCommandHandler : IRequestHandler<DeleteTeacherLectureCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        public DeleteTeacherLectureCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s, IRepository<TLecture> l)
        { _courses = c; _sections = s; _lectures = l; }

        public async Task<TeacherActionResult> Handle(DeleteTeacherLectureCommand r, CancellationToken ct)
        {
            var l = await _lectures.OwnedLecture(_sections, _courses, r.LectureId, r.TeacherId, ct);
            if (l == null) return TeacherActionResult.Fail("Lecture not found.");
            try { await _lectures.DeleteRow(l); }
            catch (DbUpdateException)
            {
                return TeacherActionResult.Fail("Is lecture se students ki progress/doubts jude hain, delete nahi ho sakta. Unpublish kar dein.");
            }
            return TeacherActionResult.Ok("Lecture deleted.");
        }
    }

    public record ToggleLectureFlagCommand(Guid TeacherId, Guid LectureId, LectureFlag Flag) : IRequest<TeacherActionResult>;

    public class ToggleLectureFlagCommandHandler : IRequestHandler<ToggleLectureFlagCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        public ToggleLectureFlagCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s, IRepository<TLecture> l)
        { _courses = c; _sections = s; _lectures = l; }

        public async Task<TeacherActionResult> Handle(ToggleLectureFlagCommand r, CancellationToken ct)
        {
            var l = await _lectures.OwnedLecture(_sections, _courses, r.LectureId, r.TeacherId, ct);
            if (l == null) return TeacherActionResult.Fail("Lecture not found.");

            string msg;
            if (r.Flag == LectureFlag.Publish)
            {
                if (!l.IsPublished && !CourseAccess.HasContent(l.VideoUrl, l.NotesUrl, l.Notes))
                    return TeacherActionResult.Fail("Publish karne se pehle video ya notes add karein.");
                l.IsPublished = !l.IsPublished;
                msg = l.IsPublished ? "Lecture published." : "Lecture unpublished.";
            }
            else
            {
                l.IsFreePreview = !l.IsFreePreview;
                msg = l.IsFreePreview ? "Free preview ON." : "Free preview OFF.";
            }
            await _lectures.UpdateRow(l);
            return TeacherActionResult.Ok(msg);
        }
    }

    public record MoveLectureCommand(Guid TeacherId, Guid LectureId, bool Up) : IRequest<TeacherActionResult>;

    public class MoveLectureCommandHandler : IRequestHandler<MoveLectureCommand, TeacherActionResult>
    {
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TCourseSection> _sections;
        private readonly IRepository<TLecture> _lectures;
        public MoveLectureCommandHandler(IRepository<TCourse> c, IRepository<TCourseSection> s, IRepository<TLecture> l)
        { _courses = c; _sections = s; _lectures = l; }

        public async Task<TeacherActionResult> Handle(MoveLectureCommand r, CancellationToken ct)
        {
            var l = await _lectures.OwnedLecture(_sections, _courses, r.LectureId, r.TeacherId, ct);
            if (l == null) return TeacherActionResult.Fail("Lecture not found.");

            var sid = l.CourseSectionId;
            var sibs = await _lectures.Entity().Where(x => x.CourseSectionId == sid)
                .OrderBy(x => x.DisplayOrder).ThenBy(x => x.CreatedAt).ToListAsync(ct);

            var i = sibs.FindIndex(x => x.LectureId == r.LectureId);
            var j = r.Up ? i - 1 : i + 1;
            if (i < 0 || j < 0 || j >= sibs.Count) return TeacherActionResult.Ok("Already at the edge.");

            (sibs[i], sibs[j]) = (sibs[j], sibs[i]);
            for (var k = 0; k < sibs.Count; k++)
                if (sibs[k].DisplayOrder != k + 1) { sibs[k].DisplayOrder = k + 1; await _lectures.UpdateRow(sibs[k]); }

            return TeacherActionResult.Ok("Order updated.");
        }
    }
}
