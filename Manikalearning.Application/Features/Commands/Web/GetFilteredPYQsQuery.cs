using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Web
{
    public record GetFilteredPYQsQuery(Guid? PaperId, Guid? SectionId, Guid? TopicId) : IRequest<PYQTestSessionVM>;

    public class PYQTestSessionVM
    {
        public string Title { get; set; }  
        public List<QuestionDto> Questions { get; set; }
    }
}
