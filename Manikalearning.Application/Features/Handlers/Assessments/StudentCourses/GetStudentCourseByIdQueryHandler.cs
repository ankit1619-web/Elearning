using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentCourses
{
    public class GetStudentCourseByIdQueryHandler : IRequestHandler<GetStudentCourseByIdQuery, ResponseDto<StudentCourseDto>>
    {
        private readonly IRepository<StudentCourse> _studentCourseRepo;

        public GetStudentCourseByIdQueryHandler(IRepository<StudentCourse> studentCourseRepo)
        {
            _studentCourseRepo = studentCourseRepo;
        }

        public async Task<ResponseDto<StudentCourseDto>> Handle(GetStudentCourseByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sc = await _studentCourseRepo.Entity()
                    .Include(x => x.User)
                    .Include(x => x.Course)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.StudentCourseId == request.StudentCourseId, cancellationToken);

                if (sc == null)
                    return ResponseDto<StudentCourseDto>.Failure("Student course enrollment record not found.");

                var dto = new StudentCourseDto
                {
                    StudentCourseId = sc.StudentCourseId,
                    UserId = sc.UserId,
                    UserName = sc.User.FullName ?? "N/A",
                    UserEmail = sc.User.Email ?? "N/A",
                    CourseId = sc.CourseId,
                    CourseTitle = sc.Course.CourseName ?? "N/A", 
                    EnrolledAt = sc.EnrolledAt,
                    StartDate = sc.StartDate,
                    ExpiryDate = sc.ExpiryDate,
                    IsActive = sc.IsActive
                };

                return ResponseDto<StudentCourseDto>.Success(dto, "Enrollment details retrieved.");
            }
            catch (Exception ex)
            {
                return ResponseDto<StudentCourseDto>.Failure($"Failed to fetch enrollment details: {ex.Message}");
            }
        }
    }
}
