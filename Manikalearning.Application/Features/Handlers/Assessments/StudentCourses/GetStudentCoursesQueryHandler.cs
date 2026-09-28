using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentCourses
{
    public class GetStudentCoursesQueryHandler : IRequestHandler<GetStudentCoursesQuery, ResponseDto<StudentCoursePagedResultDto>>
    {
        private readonly IRepository<StudentCourse> _studentCourseRepo;

        public GetStudentCoursesQueryHandler(IRepository<StudentCourse> studentCourseRepo)
        {
            _studentCourseRepo = studentCourseRepo;
        }

        public async Task<ResponseDto<StudentCoursePagedResultDto>> Handle(GetStudentCoursesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _studentCourseRepo.Entity()
                    .Include(sc => sc.User)
                    .Include(sc => sc.Course) 
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    query = query.Where(sc =>
                        sc.User.FullName.ToLower().Contains(term) ||
                        sc.User.Email!.ToLower().Contains(term) ||
                        sc.Course.CourseName.ToLower().Contains(term));
                }

                if (request.CourseId.HasValue && request.CourseId.Value != Guid.Empty)
                {
                    query = query.Where(sc => sc.CourseId == request.CourseId.Value);
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(sc => sc.IsActive == request.IsActive.Value);
                }

                var totalCount = await query.CountAsync(cancellationToken);
                var activeCount = await _studentCourseRepo.Entity().CountAsync(sc => sc.IsActive, cancellationToken);
                var inactiveCount = await _studentCourseRepo.Entity().CountAsync(sc => !sc.IsActive, cancellationToken);

                var items = await query
                    .OrderByDescending(sc => sc.EnrolledAt)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(sc => new StudentCourseDto
                    {
                        StudentCourseId = sc.StudentCourseId,
                        UserId = sc.UserId,
                        UserName = sc.User.FullName ?? "N/A",
                        UserEmail = sc.User.Email ?? "N/A",
                        CourseId = sc.CourseId,
                        CourseTitle = sc.Course.CourseName, 
                        EnrolledAt = sc.EnrolledAt,
                        StartDate = sc.StartDate,
                        ExpiryDate = sc.ExpiryDate,
                        IsActive = sc.IsActive
                    }).ToListAsync(cancellationToken);

                var result = new StudentCoursePagedResultDto
                {
                    Enrollments = items,
                    TotalCount = totalCount,
                    ActiveCount = activeCount,
                    InactiveCount = inactiveCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                return ResponseDto<StudentCoursePagedResultDto>.Success(result, "Enrollments loaded successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<StudentCoursePagedResultDto>.Failure($"Failed to retrieve enrollments: {ex.Message}");
            }
        }
    }
}
