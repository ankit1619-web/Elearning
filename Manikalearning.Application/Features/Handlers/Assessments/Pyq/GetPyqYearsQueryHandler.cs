using FluentValidation;
using Manikalearning.Application.Interface;
using Manikalearning.Application.DTOs;
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
    public record GetPyqYearsQuery(Guid? ExamId = null, Guid? PaperId = null) : IRequest<PyqYearsDto>;

    public class GetPyqYearsQueryValidator : AbstractValidator<GetPyqYearsQuery>
    {
        public GetPyqYearsQueryValidator()
        {
            RuleFor(x => x.ExamId).Must(g => g == null || g != Guid.Empty);
            RuleFor(x => x.PaperId).Must(g => g == null || g != Guid.Empty);
        }
    }

    public class GetPyqYearsQueryHandler : IRequestHandler<GetPyqYearsQuery, PyqYearsDto>
    {
        private readonly IRepository<Question> _questions;
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<Paper> _papers;

        public GetPyqYearsQueryHandler(IRepository<Question> questions, IRepository<Exam> exams, IRepository<Paper> papers)
        { _questions = questions; _exams = exams; _papers = papers; }

        public async Task<PyqYearsDto> Handle(GetPyqYearsQuery req, CancellationToken ct)
        {
            var exams = await _exams.Entity().AsNoTracking().Where(e => e.IsActive)
                .OrderBy(e => e.DisplayOrder).ThenBy(e => e.ExamName)
                .Select(e => new ExamOptionDto { ExamId = e.ExamId, ExamName = e.ExamName }).ToListAsync(ct);
             
            var papersQ = from p in _papers.Entity().AsNoTracking()
                          join e in _exams.Entity().AsNoTracking() on p.ExamId equals e.ExamId
                          where p.IsActive && e.IsActive
                          select new { p, e };
            if (req.ExamId.HasValue) papersQ = papersQ.Where(x => x.p.ExamId == req.ExamId.Value);

            var papers = await papersQ.OrderBy(x => x.e.DisplayOrder).ThenBy(x => x.p.DisplayOrder)
                .Select(x => new PaperOptionDto { PaperId = x.p.PaperId, ExamId = x.p.ExamId, ExamName = x.e.ExamName, PaperName = x.p.PaperName })
                .ToListAsync(ct);

            var q = PyqYearRules.Base(_questions.Entity().AsNoTracking());
            if (req.ExamId.HasValue) q = q.Where(x => x.ExamId == req.ExamId);
            if (req.PaperId.HasValue) q = q.Where(x => x.PaperId == req.PaperId);

            var rows = await q.GroupBy(x => new { x.PYQYear, x.PYQSession })
                .Select(g => new { Year = g.Key.PYQYear, Session = g.Key.PYQSession, Count = g.Count() }).ToListAsync(ct);

            var years = rows.GroupBy(r => r.Year!.Value).OrderByDescending(g => g.Key)
                .Select(g => new PyqYearRowDto
                {
                    Year = g.Key,
                    Total = g.Sum(r => r.Count),
                    Sessions = g.Where(r => !string.IsNullOrWhiteSpace(r.Session))
                                .OrderBy(r => r.Session)
                                .Select(r => new PyqSessionDto { Session = r.Session!, Count = r.Count }).ToList()
                }).ToList();

            return new PyqYearsDto { ExamId = req.ExamId, PaperId = req.PaperId, Exams = exams, Papers = papers, Years = years };
        }
    }
}
