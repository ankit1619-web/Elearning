using Manikalearning.Application.Interface;
using Manikalearning.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    internal class Teachertests
    {
    }
    public class TeacherTestListItem
    {
        public Guid TestId { get; set; }
        public string TestName { get; set; } = "";
        public string TestType { get; set; } = "";
        public int? DurationMinutes { get; set; }
        public int QuestionCount { get; set; }
        public decimal TotalMarks { get; set; }
        public int AttemptCount { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TeacherTestForm
    {
        public Guid? TestId { get; set; }
        public string TestName { get; set; } = "";
        public string TestType { get; set; } = "Mock";
        public int? DurationMinutes { get; set; } = 60;
        public decimal NegativeMarking { get; set; }
        public bool IsRandom { get; set; }
        public bool ShowResultImmediately { get; set; } = true;
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? TopicId { get; set; }
    }

    public class TestQuestionRow
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public decimal Marks { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class TestManageDto
    {
        public Guid TestId { get; set; }
        public string TestName { get; set; } = "";
        public bool IsPublished { get; set; }
        public int? DurationMinutes { get; set; }
        public bool HasAttempts { get; set; }
        public List<TestQuestionRow> Questions { get; set; } = new();
        public decimal TotalMarks => Questions.Sum(q => q.Marks);
    }

    public class PickQuestionRow
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public decimal Marks { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string? TopicName { get; set; }
    }

    public static class TestAccess
    {
        public static Task<bool> OwnsTest(this IRepository<TTest> tests, Guid testId, Guid teacherId, CancellationToken ct)
            => tests.Entity().AsNoTracking().AnyAsync(t => t.TestId == testId && t.CreatedBy == teacherId, ct);

        public static async Task Recalc(this IRepository<TTest> tests, IRepository<TTestQuestion> tq, TTest test, CancellationToken ct)
        {
            var id = test.TestId;
            var marks = await tq.Entity().AsNoTracking().Where(x => x.TestId == id).Select(x => x.Marks).ToListAsync(ct);
            test.TotalQuestions = marks.Count;
            test.TotalMarks = marks.Sum();
            await tests.UpdateRow(test);
        }

        public static Guid? NullIfEmpty(Guid? g) => g == Guid.Empty ? null : g;
    }
    //  LIST 
    public record GetTeacherTestsQuery(Guid TeacherId) : IRequest<List<TeacherTestListItem>>;

    public class GetTeacherTestsQueryHandler : IRequestHandler<GetTeacherTestsQuery, List<TeacherTestListItem>>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestAttempt> _attempts;

        public GetTeacherTestsQueryHandler(IRepository<TTest> t, IRepository<TTestAttempt> a) { _tests = t; _attempts = a; }

        public async Task<List<TeacherTestListItem>> Handle(GetTeacherTestsQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId;
            var list = await _tests.Entity().AsNoTracking()
                .Where(t => t.CreatedBy == tid)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TeacherTestListItem
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    TestType = t.TestType,
                    DurationMinutes = t.DurationMinutes,
                    QuestionCount = t.TotalQuestions,
                    TotalMarks = t.TotalMarks,
                    IsPublished = t.IsPublished,
                    CreatedAt = t.CreatedAt
                }).ToListAsync(ct);

            var ids = list.Select(x => x.TestId).ToList();
            if (ids.Count == 0) return list;

            var att = await _attempts.Entity().AsNoTracking().Where(a => ids.Contains((Guid)a.TestId!))
                .GroupBy(a => a.TestId).Select(g => new { g.Key, N = g.Count() }).ToListAsync(ct);
            var map = att.ToDictionary(x => x.Key, x => x.N);
            foreach (var t in list) t.AttemptCount = map.GetValueOrDefault(t.TestId);
            return list;
        }
    }

    //  FORM + SAVE 
    public record GetTeacherTestFormQuery(Guid TeacherId, Guid TestId) : IRequest<TeacherTestForm?>;

    public class GetTeacherTestFormQueryHandler : IRequestHandler<GetTeacherTestFormQuery, TeacherTestForm?>
    {
        private readonly IRepository<TTest> _tests;
        public GetTeacherTestFormQueryHandler(IRepository<TTest> t) { _tests = t; }

        public async Task<TeacherTestForm?> Handle(GetTeacherTestFormQuery r, CancellationToken ct)
            => await _tests.Entity().AsNoTracking()
                .Where(t => t.TestId == r.TestId && t.CreatedBy == r.TeacherId)
                .Select(t => new TeacherTestForm
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    TestType = t.TestType,
                    DurationMinutes = t.DurationMinutes,
                    NegativeMarking = t.NegativeMarking,
                    IsRandom = t.IsRandom,
                    ShowResultImmediately = t.ShowResultImmediately,
                    ExamId = t.ExamId,
                    PaperId = t.PaperId,
                    SubjectId = t.SubjectId,
                    SectionId = t.SectionId,
                    TopicId = t.TopicId
                }).FirstOrDefaultAsync(ct);
    }

    public record SaveTeacherTestCommand(Guid TeacherId, TeacherTestForm Form) : IRequest<TeacherActionResult>;

    public class SaveTeacherTestCommandHandler : IRequestHandler<SaveTeacherTestCommand, TeacherActionResult>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        private readonly IRepository<TTestAttempt> _attempts;
        private readonly IRepository<TTopic> _topics;

        public SaveTeacherTestCommandHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq,
            IRepository<TTestAttempt> a, IRepository<TTopic> tp)
        { _tests = t; _tq = tq; _attempts = a; _topics = tp; }

        public async Task<TeacherActionResult> Handle(SaveTeacherTestCommand r, CancellationToken ct)
        {
            var f = r.Form;
            if (string.IsNullOrWhiteSpace(f.TestName)) return TeacherActionResult.Fail("Test name is required.");
            if (f.TestName.Trim().Length > 250) return TeacherActionResult.Fail("Please keep the test name under 250 characters.");
            if (string.IsNullOrWhiteSpace(f.TestType)) return TeacherActionResult.Fail("Test type is required.");
            if (f.TestType.Trim().Length > 30) return TeacherActionResult.Fail("Please keep the test type under 30 characters.");
            if (f.DurationMinutes is < 1 or > 600) return TeacherActionResult.Fail("Duration must be between 1 and 600 minutes (or left blank).");
            if (f.NegativeMarking < 0 || f.NegativeMarking > 100) return TeacherActionResult.Fail("Negative marking must be between 0 and 100.");

            f.ExamId = TestAccess.NullIfEmpty(f.ExamId); f.PaperId = TestAccess.NullIfEmpty(f.PaperId);
            f.SubjectId = TestAccess.NullIfEmpty(f.SubjectId); f.SectionId = TestAccess.NullIfEmpty(f.SectionId);
            f.TopicId = TestAccess.NullIfEmpty(f.TopicId);

            if (f.TopicId != null)
            {
                var tp = f.TopicId;
                if (!await _topics.Entity().AsNoTracking().AnyAsync(t => t.TopicId == tp, ct))
                    return TeacherActionResult.Fail("The selected topic is invalid.");
            }

            var isNew = f.TestId == null || f.TestId == Guid.Empty;
            if (isNew)
            {
                var t = new TTest
                {
                    TestId = Guid.NewGuid(),
                    TestName = f.TestName.Trim(),
                    TestType = f.TestType.Trim(),
                    ExamId = f.ExamId,
                    PaperId = f.PaperId,
                    SubjectId = f.SubjectId,
                    SectionId = f.SectionId,
                    TopicId = f.TopicId,
                    TotalQuestions = 0,
                    DurationMinutes = f.DurationMinutes,
                    TotalMarks = 0,
                    NegativeMarking = f.NegativeMarking,
                    IsRandom = f.IsRandom,
                    ShowResultImmediately = f.ShowResultImmediately,
                    IsPublished = false,
                    CreatedBy = r.TeacherId,
                    CreatedAt = DateTime.UtcNow
                };
                await _tests.AddRow(t);
                return TeacherActionResult.Ok("Test created. You can now add questions.", t.TestId);
            }

            var id = f.TestId!.Value;
            var test = await _tests.Entity().FirstOrDefaultAsync(t => t.TestId == id && t.CreatedBy == r.TeacherId, ct);
            if (test == null) return TeacherActionResult.Fail("Test not found.");

            var negChanged = test.NegativeMarking != f.NegativeMarking;
            if (negChanged && await _attempts.Entity().AsNoTracking().AnyAsync(a => a.TestId == id, ct))
                return TeacherActionResult.Fail("This test has student attempts, so negative marking cannot be changed.");

            test.TestName = f.TestName.Trim();
            test.TestType = f.TestType.Trim();
            test.ExamId = f.ExamId; test.PaperId = f.PaperId; test.SubjectId = f.SubjectId;
            test.SectionId = f.SectionId; test.TopicId = f.TopicId;
            test.DurationMinutes = f.DurationMinutes;
            test.NegativeMarking = f.NegativeMarking;
            test.IsRandom = f.IsRandom;
            test.ShowResultImmediately = f.ShowResultImmediately;
            await _tests.UpdateRow(test);

            if (negChanged)
            {
                // Sync each question's negative marks with the new test value
                var rows = await _tq.Entity().Where(x => x.TestId == id).ToListAsync(ct);
                foreach (var row in rows) { row.NegativeMarks = f.NegativeMarking; await _tq.UpdateRow(row); }
            }
            return TeacherActionResult.Ok("Test updated.", test.TestId);
        }
    }

    //  PUBLISH / DELETE 
    public record ToggleTestPublishCommand(Guid TeacherId, Guid TestId) : IRequest<TeacherActionResult>;

    public class ToggleTestPublishCommandHandler : IRequestHandler<ToggleTestPublishCommand, TeacherActionResult>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        public ToggleTestPublishCommandHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq) { _tests = t; _tq = tq; }

        public async Task<TeacherActionResult> Handle(ToggleTestPublishCommand r, CancellationToken ct)
        {
            var test = await _tests.Entity().FirstOrDefaultAsync(t => t.TestId == r.TestId && t.CreatedBy == r.TeacherId, ct);
            if (test == null) return TeacherActionResult.Fail("Test not found.");

            if (!test.IsPublished)
            {
                var id = test.TestId;
                if (!await _tq.Entity().AsNoTracking().AnyAsync(x => x.TestId == id, ct))
                    return TeacherActionResult.Fail("Before publishing, please add at least one question.");
            }
            test.IsPublished = !test.IsPublished;
            await _tests.UpdateRow(test);
            return TeacherActionResult.Ok(test.IsPublished ? "Test published." : "Test unpublished.");
        }
    }

    public record DeleteTeacherTestCommand(Guid TeacherId, Guid TestId) : IRequest<TeacherActionResult>;

    public class DeleteTeacherTestCommandHandler : IRequestHandler<DeleteTeacherTestCommand, TeacherActionResult>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        private readonly IRepository<TTestAttempt> _attempts;
        public DeleteTeacherTestCommandHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq, IRepository<TTestAttempt> a)
        { _tests = t; _tq = tq; _attempts = a; }

        public async Task<TeacherActionResult> Handle(DeleteTeacherTestCommand r, CancellationToken ct)
        {
            var test = await _tests.Entity().FirstOrDefaultAsync(t => t.TestId == r.TestId && t.CreatedBy == r.TeacherId, ct);
            if (test == null) return TeacherActionResult.Fail("Test not found.");

            var id = test.TestId;
            if (await _attempts.Entity().AsNoTracking().AnyAsync(a => a.TestId == id, ct))
                return TeacherActionResult.Fail("Students have attempted this test, so it cannot be deleted. Please unpublish it instead.");

            var rows = await _tq.Entity().Where(x => x.TestId == id).ToListAsync(ct);
            try
            {
                await _tq.DeleteRows(rows);
                await _tests.DeleteRow(test);
            }
            catch (DbUpdateException)
            {
                return TeacherActionResult.Fail("Could not delete the test as related records exist. Please unpublish it instead.");
            }
            return TeacherActionResult.Ok("Test deleted.");
        }
    }

    //  MANAGE (questions in a test) 
    public record GetTestManageQuery(Guid TeacherId, Guid TestId) : IRequest<TestManageDto?>;

    public class GetTestManageQueryHandler : IRequestHandler<GetTestManageQuery, TestManageDto?>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TTestAttempt> _attempts;

        public GetTestManageQueryHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq, IRepository<TQuestion> q, IRepository<TTestAttempt> a)
        { _tests = t; _tq = tq; _questions = q; _attempts = a; }

        public async Task<TestManageDto?> Handle(GetTestManageQuery r, CancellationToken ct)
        {
            var test = await _tests.Entity().AsNoTracking()
                .Where(t => t.TestId == r.TestId && t.CreatedBy == r.TeacherId)
                .Select(t => new { t.TestId, t.TestName, t.IsPublished, t.DurationMinutes }).FirstOrDefaultAsync(ct);
            if (test == null) return null;

            var id = test.TestId;
            var rows = await (from x in _tq.Entity().AsNoTracking()
                              from q in _questions.Entity().AsNoTracking()
                              where x.TestId == id && x.QuestionId == q.QuestionId
                              orderby x.QuestionOrder
                              select new TestQuestionRow
                              {
                                  QuestionId = q.QuestionId,
                                  QuestionText = q.QuestionText,
                                  Marks = x.Marks,
                                  Difficulty = q.Difficulty,
                                  DisplayOrder = x.QuestionOrder
                              }).ToListAsync(ct);

            return new TestManageDto
            {
                TestId = test.TestId,
                TestName = test.TestName,
                IsPublished = test.IsPublished,
                DurationMinutes = test.DurationMinutes,
                HasAttempts = await _attempts.Entity().AsNoTracking().AnyAsync(a => a.TestId == id, ct),
                Questions = rows
            };
        }
    }

    public record GetPickQuestionsQuery(Guid TeacherId, Guid TestId, string? Search = null, int Page = 1, int PageSize = 10)
        : IRequest<PagedResult<PickQuestionRow>?>;

    public class GetPickQuestionsQueryHandler : IRequestHandler<GetPickQuestionsQuery, PagedResult<PickQuestionRow>?>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TTopic> _topics;

        public GetPickQuestionsQueryHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq, IRepository<TQuestion> q, IRepository<TTopic> tp)
        { _tests = t; _tq = tq; _questions = q; _topics = tp; }

        public async Task<PagedResult<PickQuestionRow>?> Handle(GetPickQuestionsQuery r, CancellationToken ct)
        {
            if (!await _tests.OwnsTest(r.TestId, r.TeacherId, ct)) return null;

            var tid = r.TeacherId; var testId = r.TestId;
            var search = r.Search?.Trim();
            var page = Math.Max(1, r.Page);
            var size = Math.Clamp(r.PageSize, 5, 50);

            var used = _tq.Entity().AsNoTracking().Where(x => x.TestId == testId).Select(x => x.QuestionId);
            var q = _questions.Entity().AsNoTracking()
                .Where(x => x.CreatedBy == tid && x.IsActive && !used.Contains(x.QuestionId)
                            && (string.IsNullOrEmpty(search) || x.QuestionText.Contains(search)));

            var total = await q.CountAsync(ct);
            var rows = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * size).Take(size)
                .Select(x => new { x.QuestionId, x.QuestionText, x.Marks, x.TopicId, Difficulty = x.Difficulty })
                .ToListAsync(ct);

            var tIds = rows.Where(x => x.TopicId != null).Select(x => x.TopicId!.Value).Distinct().ToList();
            var names = await _topics.Entity().AsNoTracking().Where(t => tIds.Contains(t.TopicId))
                .Select(t => new { t.TopicId, t.TopicName }).ToListAsync(ct);
            var map = names.ToDictionary(x => x.TopicId, x => x.TopicName);

            return new PagedResult<PickQuestionRow>
            {
                Page = page,
                PageSize = size,
                TotalCount = total,
                Items = rows.Select(x => new PickQuestionRow
                {
                    QuestionId = x.QuestionId,
                    QuestionText = x.QuestionText,
                    Marks = x.Marks,
                    Difficulty = x.Difficulty,
                    TopicName = x.TopicId != null ? map.GetValueOrDefault(x.TopicId.Value) : null
                }).ToList()
            };
        }
    }

    public record AddQuestionsToTestCommand(Guid TeacherId, Guid TestId, List<Guid> QuestionIds) : IRequest<TeacherActionResult>;

    public class AddQuestionsToTestCommandHandler : IRequestHandler<AddQuestionsToTestCommand, TeacherActionResult>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TTestAttempt> _attempts;

        public AddQuestionsToTestCommandHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq, IRepository<TQuestion> q, IRepository<TTestAttempt> a)
        { _tests = t; _tq = tq; _questions = q; _attempts = a; }

        public async Task<TeacherActionResult> Handle(AddQuestionsToTestCommand r, CancellationToken ct)
        {
            var testId = r.TestId; var tid = r.TeacherId;
            var test = await _tests.Entity().FirstOrDefaultAsync(t => t.TestId == testId && t.CreatedBy == tid, ct);
            if (test == null) return TeacherActionResult.Fail("Test not found.");

            var ids = r.QuestionIds.Distinct().ToList();
            if (ids.Count == 0) return TeacherActionResult.Fail("Please select at least one question.");
            if (ids.Count > 100) return TeacherActionResult.Fail("You can add a maximum of 100 questions at a time.");

            if (await _attempts.Entity().AsNoTracking().AnyAsync(a => a.TestId == testId, ct))
                return TeacherActionResult.Fail("Students have attempted this test, so questions cannot be modified.");

            var valid = await _questions.Entity().AsNoTracking()
                .Where(q => ids.Contains(q.QuestionId) && q.CreatedBy == tid && q.IsActive)
                .Select(q => new { q.QuestionId, q.Marks }).ToListAsync(ct);
            var existing = await _tq.Entity().AsNoTracking().Where(x => x.TestId == testId)
                .Select(x => x.QuestionId).ToListAsync(ct);
            var toAdd = valid.Where(v => !existing.Contains(v.QuestionId)).ToList();
            if (toAdd.Count == 0) return TeacherActionResult.Fail("Selected questions cannot be added (they may be inactive or already added).");

            var max = await _tq.Entity().AsNoTracking().Where(x => x.TestId == testId)
                .MaxAsync(x => (int?)x.QuestionOrder, ct) ?? 0;

            var rows = toAdd.Select(v => new TTestQuestion
            {
                TestQuestionId = Guid.NewGuid(),
                TestId = testId,
                QuestionId = v.QuestionId,
                QuestionOrder = ++max,
                Marks = v.Marks,
                NegativeMarks = test.NegativeMarking     // Test negative marking applies to each question
            }).ToList();
            await _tq.AddRows(rows);
            await _tests.Recalc(_tq, test, ct);
            return TeacherActionResult.Ok(rows.Count + " question(s) added.");
        }
    }

    public record RemoveTestQuestionCommand(Guid TeacherId, Guid TestId, Guid QuestionId) : IRequest<TeacherActionResult>;

    public class RemoveTestQuestionCommandHandler : IRequestHandler<RemoveTestQuestionCommand, TeacherActionResult>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        private readonly IRepository<TTestAttempt> _attempts;

        public RemoveTestQuestionCommandHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq, IRepository<TTestAttempt> a)
        { _tests = t; _tq = tq; _attempts = a; }

        public async Task<TeacherActionResult> Handle(RemoveTestQuestionCommand r, CancellationToken ct)
        {
            var testId = r.TestId; var qid = r.QuestionId; var tid = r.TeacherId;
            var test = await _tests.Entity().FirstOrDefaultAsync(t => t.TestId == testId && t.CreatedBy == tid, ct);
            if (test == null) return TeacherActionResult.Fail("Test not found.");

            if (await _attempts.Entity().AsNoTracking().AnyAsync(a => a.TestId == testId, ct))
                return TeacherActionResult.Fail("Students have attempted this test, so questions cannot be modified.");

            var row = await _tq.Entity().FirstOrDefaultAsync(x => x.TestId == testId && x.QuestionId == qid, ct);
            if (row == null) return TeacherActionResult.Fail("Question not found in this test.");
            await _tq.DeleteRow(row);

            var rest = await _tq.Entity().Where(x => x.TestId == testId).OrderBy(x => x.QuestionOrder).ToListAsync(ct);
            for (var k = 0; k < rest.Count; k++)
                if (rest[k].QuestionOrder != k + 1) { rest[k].QuestionOrder = k + 1; await _tq.UpdateRow(rest[k]); }

            await _tests.Recalc(_tq, test, ct);
            return TeacherActionResult.Ok("Question removed.");
        }
    }

    public record MoveTestQuestionCommand(Guid TeacherId, Guid TestId, Guid QuestionId, bool Up) : IRequest<TeacherActionResult>;

    public class MoveTestQuestionCommandHandler : IRequestHandler<MoveTestQuestionCommand, TeacherActionResult>
    {
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestQuestion> _tq;
        public MoveTestQuestionCommandHandler(IRepository<TTest> t, IRepository<TTestQuestion> tq) { _tests = t; _tq = tq; }

        public async Task<TeacherActionResult> Handle(MoveTestQuestionCommand r, CancellationToken ct)
        {
            if (!await _tests.OwnsTest(r.TestId, r.TeacherId, ct)) return TeacherActionResult.Fail("Test not found.");
            var testId = r.TestId;
            var sibs = await _tq.Entity().Where(x => x.TestId == testId).OrderBy(x => x.QuestionOrder).ToListAsync(ct);

            var i = sibs.FindIndex(x => x.QuestionId == r.QuestionId);
            var j = r.Up ? i - 1 : i + 1;
            if (i < 0 || j < 0 || j >= sibs.Count) return TeacherActionResult.Ok("Already at the edge.");

            (sibs[i], sibs[j]) = (sibs[j], sibs[i]);
            for (var k = 0; k < sibs.Count; k++)
                if (sibs[k].QuestionOrder != k + 1) { sibs[k].QuestionOrder = k + 1; await _tq.UpdateRow(sibs[k]); }

            return TeacherActionResult.Ok("Order updated.");
        }
    }
}