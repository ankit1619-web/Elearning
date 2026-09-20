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
    public record UpdateCourseSectionCommand(
        Guid SectionId,
        string Title,
        string? Description,
        int DisplayOrder,
        bool IsActive
    ) : IRequest<ResponseDto<Guid>>;

    public class UpdateCourseSectionCommandValidator : AbstractValidator<UpdateCourseSectionCommand>
    {
        public UpdateCourseSectionCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required for update.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Section Title is required.")
                .MinimumLength(3).WithMessage("Section Title must be at least 3 characters long.")
                .MaximumLength(150).WithMessage("Section Title cannot exceed 150 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
        }
    }
}
