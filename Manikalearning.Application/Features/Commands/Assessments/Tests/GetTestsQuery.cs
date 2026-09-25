using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public class GetTestsQuery : IRequest<ResponseDto<List<TestDto>>>
    {
        public Guid? SubjectId { get; set; }
        public string? Search { get; set; }

        public GetTestsQuery(Guid? subjectId = null, string? search = null)
        {
            SubjectId = subjectId;
            Search = search;
        }
    }
     
    public class GetTestsQueryValidator : AbstractValidator<GetTestsQuery>
    {
        public GetTestsQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters.");
        }
    }
}
