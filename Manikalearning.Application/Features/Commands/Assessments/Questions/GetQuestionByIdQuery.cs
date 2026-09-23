using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Questions
{
    public record GetQuestionByIdQuery(Guid QuestionId) : IRequest<ResponseDto<QuestionDto>>;
}
