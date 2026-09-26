using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 

namespace Manikalearning.Application.Features.Handlers.Assessments.Plans
{
    public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, ResponseDto<List<PlanDto>>>
    {
        private readonly IRepository<Plan> _planRepo;

        public GetPlansQueryHandler(IRepository<Plan> planRepo)
        {
            _planRepo = planRepo;
        }

        public async Task<ResponseDto<List<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _planRepo.Entity()
                    .Include(p => p.PlanCourses)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.Trim().ToLower();
                    query = query.Where(p => p.PlanName.ToLower().Contains(term));
                }

                var list = await query.Select(p => new PlanDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    Price = p.Price,
                    DurationDays = p.DurationDays,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    TotalCoursesMapped = p.PlanCourses.Count
                }).ToListAsync(cancellationToken);

                return ResponseDto<List<PlanDto>>.Success(list, "Plans retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<PlanDto>>.Failure($"Error fetching plans: {ex.Message}");
            }
        }
    }
}
