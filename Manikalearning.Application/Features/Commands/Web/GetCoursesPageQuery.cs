using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Web
{ 
    public record GetCoursesPageQuery(Guid? ExamId) : IRequest<List<CourseDto>>;
}
