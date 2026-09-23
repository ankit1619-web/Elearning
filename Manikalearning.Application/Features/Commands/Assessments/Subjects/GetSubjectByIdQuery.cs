using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subjects
{
    public record GetSubjectByIdQuery(Guid SubjectId) : IRequest<ResponseDto<SubjectDto>>;
}
