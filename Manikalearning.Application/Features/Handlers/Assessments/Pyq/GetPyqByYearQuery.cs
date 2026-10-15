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

namespace Manikalearning.Application.Features.Handlers.Assessments.Pyq
{
    public record GetPyqByYearQuery(int Year, Guid? ExamId = null, Guid? PaperId = null, string? Session = null, Guid? SubjectId = null) : IRequest<TopicPyqListDto>;

    public class GetPyqByYearQueryValidator : AbstractValidator<GetPyqByYearQuery>
    {
        public GetPyqByYearQueryValidator()
        {
            RuleFor(x => x.Year).InclusiveBetween(1990, 2100);
            RuleFor(x => x.Session).MaximumLength(100);
        }
    }

    public class GetPyqByYearQueryHandler : IRequestHandler<GetPyqByYearQuery, TopicPyqListDto>
    {
        private readonly IRepository<Question> _questions;
        private readonly IRepository<Exam> _exams;

        public GetPyqByYearQueryHandler(IRepository<Question> questions, IRepository<Exam> exams)
        { _questions = questions; _exams = exams; }

        public async Task<TopicPyqListDto> Handle(GetPyqByYearQuery req, CancellationToken ct)
        {
            var session = string.IsNullOrWhiteSpace(req.Session) ? null : req.Session.Trim();

            var q = PyqYearRules.Base(_questions.Entity().AsNoTracking().Include(x => x.Options))
                .Where(x => x.PYQYear == req.Year);

            if (req.ExamId.HasValue) q = q.Where(x => x.ExamId == req.ExamId);
            if (req.PaperId.HasValue) q = q.Where(x => x.PaperId == req.PaperId);
            if (req.SubjectId.HasValue) q = q.Where(x => x.SubjectId == req.SubjectId);
            if (session != null) q = q.Where(x => x.PYQSession == session);
             
            var list = await q.OrderBy(x => x.PaperId).ThenBy(x => x.PYQSession).ThenBy(x => x.CreatedAt).ThenBy(x => x.QuestionId)
                              .Take(PyqYearRules.MaxQuestionsPerTest).ToListAsync(ct);

            var examIds = list.Where(x => x.ExamId != null).Select(x => x.ExamId!.Value).Distinct().ToList();
            var examNames = await _exams.Entity().AsNoTracking().Where(e => examIds.Contains(e.ExamId))
                .ToDictionaryAsync(e => e.ExamId, e => e.ExamName, ct);

            return new TopicPyqListDto
            {
                IsPreviewOnly = false,                 
                BannerMessage = null,
                Questions = list.Select(x =>
                {
                    var opts = x.Options.OrderBy(o => o.DisplayOrder).Select(o => o.OptionText).ToList();
                    var label = !string.IsNullOrWhiteSpace(x.SourceName) ? x.SourceName
                              : (x.ExamId != null && examNames.TryGetValue(x.ExamId.Value, out var en) ? en : "PYQ");

                    return new TopicPyqDto
                    {
                        QuestionId = x.QuestionId,
                        QuestionText = x.QuestionText,
                        QuestionTextHindi = x.QuestionTextHindi,
                        OptionA = opts.ElementAtOrDefault(0),
                        OptionB = opts.ElementAtOrDefault(1),
                        OptionC = opts.ElementAtOrDefault(2),
                        OptionD = opts.ElementAtOrDefault(3),
                        Explanation = x.Explanation,
                        ExplanationHindi = x.ExplanationHindi, 
                        ExamName = $"{label} {x.PYQYear}" + (string.IsNullOrWhiteSpace(x.PYQSession) ? "" : $" - {x.PYQSession}")
                    };
                }).ToList()
            };
        }
    }
}
