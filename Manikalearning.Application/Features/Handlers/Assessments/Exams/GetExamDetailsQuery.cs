using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Exams
{
    public record GetExamDetailsQuery(Guid ExamId) : IRequest<ExamListDetailsDto?>;

    public class GetExamDetailsQueryValidator : AbstractValidator<GetExamDetailsQuery>
    {
        public GetExamDetailsQueryValidator() => RuleFor(x => x.ExamId).NotEmpty();
    }

    public class GetExamDetailsQueryHandler : IRequestHandler<GetExamDetailsQuery, ExamListDetailsDto?>
    {
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<ExamCourse> _examCourses;
        private readonly IRepository<Course> _courses;

        public GetExamDetailsQueryHandler(IRepository<Exam> exams, IRepository<ExamCourse> examCourses, IRepository<Course> courses)
        { _exams = exams; _examCourses = examCourses; _courses = courses; }

        public async Task<ExamListDetailsDto?> Handle(GetExamDetailsQuery req, CancellationToken ct)
        {
            var exam = await _exams.Entity().AsNoTracking()
                .FirstOrDefaultAsync(e => e.ExamId == req.ExamId && e.IsActive, ct);
            if (exam == null) return null;

            var courses = await (from ec in _examCourses.Entity().AsNoTracking()
                                 join c in _courses.Entity().AsNoTracking() on ec.CourseId equals c.CourseId
                                 where ec.ExamId == req.ExamId && c.IsPublished
                                 orderby c.CourseName
                                 select new { c.CourseId, c.CourseName, c.Description, c.ThumbnailUrl, c.Price, c.DiscountPrice, c.IsFree })
                                .ToListAsync(ct);

            return new ExamListDetailsDto
            {
                Exam = new ExamDto
                {
                    ExamId = exam.ExamId,
                    ExamName = exam.ExamName,
                    ExamCode = exam.ExamCode,
                    Description = exam.Description,
                    ImageUrl = exam.ImageUrl,
                    CourseCount = courses.Count
                },
                Courses = courses.Select(c => new CourseCardDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Price = c.Price,
                    IsFree = c.IsFree,
                    // DiscountPrice 0 (or higher than price) means "no discount"
                    EffectivePrice =  c.DiscountPrice.HasValue && c.DiscountPrice.Value > 0 && c.DiscountPrice.Value < c.Price ? c.DiscountPrice.Value : c.Price
                }).ToList()
            };
        }
    }
}
