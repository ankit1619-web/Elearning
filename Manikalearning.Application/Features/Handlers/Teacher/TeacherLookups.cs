using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    public class TeacherLookups
    {
    }
    public enum LookupKind { Exams, Papers, Subjects, Sections, Topics, SubTopics, Passages }

    public record GetTeacherLookupQuery(LookupKind Kind, Guid? ParentId = null, string? Search = null)
        : IRequest<List<LookupItem>>;

    public class GetTeacherLookupQueryHandler : IRequestHandler<GetTeacherLookupQuery, List<LookupItem>>
    {
        private readonly IRepository<TExam> _exams;
        private readonly IRepository<TPaper> _papers;
        private readonly IRepository<TSubject> _subjects;
        private readonly IRepository<TSection> _sections;
        private readonly IRepository<TTopic> _topics;
        private readonly IRepository<TSubTopic> _subTopics;
        private readonly IRepository<TPassage> _passages;

        public GetTeacherLookupQueryHandler(IRepository<TExam> exams, IRepository<TPaper> papers,
            IRepository<TSubject> subjects, IRepository<TSection> sections, IRepository<TTopic> topics,
            IRepository<TSubTopic> subTopics, IRepository<TPassage> passages)
        {
            _exams = exams; _papers = papers; _subjects = subjects; _sections = sections;
            _topics = topics; _subTopics = subTopics; _passages = passages;
        }

        public async Task<List<LookupItem>> Handle(GetTeacherLookupQuery r, CancellationToken ct)
        {
            var parent = r.ParentId;
            var search = r.Search?.Trim();

            switch (r.Kind)
            {
                case LookupKind.Exams:
                    // ASSUMPTION: Exam.ExamName
                    return await _exams.Entity().AsNoTracking()
                        .OrderBy(x => x.ExamName)
                        .Select(x => new LookupItem(x.ExamId, x.ExamName)).ToListAsync(ct);

                case LookupKind.Papers:
                    // ASSUMPTION: Paper.PaperName, Paper.ExamId
                    if (parent == null) return new();
                    return await _papers.Entity().AsNoTracking()
                        .Where(p => p.ExamId == parent)
                        .OrderBy(p => p.PaperName)
                        .Select(p => new LookupItem(p.PaperId, p.PaperName)).ToListAsync(ct);

                case LookupKind.Subjects:
                    // Label: "Paper > Subject" (alag papers me same naam ke subjects ho sakte hain)
                    return await (from s in _subjects.Entity().AsNoTracking()
                                  from p in _papers.Entity().AsNoTracking()
                                  where s.PaperId == p.PaperId && s.IsActive
                                        && (parent == null || s.PaperId == parent)
                                  orderby p.PaperName, s.DisplayOrder
                                  select new LookupItem(s.SubjectId, p.PaperName + " > " + s.SubjectName))
                        .ToListAsync(ct);

                case LookupKind.Sections:
                    if (parent == null) return new();
                    return await _sections.Entity().AsNoTracking()
                        .Where(s => s.SubjectId == parent && s.IsActive)
                        .OrderBy(s => s.DisplayOrder)
                        .Select(s => new LookupItem(s.SectionId, s.SectionName)).ToListAsync(ct);

                case LookupKind.Topics:
                    if (parent == null) return new();
                    return await _topics.Entity().AsNoTracking()
                        .Where(t => t.SectionId == parent && t.IsActive)
                        .OrderBy(t => t.DisplayOrder)
                        .Select(t => new LookupItem(t.TopicId, t.TopicName)).ToListAsync(ct);

                case LookupKind.SubTopics:
                    if (parent == null) return new();
                    return await _subTopics.Entity().AsNoTracking()
                        .Where(t => t.TopicId == parent && t.IsActive)
                        .OrderBy(t => t.SubTopicName)
                        .Select(t => new LookupItem(t.SubTopicId, t.SubTopicName)).ToListAsync(ct);

                case LookupKind.Passages:
                    // QuestionPassages me TopicId nahi hai, isliye latest 200 (+ optional title search)
                    return await _passages.Entity().AsNoTracking()
                        .Where(p => string.IsNullOrEmpty(search) || p.Title.Contains(search))
                        .OrderByDescending(p => p.CreatedAt).Take(200)
                        .Select(p => new LookupItem(p.PassageId, p.Title)).ToListAsync(ct);

                default:
                    return new();
            }
        }
    }
}
