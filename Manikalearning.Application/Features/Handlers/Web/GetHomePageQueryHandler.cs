using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public class GetHomePageQueryHandler: IRequestHandler<GetHomePageQuery, HomeDto>
    {
        private readonly IRepository<User> _studentRepository;
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Question> _questionRepository; 
        private readonly IRepository<Test> _testRepository; 

        public GetHomePageQueryHandler(
            IRepository<User> studentRepository,
            IRepository<Exam> examRepository,
            IRepository<Course> courseRepository,
            IRepository<Question> questionRepository, 
            IRepository<Test> testRepository)
        {
            _studentRepository = studentRepository;
            _examRepository = examRepository;
            _courseRepository = courseRepository;
            _questionRepository = questionRepository; 
            _testRepository = testRepository; 
        }

        public async Task<HomeDto> Handle(GetHomePageQuery request,CancellationToken cancellationToken)
        {
            var dto = new HomeDto
            {
                SiteTitle = "ManikaLearning",

                HeroTitle =
                    "Prepare Smart. Practice Better. Score Higher.",

                HeroDescription =
                    "Learn from expert courses, practice previous year questions, attempt mock tests and analyse your performance — all in one place.",

                HeroButtonText = "Start Preparing",

                HeroButtonUrl = "/Public/Exams"
            };
              
            dto.TotalStudents = await _studentRepository
                .Query().AsNoTracking()
                .CountAsync(cancellationToken);

            dto.TotalQuestions = await _questionRepository
                .Query().AsNoTracking()
                .CountAsync(cancellationToken);

            //dto.TotalPYQs = await _pyqRepository
            //    .Query()
            //    .CountAsync(cancellationToken);

            dto.TotalTests = await _testRepository
                .Query().AsNoTracking()
                .CountAsync(cancellationToken);
             

            var exams = await _examRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.IsActive)
                //.OrderBy(x => x.DisplayOrder)
                .Take(6)
                .ToListAsync(cancellationToken);

            dto.PopularExams = new List<HomeExamDto>();

            foreach (var exam in exams)
            {
                var totalQuestions = await _questionRepository
                    .Query().AsNoTracking()
                    .CountAsync(
                        x => x.ExamId == exam.ExamId,
                        cancellationToken);

                //var totalPYQs = await _pyqRepository
                //    .Query()
                //    .CountAsync(
                //        x => x.ExamId == exam.Id,
                //        cancellationToken);

                //var totalTests = await _testRepository
                //    .Query()
                //    .CountAsync(
                //        x => x.TestId == exam.Id,
                //        cancellationToken);

                //dto.PopularExams.Add(new HomeExamDto
                //{
                //    ExamId = exam.Id,
                //    ExamName = exam.Name,
                //    ImageUrl = exam.ImageUrl,
                //    TotalQuestions = totalQuestions,
                //    TotalPYQs = totalPYQs,
                //    TotalTests = totalTests
                //});
            }
             

            var courses = await _courseRepository
                .Query().AsNoTracking()
                .AsNoTracking()
                .Where(x => x.IsPublished)
               // .OrderBy(x => x.DisplayOrder)
                .Take(6)
                .ToListAsync(cancellationToken);

            dto.FeaturedCourses = new List<HomeCourseDto>();

            //foreach (var course in courses)
            //{
            //    var totalLectures = await _courseVideoRepository
            //        .Query()
            //        .CountAsync(
            //            x => x.CourseId == course.Id,
            //            cancellationToken);

            //    dto.FeaturedCourses.Add(new HomeCourseDto
            //    {
            //        CourseId = course.Id,
            //        CourseName = course.CourseName,
            //        ThumbnailUrl = course.ThumbnailUrl,
            //        ShortDescription = course.ShortDescription,
            //        Price = course.Price,
            //        DiscountPrice = course.DiscountPrice,
            //        TotalLectures = totalLectures,
            //        TotalHours = course.TotalHours
            //    });
            //}
               
             
            return dto;
        }
    }
}
