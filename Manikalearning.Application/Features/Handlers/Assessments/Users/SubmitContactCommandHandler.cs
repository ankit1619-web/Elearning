using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Users
{
    public record SubmitContactCommand(string? Name, string? Email, string? Subject, string? Message, string? IpAddress)
        : IRequest<ResponseDto<Guid>>;
     
    public class SubmitContactCommandValidator : AbstractValidator<SubmitContactCommand>
    {
        public SubmitContactCommandValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please enter your name.")
                .Must(v => v!.Trim().Length >= 2).WithMessage("Your name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Your name must be 100 characters or fewer.");

            RuleFor(x => x.Email).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please enter your email address.")
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .MaximumLength(150).WithMessage("Your email must be 150 characters or fewer.");

            RuleFor(x => x.Subject).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please enter a subject.")
                .Must(v => v!.Trim().Length >= 3).WithMessage("The subject must be at least 3 characters.")
                .MaximumLength(200).WithMessage("The subject must be 200 characters or fewer.");

            RuleFor(x => x.Message).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please enter your message.")
                .Must(v => v!.Trim().Length >= 10).WithMessage("Your message must be at least 10 characters.")
                .MaximumLength(2000).WithMessage("Your message must be 2000 characters or fewer.");
        }
    }
    public class SubmitContactCommandHandler : IRequestHandler<SubmitContactCommand, ResponseDto<Guid>>
    {
        private const int MaxPerHourPerIp = 5;          
        private readonly IRepository<ContactMessage> _repo;
        private readonly IEmailService _emailService;
        public SubmitContactCommandHandler(IRepository<ContactMessage> repo, IEmailService emailService) { _repo = repo; _emailService = emailService; }

        public async Task<ResponseDto<Guid>> Handle(SubmitContactCommand r, CancellationToken ct)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(r.IpAddress))
                {
                    var since = DateTime.UtcNow.AddHours(-1);
                    var recent = await _repo.Entity().AsNoTracking()
                        .CountAsync(m => m.IpAddress == r.IpAddress && m.CreatedAt >= since, ct);
                    if (recent >= MaxPerHourPerIp)
                        return ResponseDto<Guid>.Failure("You have sent several messages recently. Please try again later.");
                }

                var msg = new ContactMessage
                {
                    ContactMessageId = Guid.NewGuid(),
                    Name = r.Name!.Trim(),
                    Email = r.Email!.Trim().ToLowerInvariant(),
                    Subject = r.Subject!.Trim(),
                    Message = r.Message!.Trim(),
                    IpAddress = r.IpAddress,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(msg);
                await _repo.SaveChangesAsync();
                await _emailService.SendAsync(msg, ct);
                return ResponseDto<Guid>.Success(msg.ContactMessageId, "Thank you! Your message has been sent. We will get back to you soon.");
            }
            catch (Exception)
            {
                return ResponseDto<Guid>.Failure("We could not send your message right now. Please try again.");
            }
        }
    }
}
