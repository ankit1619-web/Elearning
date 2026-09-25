using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public class DeleteTestCommand : IRequest<ResponseDto<bool>>
    {
        public Guid Id { get; set; }

        public DeleteTestCommand(Guid id)
        {
            Id = id;
        }
    }
     
    public class DeleteTestCommandValidator : AbstractValidator<DeleteTestCommand>
    {
        public DeleteTestCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Test ID is required for deletion.");
        }
    }
}
