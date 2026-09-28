using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentCourses
{
    public record GetStudentCourseByIdQuery(Guid StudentCourseId) : IRequest<ResponseDto<StudentCourseDto>>;
}
