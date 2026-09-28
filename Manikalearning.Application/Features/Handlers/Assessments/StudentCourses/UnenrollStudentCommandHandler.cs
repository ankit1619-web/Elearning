using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentCourses
{
    public class UnenrollStudentCommandHandler : IRequestHandler<UnenrollStudentCommand, ResponseDto<bool>>
    {
        private readonly IRepository<StudentCourse> _studentCourseRepo;

        public UnenrollStudentCommandHandler(IRepository<StudentCourse> studentCourseRepo)
        {
            _studentCourseRepo = studentCourseRepo;
        }

        public async Task<ResponseDto<bool>> Handle(UnenrollStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sc = await _studentCourseRepo.Entity()
                    .FirstOrDefaultAsync(x => x.StudentCourseId == request.StudentCourseId, cancellationToken);

                if (sc == null)
                    return ResponseDto<bool>.Failure("Enrollment record not found.");

                  _studentCourseRepo.Remove(sc);
                await _studentCourseRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Student course enrollment deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to delete enrollment: {ex.Message}");
            }
        }
    }
}
