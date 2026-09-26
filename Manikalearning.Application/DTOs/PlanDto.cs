using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class PlanDto
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int TotalCoursesMapped { get; set; }
    }
}
