using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public record GetAvailableQuestionsQuery(Guid? SubjectId) : IRequest<List<TestQuestionDto>>; 
}
