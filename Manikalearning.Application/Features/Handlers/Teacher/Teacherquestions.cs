using Manikalearning.Application.Interface;
using Manikalearning.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    internal class Teacherquestions
    {
    }
    public class TeacherQuestionListItem
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public string? TopicName { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public decimal Marks { get; set; }
        public int OptionCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class QuestionOptionForm
    {
        public Guid? OptionId { get; set; }
        public string OptionLabel { get; set; } = "";
        public string OptionText { get; set; } = "";
        public string? OptionTextHindi { get; set; }
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class TeacherQuestionForm
    {
        public Guid? QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public string QuestionText { get; set; } = "";
        public string? QuestionTextHindi { get; set; }
        public string? ShortExplanation { get; set; }
        public string? Explanation { get; set; }
        public decimal Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; }
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;
        public Guid? TopicId { get; set; }
        public Guid? SubTopicId { get; set; }
        public Guid? PassageId { get; set; }
        public bool IsPYQ { get; set; }
        public string? SourceName { get; set; }
        public int? PYQYear { get; set; }
        public string? PYQSession { get; set; }
        public bool IsActive { get; set; } = true;

        public int? CorrectOptionIndex { get; set; }
        public List<QuestionOptionForm> Options { get; set; } = new();

        public Guid? PickSubjectId { get; set; }
        public Guid? PickSectionId { get; set; }
    }

    public static class QuestionTypeInfo
    {
        public static (bool IsValid, bool IsMsq, bool IsNumerical) Resolve(QuestionType value)
        {
            var name = value.ToString();
            if (name.Length == 0 || char.IsDigit(name[0]) || name[0] == '-') return (false, false, false);
            var u = name.ToUpperInvariant();
            var msq = u.Contains("MULTIPLE") || u.Contains("MSQ");
            var num = u.Contains("NUMERICAL") || u.Contains("NAT") || u.Contains("SUBJECTIVE");
            return (true, msq, num);
        }

        public static bool IsValidEnum<T>(DifficultyLevel value) where T : struct, Enum
        {
            var name = Enum.ToObject(typeof(T), value).ToString() ?? "";
            return name.Length > 0 && !char.IsDigit(name[0]) && name[0] != '-';
        }

    }

    // LIST 
    public record GetTeacherQuestionsQuery(Guid TeacherId, string? Search = null, Guid? TopicId = null,
        int Page = 1, int PageSize = 10) : IRequest<PagedResult<TeacherQuestionListItem>>;

    public class GetTeacherQuestionsQueryHandler : IRequestHandler<GetTeacherQuestionsQuery, PagedResult<TeacherQuestionListItem>>
    {
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TOption> _options;
        private readonly IRepository<TTopic> _topics;

        public GetTeacherQuestionsQueryHandler(IRepository<TQuestion> q, IRepository<TOption> o, IRepository<TTopic> t)
        { _questions = q; _options = o; _topics = t; }

        public async Task<PagedResult<TeacherQuestionListItem>> Handle(GetTeacherQuestionsQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId;
            var search = r.Search?.Trim();
            var topicId = r.TopicId;
            var page = Math.Max(1, r.Page);
            var size = Math.Clamp(r.PageSize, 5, 50);

            var q = _questions.Entity().AsNoTracking()
                .Where(x => x.CreatedBy == tid
                            && (string.IsNullOrEmpty(search) || x.QuestionText.Contains(search))
                            && (topicId == null || x.TopicId == topicId));

            var total = await q.CountAsync(ct);

            var rows = await q.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .Select(x => new
                {
                    x.QuestionId,
                    x.QuestionText,
                    x.TopicId,
                    x.Marks,
                    x.IsActive,
                    x.CreatedAt,
                    Difficulty = x.Difficulty
                }).ToListAsync(ct);

            var qIds = rows.Select(x => x.QuestionId).ToList();
            var tIds = rows.Where(x => x.TopicId != null).Select(x => x.TopicId!.Value).Distinct().ToList();

            var optCounts = await _options.Entity().AsNoTracking()
                .Where(o => qIds.Contains(o.QuestionId))
                .GroupBy(o => o.QuestionId).Select(g => new { g.Key, N = g.Count() }).ToListAsync(ct);
            var optMap = optCounts.ToDictionary(x => x.Key, x => x.N);

            var topicNames = await _topics.Entity().AsNoTracking()
                .Where(t => tIds.Contains(t.TopicId))
                .Select(t => new { t.TopicId, t.TopicName }).ToListAsync(ct);
            var topicMap = topicNames.ToDictionary(x => x.TopicId, x => x.TopicName);

            return new PagedResult<TeacherQuestionListItem>
            {
                Page = page,
                PageSize = size,
                TotalCount = total,
                Items = rows.Select(x => new TeacherQuestionListItem
                {
                    QuestionId = x.QuestionId,
                    QuestionText = x.QuestionText,
                    TopicName = x.TopicId != null ? topicMap.GetValueOrDefault(x.TopicId.Value) : null,
                    Difficulty = x.Difficulty,
                    Marks = x.Marks,
                    OptionCount = optMap.GetValueOrDefault(x.QuestionId),
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }

    // FORM (load for edit) 
    public record GetTeacherQuestionFormQuery(Guid TeacherId, Guid QuestionId) : IRequest<TeacherQuestionForm?>;

    public class GetTeacherQuestionFormQueryHandler : IRequestHandler<GetTeacherQuestionFormQuery, TeacherQuestionForm?>
    {
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TOption> _options;
        private readonly IRepository<TTopic> _topics;
        private readonly IRepository<TSection> _curSections;

        public GetTeacherQuestionFormQueryHandler(IRepository<TQuestion> q, IRepository<TOption> o,
            IRepository<TTopic> t, IRepository<TSection> cs)
        { _questions = q; _options = o; _topics = t; _curSections = cs; }

        public async Task<TeacherQuestionForm?> Handle(GetTeacherQuestionFormQuery r, CancellationToken ct)
        {
            var q = await _questions.Entity().AsNoTracking()
                .FirstOrDefaultAsync(x => x.QuestionId == r.QuestionId && x.CreatedBy == r.TeacherId, ct);
            if (q == null) return null;

            var opts = await _options.Entity().AsNoTracking()
                .Where(o => o.QuestionId == q.QuestionId)
                .OrderBy(o => o.DisplayOrder).ToListAsync(ct);

            var form = new TeacherQuestionForm
            {
                QuestionId = q.QuestionId,
                QuestionType = q.QuestionType,
                QuestionText = q.QuestionText,
                QuestionTextHindi = q.QuestionTextHindi,
                ShortExplanation = q.ShortExplanation,
                Explanation = q.Explanation,
                Marks = q.Marks,
                NegativeMarks = q.NegativeMarks,
                Difficulty = q.Difficulty,
                TopicId = q.TopicId,
                SubTopicId = q.SubTopicId,
                PassageId = q.PassageId,
                IsPYQ = q.IsPYQ,
                SourceName = q.SourceName,
                PYQYear = q.PYQYear,
                PYQSession = q.PYQSession,
                IsActive = q.IsActive,
                Options = opts.Select((o, i) => new QuestionOptionForm
                {
                    OptionId = o.OptionId,
                    OptionLabel = o.OptionLabel,
                    OptionText = o.OptionText,
                    OptionTextHindi = o.OptionTextHindi,
                    IsCorrect = o.IsCorrect,
                    DisplayOrder = i + 1
                }).ToList(),
                CorrectOptionIndex = opts.FindIndex(o => o.IsCorrect) is var ix && ix >= 0 ? ix : null
            };

            var topicId = q.TopicId;
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

    // SAVE (create + update) 
    public record SaveTeacherQuestionCommand(Guid TeacherId, TeacherQuestionForm Form) : IRequest<TeacherActionResult>;

    public class SaveTeacherQuestionCommandHandler : IRequestHandler<SaveTeacherQuestionCommand, TeacherActionResult>
    {
        private static readonly string[] Labels = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TOption> _options;
        private readonly IRepository<TTopic> _topics;
        private readonly IRepository<TSubTopic> _subTopics;
        private readonly IRepository<TPassage> _passages;

        public SaveTeacherQuestionCommandHandler(IRepository<TQuestion> q, IRepository<TOption> o,
            IRepository<TTopic> t, IRepository<TSubTopic> st, IRepository<TPassage> p)
        { _questions = q; _options = o; _topics = t; _subTopics = st; _passages = p; }

        public async Task<TeacherActionResult> Handle(SaveTeacherQuestionCommand r, CancellationToken ct)
        {
            var f = r.Form;

            var (typeOk, isMsq, isNum) = QuestionTypeInfo.Resolve(f.QuestionType);
            if (!typeOk) return TeacherActionResult.Fail("Invalid question type.");
            if (!QuestionTypeInfo.IsValidEnum<DifficultyLevel>(f.Difficulty)) return TeacherActionResult.Fail("Invalid difficulty level.");

            if (string.IsNullOrWhiteSpace(f.QuestionText)) return TeacherActionResult.Fail("Question text is required.");
            if (f.QuestionText.Length > 4000) return TeacherActionResult.Fail("Please keep the question text under 4000 characters.");
            if (f.QuestionTextHindi?.Length > 4000) return TeacherActionResult.Fail("Please keep the Hindi question text under 4000 characters.");
            if (f.Explanation?.Length > 4000) return TeacherActionResult.Fail("Please keep the solution under 4000 characters.");
            if (f.ShortExplanation?.Length > 500) return TeacherActionResult.Fail("Please keep the short hint under 500 characters.");
            if (f.Marks <= 0 || f.Marks > 100) return TeacherActionResult.Fail("Marks must be greater than 0 and up to 100.");
            if (f.NegativeMarks < 0 || f.NegativeMarks > f.Marks) return TeacherActionResult.Fail("Negative marks must be between 0 and the total marks.");
            if (f.TopicId == null || f.TopicId == Guid.Empty) return TeacherActionResult.Fail("Selecting a topic is required.");

            if (f.IsPYQ)
            {
                if (f.PYQYear is < 1950 or > 2100) return TeacherActionResult.Fail("PYQ year must be between 1950 and 2100.");
                if (f.PYQSession?.Length > 100) return TeacherActionResult.Fail("Please keep the PYQ session under 100 characters.");
            }
            if (f.SourceName?.Length > 200) return TeacherActionResult.Fail("Please keep the source under 200 characters.");

            // Options: remove empty options; numerical questions do not use options
            var cleaned = new List<(QuestionOptionForm O, bool Correct)>();
            if (!isNum)
            {
                for (var i = 0; i < f.Options.Count; i++)
                {
                    var o = f.Options[i];
                    if (string.IsNullOrWhiteSpace(o.OptionText)) continue;
                    cleaned.Add((o, isMsq ? o.IsCorrect : i == f.CorrectOptionIndex));
                }

                if (cleaned.Count < 2) return TeacherActionResult.Fail("At least 2 options are required.");
                if (cleaned.Count > 10) return TeacherActionResult.Fail("A maximum of 10 options are allowed.");
                if (cleaned.Any(x => x.O.OptionText.Trim().Length > 1000 || x.O.OptionTextHindi?.Length > 1000))
                    return TeacherActionResult.Fail("Please keep each option under 1000 characters.");

                var correct = cleaned.Count(x => x.Correct);
                if (isMsq && correct < 1) return TeacherActionResult.Fail("Please select at least one correct option.");
                if (!isMsq && correct != 1) return TeacherActionResult.Fail("Please select exactly one correct option (and ensure it is not empty).");
            }

            var tid = f.TopicId;
            if (!await _topics.Entity().AsNoTracking().AnyAsync(t => t.TopicId == tid, ct))
                return TeacherActionResult.Fail("The selected topic is invalid.");

            if (f.SubTopicId == Guid.Empty) f.SubTopicId = null;
            if (f.SubTopicId != null)
            {
                var sid = f.SubTopicId;
                if (!await _subTopics.Entity().AsNoTracking().AnyAsync(s => s.SubTopicId == sid && s.TopicId == tid, ct))
                    return TeacherActionResult.Fail("The selected sub-topic is invalid.");
            }
            if (f.PassageId == Guid.Empty) f.PassageId = null;
            if (f.PassageId != null)
            {
                var pid = f.PassageId;
                if (!await _passages.Entity().AsNoTracking().AnyAsync(p => p.PassageId == pid, ct))
                    return TeacherActionResult.Fail("The selected passage is invalid.");
            }

            var isNew = f.QuestionId == null || f.QuestionId == Guid.Empty;
            TQuestion q;
            if (isNew)
            {
                // IsApproved defaults to false; an admin will review and approve it
                q = new TQuestion { QuestionId = Guid.NewGuid(), CreatedBy = r.TeacherId, CreatedAt = DateTime.UtcNow };
            }
            else
            {
                var id = f.QuestionId!.Value;
                q = (await _questions.Entity().FirstOrDefaultAsync(x => x.QuestionId == id && x.CreatedBy == r.TeacherId, ct))!;
                if (q == null) return TeacherActionResult.Fail("Question not found.");
            }

            q.QuestionType = Conv.Cast(f.QuestionType, q.QuestionType);
            q.QuestionText = f.QuestionText.Trim();
            q.QuestionTextHindi = string.IsNullOrWhiteSpace(f.QuestionTextHindi) ? null : f.QuestionTextHindi.Trim();
            q.ShortExplanation = string.IsNullOrWhiteSpace(f.ShortExplanation) ? null : f.ShortExplanation.Trim();
            q.Explanation = string.IsNullOrWhiteSpace(f.Explanation) ? null : f.Explanation.Trim();
            q.Marks = f.Marks;
            q.NegativeMarks = f.NegativeMarks;
            q.Difficulty = Conv.Cast(f.Difficulty, q.Difficulty);
            q.TopicId = f.TopicId;
            q.SubTopicId = f.SubTopicId;
            q.PassageId = f.PassageId;
            q.IsPYQ = f.IsPYQ;
            q.SourceName = string.IsNullOrWhiteSpace(f.SourceName) ? null : f.SourceName.Trim();
            q.PYQYear = f.IsPYQ ? f.PYQYear : null;
            q.PYQSession = f.IsPYQ && !string.IsNullOrWhiteSpace(f.PYQSession) ? f.PYQSession.Trim() : null;
            q.IsActive = f.IsActive;

            if (isNew) await _questions.AddRow(q); else await _questions.UpdateRow(q);

            // Options: remove old options and insert new ones (simple and duplicate-order safe)
            if (!isNew)
            {
                var qid = q.QuestionId;
                var old = await _options.Entity().Where(o => o.QuestionId == qid).ToListAsync(ct);
                try { await _options.DeleteRows(old); }
                catch (DbUpdateException)
                {
                    return TeacherActionResult.Fail("Could not update options as student answers are linked to them.");
                }
            }

            if (cleaned.Count > 0)
            {
                var order = 1;
                var newOpts = cleaned.Select(x => new TOption
                {
                    OptionId = Guid.NewGuid(),
                    QuestionId = q.QuestionId,
                    OptionLabel = Labels[order - 1],
                    OptionText = x.O.OptionText.Trim(),
                    OptionTextHindi = string.IsNullOrWhiteSpace(x.O.OptionTextHindi) ? null : x.O.OptionTextHindi.Trim(),
                    IsCorrect = x.Correct,
                    DisplayOrder = order++
                }).ToList();
                await _options.AddRows(newOpts);
            }

            return TeacherActionResult.Ok(isNew ? "Question added." : "Question updated.", q.QuestionId);
        }
    }

    // =====================================================================
    // DELETE
    // =====================================================================
    public record DeleteTeacherQuestionCommand(Guid TeacherId, Guid QuestionId) : IRequest<TeacherActionResult>;

    public class DeleteTeacherQuestionCommandHandler : IRequestHandler<DeleteTeacherQuestionCommand, TeacherActionResult>
    {
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TOption> _options;
        private readonly IRepository<TTestQuestion> _testQuestions;

        public DeleteTeacherQuestionCommandHandler(IRepository<TQuestion> q, IRepository<TOption> o, IRepository<TTestQuestion> tq)
        { _questions = q; _options = o; _testQuestions = tq; }

        public async Task<TeacherActionResult> Handle(DeleteTeacherQuestionCommand r, CancellationToken ct)
        {
            var q = await _questions.Entity().FirstOrDefaultAsync(x => x.QuestionId == r.QuestionId && x.CreatedBy == r.TeacherId, ct);
            if (q == null) return TeacherActionResult.Fail("Question not found.");

            var qid = q.QuestionId;
            if (await _testQuestions.Entity().AsNoTracking().AnyAsync(x => x.QuestionId == qid, ct))
                return TeacherActionResult.Fail("This question is used in a test and cannot be deleted. Please mark it as inactive instead.");

            var opts = await _options.Entity().Where(o => o.QuestionId == qid).ToListAsync(ct);
            try
            {
                await _options.DeleteRows(opts);
                await _questions.DeleteRow(q);
            }
            catch (DbUpdateException)
            {
                return TeacherActionResult.Fail("Could not delete the question as related records exist. Please mark it as inactive instead.");
            }
            return TeacherActionResult.Ok("Question deleted.");
        }
    }
}