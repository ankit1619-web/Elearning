using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public class GetTestByIdQuery : IRequest<ResponseDto<TestDto>>
    {
        public Guid Id { get; set; }

        public GetTestByIdQuery(Guid id)
        {
            Id = id;
        }
    } 
    public class GetTestByIdQueryValidator : AbstractValidator<GetTestByIdQuery>
    {
        public GetTestByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Test ID is required.");
        }
    }
}
