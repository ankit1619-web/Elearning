using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Dropdowns
{
    public record GetCourseOptionsQuery() : IRequest<List<CourseOptionDto>>;
    public class GetCourseOptionsQueryHandler : IRequestHandler<GetCourseOptionsQuery, List<CourseOptionDto>>
    {
        private readonly IRepository<Course> _courses;
        public GetCourseOptionsQueryHandler(IRepository<Course> courses) => _courses = courses;

        public Task<List<CourseOptionDto>> Handle(GetCourseOptionsQuery req, CancellationToken ct) =>
            _courses.Entity().AsNoTracking().Where(c => c.IsPublished).OrderBy(c => c.CourseName)
                .Select(c => new CourseOptionDto { CourseId = c.CourseId, CourseName = c.CourseName }).ToListAsync(ct);
    }

}
