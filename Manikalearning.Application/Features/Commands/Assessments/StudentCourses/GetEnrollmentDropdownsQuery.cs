using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentCourses
{
    public record GetEnrollmentDropdownsQuery() : IRequest<ResponseDto<(List<UserSelectOptionDto> Users, List<CourseSelectOptionDto> Courses)>>;
}
