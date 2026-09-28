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
    public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<StudentCourse> _studentCourseRepo;

        public EnrollStudentCommandHandler(IRepository<StudentCourse> studentCourseRepo)
        {
            _studentCourseRepo = studentCourseRepo;
        }

        public async Task<ResponseDto<Guid>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _studentCourseRepo.Entity()
                    .AnyAsync(sc => sc.UserId == request.UserId && sc.CourseId == request.CourseId, cancellationToken);

                if (exists)
                    return ResponseDto<Guid>.Failure("This student is already enrolled in the selected course.");

                var enrollment = new StudentCourse
                {
                    StudentCourseId = Guid.NewGuid(),
                    UserId = request.UserId,
                    CourseId = request.CourseId,
                    EnrolledAt = DateTime.UtcNow,
                    StartDate = request.StartDate ?? DateTime.UtcNow,
                    ExpiryDate = request.ExpiryDate,
                    IsActive = request.IsActive
                };

                await _studentCourseRepo.AddAsync(enrollment);
                await _studentCourseRepo.SaveChangesAsync();

                return ResponseDto<Guid>.Success(enrollment.StudentCourseId, "Student enrolled successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<Guid>.Failure($"Failed to enroll student: {ex.Message}");
            }
        }
    }
}
