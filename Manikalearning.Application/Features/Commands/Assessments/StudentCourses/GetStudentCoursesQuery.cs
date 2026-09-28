using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentCourses
{
    public record GetStudentCoursesQuery(
        string? Search = null,
        Guid? CourseId = null,
        bool? IsActive = null,
        int PageIndex = 1,
        int PageSize = 10
    ) : IRequest<ResponseDto<StudentCoursePagedResultDto>>;
}
