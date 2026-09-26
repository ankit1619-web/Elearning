using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{
    public record GetPlansQuery(string SearchTerm = null) : IRequest<ResponseDto<List<PlanDto>>>; 
}
