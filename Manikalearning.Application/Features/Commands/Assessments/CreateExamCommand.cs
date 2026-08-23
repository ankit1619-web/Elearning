using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments
{
    public record CreateExamCommand(string ExamName, string Slug, string Description) : IRequest<Guid>;
}
