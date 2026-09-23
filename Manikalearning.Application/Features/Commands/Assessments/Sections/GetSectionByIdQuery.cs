using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Sections
{
    public record GetSectionByIdQuery(Guid SectionId) : IRequest<ResponseDto<SectionDto>>;
}
