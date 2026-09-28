using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentCourses
{
    public record UnenrollStudentCommand(Guid StudentCourseId) : IRequest<ResponseDto<bool>>;
}
