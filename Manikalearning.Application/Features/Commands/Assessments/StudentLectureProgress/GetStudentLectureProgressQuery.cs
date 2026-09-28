using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress
{
    public record GetStudentLectureProgressQuery(
         string? Search = null,
         Guid? CourseId = null,
         bool? IsCompleted = null,
         int PageIndex = 1,
         int PageSize = 10
     ) : IRequest<ResponseDto<ProgressPagedResultDto>>;
}
