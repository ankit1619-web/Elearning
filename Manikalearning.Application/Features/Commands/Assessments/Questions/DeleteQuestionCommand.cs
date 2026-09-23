using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Questions
{
    public record DeleteQuestionCommand(Guid QuestionId) : IRequest<ResponseDto<bool>>;
}
