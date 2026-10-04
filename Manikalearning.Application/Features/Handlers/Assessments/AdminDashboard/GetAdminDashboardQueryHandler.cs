using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.AdminDashboard;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.AdminDashboard
{
    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardViewModel>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Course> _courseRepo; 
        private readonly IRepository<TestAttempt> _attemptRepo; 
        private readonly IRepository<Payment> _transactionRepo; 
        private readonly UserManager<User> _userManager;
        public GetAdminDashboardQueryHandler(
            IRepository<User> userRepo,
            IRepository<Course> courseRepo,
            IRepository<TestAttempt> attemptRepo,
            UserManager<User> userManager,
            IRepository<Payment> transactionRepo)
        {
            _userRepo = userRepo;
            _courseRepo = courseRepo;
            _attemptRepo = attemptRepo;
            _transactionRepo = transactionRepo;
            _userManager = userManager;
        }

        public async Task<AdminDashboardViewModel> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + 1).Date;
            var totalStudents = await _userRepo.Entity()
              .AsNoTracking()
              .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Student"))
              .CountAsync(cancellationToken);

            var activeCourses = await _courseRepo.Entity()
                .AsNoTracking()
                .CountAsync(cancellationToken);
             
            var monthlyRevenue = await _transactionRepo.Entity()
                .AsNoTracking()
                .Where(o => o.Status == "Success" && o.CreatedAt >= startOfMonth)
                .SumAsync(o => (decimal?)o.Amount, cancellationToken) ?? 0m;
             
            var weeklyTestAttempts = await _attemptRepo.Entity()
                .AsNoTracking()
                .CountAsync(a => a.SubmittedAt >= startOfWeek, cancellationToken);

            var recentEnrollments = await _userManager.Users
               .AsNoTracking()
               .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Student"))
               .OrderByDescending(e => e.CreatedAt)
               .Take(5)
               .Select(e => new RecentEnrollmentDto
               {
                   StudentName = !string.IsNullOrEmpty(e.FullName) ? e.FullName : e.UserName!,
                   EnrolledAt = e.CreatedAt
               })
               .ToListAsync(cancellationToken);

            var analytics = new List<AnalyticsChartPointDto>();
            for (int i = 6; i >= 0; i--)
            {
                var dayDate = now.Date.AddDays(-i);
                var dayRevenue = await _transactionRepo.Entity()
                    .AsNoTracking()
                    .Where(o => o.Status == "Success" && o.CreatedAt.Date == dayDate)
                    .SumAsync(o => (decimal?)o.Amount, cancellationToken) ?? 0m;

                int heightPercent = dayRevenue > 0 ? Math.Min((int)(dayRevenue / 5000m * 100), 100) : 15;

                analytics.Add(new AnalyticsChartPointDto
                {
                    Label = dayDate.ToString("ddd"),
                    HeightPercentage = Math.Max(heightPercent, 10),
                    Value = dayRevenue
                });
            }

            return new AdminDashboardViewModel
            {
                TotalStudents = totalStudents,
                StudentsGrowthPercentage = 12.0,
                MonthlyRevenue = monthlyRevenue,
                RevenueGrowthPercentage = 8.5,
                ActiveCourses = activeCourses,
                WeeklyTestAttempts = weeklyTestAttempts,
                TestAttemptsGrowthPercentage = 24.0,
                RecentEnrollments = recentEnrollments,
                WeeklyAnalytics = analytics
            };
        }
    }
}