using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Coursesection
{
    public record DeleteCourseSectionCommand(Guid SectionId) : IRequest<ResponseDto<bool>>;

    public class DeleteCourseSectionCommandValidator : AbstractValidator<DeleteCourseSectionCommand>
    {
        public DeleteCourseSectionCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Valid Section ID is required to perform deletion.");
        }
    }
}
