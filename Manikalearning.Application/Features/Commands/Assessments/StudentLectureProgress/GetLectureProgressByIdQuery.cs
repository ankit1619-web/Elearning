using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress
{
    public record GetLectureProgressByIdQuery(Guid ProgressId) : IRequest<ResponseDto<StudentLectureProgressDto>>;
}
