using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress
{
    public record ResetLectureProgressCommand(Guid ProgressId) : IRequest<ResponseDto<bool>>;
}
