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

namespace Manikalearning.Application.Features.Commands.Students
{
    public class PyqBankSubjectDto
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public Guid ExamId { get; set; }
        public string ExamName { get; set; } = "";
        public string PaperName { get; set; } = "";
    }

    public class PyqBankSubjectsDto
    {
        public Guid? ExamId { get; set; }
        public List<ExamOptionDto> Exams { get; set; } = new();          // ExamOptionDto: defined in PyqYearFeatures.cs
        public List<PyqBankSubjectDto> Subjects { get; set; } = new();
    }

    public record GetPyqBankSubjectsQuery(Guid? ExamId = null) : IRequest<PyqBankSubjectsDto>;

    public class GetPyqBankSubjectsQueryValidator : AbstractValidator<GetPyqBankSubjectsQuery>
    {
        public GetPyqBankSubjectsQueryValidator() => RuleFor(x => x.ExamId).Must(g => g == null || g != Guid.Empty);
    }

    public class GetPyqBankSubjectsQueryHandler : IRequestHandler<GetPyqBankSubjectsQuery, PyqBankSubjectsDto>
    {
        private readonly IRepository<Subject> _subjects;
        private readonly IRepository<Paper> _papers;
        private readonly IRepository<Exam> _exams;

        public GetPyqBankSubjectsQueryHandler(IRepository<Subject> subjects, IRepository<Paper> papers, IRepository<Exam> exams)
        { _subjects = subjects; _papers = papers; _exams = exams; }

        public async Task<PyqBankSubjectsDto> Handle(GetPyqBankSubjectsQuery req, CancellationToken ct)
        {
            var exams = await _exams.Entity().AsNoTracking().Where(e => e.IsActive)
                .OrderBy(e => e.DisplayOrder).ThenBy(e => e.ExamName)
                .Select(e => new ExamOptionDto { ExamId = e.ExamId, ExamName = e.ExamName }).ToListAsync(ct);
             
            var q = from s in _subjects.Entity().AsNoTracking()
                    join p in _papers.Entity().AsNoTracking() on s.PaperId equals p.PaperId
                    join e in _exams.Entity().AsNoTracking() on p.ExamId equals e.ExamId
                    where p.IsActive && e.IsActive
                    select new { s, p, e };

            if (req.ExamId.HasValue) q = q.Where(x => x.e.ExamId == req.ExamId.Value);

            var subjects = await q.OrderBy(x => x.e.DisplayOrder).ThenBy(x => x.p.DisplayOrder).ThenBy(x => x.s.DisplayOrder)
                .Select(x => new PyqBankSubjectDto
                {
                    SubjectId = x.s.SubjectId,
                    SubjectName = x.s.SubjectName,
                    ExamId = x.e.ExamId,
                    ExamName = x.e.ExamName,
                    PaperName = x.p.PaperName
                }).ToListAsync(ct);

            return new PyqBankSubjectsDto { ExamId = req.ExamId, Exams = exams, Subjects = subjects };
        }
    }
}
