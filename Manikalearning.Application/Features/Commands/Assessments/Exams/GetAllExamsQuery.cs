using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Exams
{
    public record GetAllExamsQuery(string SearchTerm = null) : IRequest<ResponseDto<List<ExamDto>>>;
    public class GetExamsQueryValidator : AbstractValidator<GetAllExamsQuery>
    {
        public GetExamsQueryValidator() => RuleFor(x => x.SearchTerm).MaximumLength(100);
    }
}
