using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Exams
{
    public record GetExamByIdQuery(Guid ExamId) : IRequest<ResponseDto<ExamDto>>;
    public class GetExamByIdQueryValidator : AbstractValidator<GetExamByIdQuery>
    {
        public GetExamByIdQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam id is required.");
             
        }
    }
}
