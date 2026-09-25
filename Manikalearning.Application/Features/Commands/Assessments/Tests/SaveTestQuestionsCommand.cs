using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public class SaveTestQuestionsCommand : IRequest<ResponseDto<bool>>
    {
        public Guid TestId { get; set; }
        public List<TestQuestionItemDto> Questions { get; set; } = new();
    }

    // 2. Validator
    public class SaveTestQuestionsCommandValidator : AbstractValidator<SaveTestQuestionsCommand>
    {
        public SaveTestQuestionsCommandValidator()
        {
            RuleFor(x => x.Questions)
                .NotEmpty().WithMessage("At least one question is required.");
             
            RuleForEach(x => x.Questions)
                .SetValidator(new TestQuestionItemDtoValidator());
        }
    }
    public class TestQuestionItemDtoValidator : AbstractValidator<TestQuestionItemDto>
    {
        public TestQuestionItemDtoValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("Question Id is required.");

            RuleFor(x => x.Marks)
                .GreaterThan(0).WithMessage("Marks must be greater than 0.");
        }
    }
}
