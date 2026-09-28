using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentCourses
{
    public record EnrollStudentCommand(
        Guid UserId,
        Guid CourseId,
        DateTime? StartDate,
        DateTime? ExpiryDate,
        bool IsActive = true
    ) : IRequest<ResponseDto<Guid>>; 
}
