using FluentValidation;
using Manikalearning.Application.Comman;
using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Users
{
    public record UpdateProfileCommand(Guid UserId, string FullName, string? Mobile, Guid? TargetExamId, DateTime? TargetExamDate, int DailyGoal) : IRequest<bool>;

    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();

            RuleFor(x => x.FullName).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please enter your name.")
                .Must(n => n.Trim().Length is >= 2 and <= 150).WithMessage("Your name must be 2 to 150 letters.")
                .Matches(@"^[\p{L}\p{M}][\p{L}\p{M} .'\-]*$").WithMessage("Your name can have only letters, spaces, . ' and -");

            RuleFor(x => x.Mobile)
                .Must(m => string.IsNullOrWhiteSpace(m) || MobileNumber.IsValid(MobileNumber.Normalize(m)))
                .WithMessage("Enter a valid 10 digit mobile number.");

            RuleFor(x => x.DailyGoal).InclusiveBetween(5, 200).WithMessage("Daily goal must be between 5 and 200 questions.");

            RuleFor(x => x.TargetExamDate)
                .Must(d => d == null || (d.Value.Date >= AppClock.TodayIst && d.Value.Date <= AppClock.TodayIst.AddYears(3)))
                .WithMessage("Choose an exam date from today up to 3 years ahead.");
        }
    }
}
