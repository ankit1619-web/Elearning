using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetMyDashboardQueryHandler : IRequestHandler<GetMyDashboardQuery, MyDashboardDto>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<TestAttempt> _attemptRepo;
        private readonly IRepository<Payment> _transactionRepo;
        private readonly UserManager<User> _userManager;

        public GetMyDashboardQueryHandler(
            IRepository<User> userRepo,
            IRepository<Course> courseRepo,
            IRepository<TestAttempt> attemptRepo,
            IRepository<Payment> transactionRepo,
            UserManager<User> userManager)
        {
            _userRepo = userRepo;
            _courseRepo = courseRepo;
            _attemptRepo = attemptRepo;
            _transactionRepo = transactionRepo;
            _userManager = userManager;
        }

        public async Task<MyDashboardDto> Handle(GetMyDashboardQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                       ?? await _userRepo.Entity().FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
                return new MyDashboardDto();
             
            var attempts = await _attemptRepo.Entity()
                .AsNoTracking()
                .Where(a => a.UserId == request.UserId)  
                .ToListAsync(cancellationToken);       

            int totalQuestionsSolved = attempts.Sum(a => a.AttemptedQuestions);
            int totalCorrect = attempts.Sum(a => a.CorrectAnswers);
            double accuracy = totalQuestionsSolved > 0
                ? Math.Round((double)totalCorrect / totalQuestionsSolved * 100, 1)
                : 0;
             
            var courses = await _courseRepo.Entity()
                .AsNoTracking()
                .ToListAsync(cancellationToken);     

            var enrolledCourseDtos = courses.Select(c => new CourseProgressDto
            {
                CourseId = c.CourseId,
                Title = c.CourseName,
                //CurrentModule = c.CurrentModuleName ?? "Core Module",
                //ProgressPercentage = c.ProgressPercentage,
                //ThumbnailUrl = string.IsNullOrEmpty(c.ThumbnailUrl) ? "/images/course-placeholder.jpg" : c.ThumbnailUrl,
                //EstimatedMinutesLeft = c.EstimatedMinutesLeft
            }).ToList();
             
            var completedPayments = await _transactionRepo.Entity()
                .AsNoTracking()
                .Where(p => p.UserId == request.UserId && p.Status == "Success")
                .ToListAsync(cancellationToken);

            var subjectStrengths = attempts
     .Where(a => a.Test != null && !string.IsNullOrEmpty(a.Test.TestName))
     .GroupBy(a => a.Test.TestName)
     .Select(g => new SubjectStrengthDto
     {
         SubjectName = g.Key,
         AccuracyPercentage = (double)Math.Round(g.Average(x => x.Accuracy), 1)
     })
     .ToList();

            return new MyDashboardDto
            {
                StudentName = user.FullName ?? user.UserName ?? "Student",
                TargetExam = "UGC NET Paper 1",
                ReadinessScore = (int)accuracy,
                TotalQuestionsSolved = totalQuestionsSolved,
                AccuracyPercentage = accuracy,  
                //ActiveMockTestsCount = attempts.Count(a => a.),
                //MistakeBookCount = attempts.Sum(a => a.IncorrectAnswersCount),
                //TotalPyqSolvedCount = attempts.Where(a => a.IsPyq).Sum(a => a.TotalQuestionsAttempted),
                //BookmarksCount = attempts.Sum(a => a.BookmarkedQuestionsCount),

                EnrolledCourses = enrolledCourseDtos,
                SubjectStrengths = subjectStrengths
            };
        }
    }
}
