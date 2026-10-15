using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetMyDashboardQuery(Guid UserId) : IRequest<MyDashboardDto>;  
    public class GetMyDashboardQueryValidator : AbstractValidator<GetMyDashboardQuery>
    { public GetMyDashboardQueryValidator() => RuleFor(x => x.UserId).NotEmpty().WithMessage("Invalid User."); }
}
