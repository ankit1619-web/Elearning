using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class PlanCourseMappingDto
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public List<Guid> SelectedCourseIds { get; set; } = new();
        public List<CourseOptionDto> AvailableCourses { get; set; } = new();
    }
    public class CourseOptionDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsMapped { get; set; }
    }
}
