using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{
    public record GetPlansQuery(string SearchTerm = null) : IRequest<ResponseDto<List<PlanDto>>>; 
}
