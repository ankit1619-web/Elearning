using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Plans
{
    public record SavePlanCourseMappingCommand(Guid PlanId, List<Guid> SelectedCourseIds) : IRequest<ResponseDto<bool>>;
}
