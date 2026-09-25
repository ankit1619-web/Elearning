using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public class GetMappedTestQuestionsQuery : IRequest<ResponseDto<List<TestQuestionDto>>>
    {
        public Guid TestId { get; set; }

        public GetMappedTestQuestionsQuery(Guid testId)
        {
            TestId = testId;
        }
    }
     
    public class GetMappedTestQuestionsQueryValidator : AbstractValidator<GetMappedTestQuestionsQuery>
    {
        public GetMappedTestQuestionsQueryValidator()
        {
            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");
        }
    }
}
