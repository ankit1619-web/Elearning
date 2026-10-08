using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.CourseLecture
{
    public record CreateCourseLectureCommand(
         Guid CourseSectionId,
         string Title,
         string? VideoUrl,
         string? PdfUrl,
         string? Notes,
         int DurationInSeconds,
         int DisplayOrder,
         bool IsPreview
     ) : IRequest<ResponseDto<Guid>>;
     
    public class CreateCourseLectureCommandValidator : AbstractValidator<CreateCourseLectureCommand>
    {
        public CreateCourseLectureCommandValidator()
        {
            RuleFor(x => x.CourseSectionId)
                .NotEmpty().WithMessage("Course Section ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Lecture title is required.")
                .Length(3, 200).WithMessage("Lecture title must be between 3 and 200 characters.");

            RuleFor(x => x.VideoUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.VideoUrl))
                .WithMessage("Please enter a valid Video URL.");

            RuleFor(x => x.PdfUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.PdfUrl))
                .WithMessage("Please enter a valid PDF/Resource URL.");

            RuleFor(x => x.DurationInSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("Duration cannot be negative.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
