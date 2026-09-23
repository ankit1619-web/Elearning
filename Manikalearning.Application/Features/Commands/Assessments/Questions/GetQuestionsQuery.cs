using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Questions
{
    public record GetQuestionsQuery(Guid? TopicId = null, Guid? SubTopicId = null, string SearchTerm = "")
         : IRequest<ResponseDto<List<QuestionDto>>>;
}
