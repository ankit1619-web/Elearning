using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Courses
{
    public record GetCoursesQuery(string SearchTerm = null) : IRequest<ResponseDto<List<CourseDto>>>; 
}
