using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetPYQQuestionsQuery(Guid SectionId, Guid? UserId) : IRequest<ResponseDto<TopicPyqListDto>>;
}
