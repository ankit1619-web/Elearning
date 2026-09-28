using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentCourses
{
    public class ToggleEnrollmentStatusCommandHandler : IRequestHandler<ToggleEnrollmentStatusCommand, ResponseDto<bool>>
    {
        private readonly IRepository<StudentCourse> _studentCourseRepo;

        public ToggleEnrollmentStatusCommandHandler(IRepository<StudentCourse> studentCourseRepo)
        {
            _studentCourseRepo = studentCourseRepo;
        }

        public async Task<ResponseDto<bool>> Handle(ToggleEnrollmentStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sc = await _studentCourseRepo.Entity()
                    .FirstOrDefaultAsync(x => x.StudentCourseId == request.StudentCourseId, cancellationToken);

                if (sc == null)
                    return ResponseDto<bool>.Failure("Enrollment record not found.");

                sc.IsActive = !sc.IsActive;
                  _studentCourseRepo.Update(sc);
                await _studentCourseRepo.SaveChangesAsync();

                string statusText = sc.IsActive ? "Activated" : "Deactivated";
                return ResponseDto<bool>.Success(true, $"Enrollment status changed to {statusText}.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to toggle status: {ex.Message}");
            }
        }
    }
}
