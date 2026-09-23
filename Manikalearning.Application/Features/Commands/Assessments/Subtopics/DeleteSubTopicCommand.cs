using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subtopics
{
    public record DeleteSubTopicCommand(Guid Id) : IRequest<ResponseDto<bool>>;
}
