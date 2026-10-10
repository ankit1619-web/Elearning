using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetSectionsBySubjectQuery(Guid SubjectId) : IRequest<ResponseDto<List<SectionDto>>>;
}
