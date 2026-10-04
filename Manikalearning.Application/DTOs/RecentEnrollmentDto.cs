using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class RecentEnrollmentDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    public class AnalyticsChartPointDto
    {
        public string Label { get; set; } = string.Empty;  
        public int HeightPercentage { get; set; } 
        public decimal Value { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public double StudentsGrowthPercentage { get; set; }

        public decimal MonthlyRevenue { get; set; }
        public double RevenueGrowthPercentage { get; set; }

        public int ActiveCourses { get; set; }

        public int WeeklyTestAttempts { get; set; }
        public double TestAttemptsGrowthPercentage { get; set; }

        public List<AnalyticsChartPointDto> WeeklyAnalytics { get; set; } = new();
        public List<RecentEnrollmentDto> RecentEnrollments { get; set; } = new();
    }
}
