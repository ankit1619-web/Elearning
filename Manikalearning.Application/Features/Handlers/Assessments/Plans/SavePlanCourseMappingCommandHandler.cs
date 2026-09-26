using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Plans
{
    public class SavePlanCourseMappingCommandHandler : IRequestHandler<SavePlanCourseMappingCommand, ResponseDto<bool>>
    {
        private readonly IRepository<PlanCourse> _planCourseRepo;

        public SavePlanCourseMappingCommandHandler(IRepository<PlanCourse> planCourseRepo)
        {
            _planCourseRepo = planCourseRepo;
        }

        public async Task<ResponseDto<bool>> Handle(SavePlanCourseMappingCommand request, CancellationToken cancellationToken)
        {
            try
            { 
                var existingMappings = await _planCourseRepo.Entity()
                    .Where(pc => pc.PlanId == request.PlanId)
                    .ToListAsync(cancellationToken);

                foreach (var mapping in existingMappings)
                {
                      _planCourseRepo.Remove(mapping);
                }

 
                if (request.SelectedCourseIds != null && request.SelectedCourseIds.Any())
                {
                    foreach (var courseId in request.SelectedCourseIds.Distinct())
                    {
                        await _planCourseRepo.AddAsync(new PlanCourse
                        {
                            PlanId = request.PlanId,
                            CourseId = courseId
                        });
                    }
                }

                await _planCourseRepo.SaveChangesAsync();
                return ResponseDto<bool>.Success(true, "Plan courses updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to update mapped courses: {ex.Message}");
            }
        }
    }
}
