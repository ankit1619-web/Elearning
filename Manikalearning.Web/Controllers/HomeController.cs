using Manikalearning.Application.Features.Commands.Home;
using Manikalearning.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers;

public class HomeController : BaseController
{
    public HomeController(IMediator mediator)
        : base(mediator)
    {
    }
    public async Task<IActionResult> Index()
    {
        var dto = await _mediator.Send(new GetHomePageQuery());

        var model = new HomeVM
        {
            SiteTitle = dto.SiteTitle,

            HeroTitle = dto.HeroTitle,
            HeroDescription = dto.HeroDescription,
            HeroImageUrl = dto.HeroImageUrl,
            HeroButtonText = dto.HeroButtonText,
            HeroButtonUrl = dto.HeroButtonUrl,

            TotalStudents = dto.TotalStudents,
            TotalQuestions = dto.TotalQuestions,
            TotalPYQs = dto.TotalPYQs,
            TotalTests = dto.TotalTests,

            PopularExams = dto.PopularExams.Select(x => new HomeExamVM
            {
                ExamId = x.ExamId,
                ExamName = x.ExamName,
                ImageUrl = x.ImageUrl,
                TotalQuestions = x.TotalQuestions,
                TotalPYQs = x.TotalPYQs,
                TotalTests = x.TotalTests
            }).ToList(),

            FeaturedCourses = dto.FeaturedCourses.Select(x => new HomeCourseVM
            {
                CourseId = x.CourseId,
                CourseName = x.CourseName,
                ThumbnailUrl = x.ThumbnailUrl,
                ShortDescription = x.ShortDescription,
                Price = x.Price,
                DiscountPrice = x.DiscountPrice,
                TotalLectures = x.TotalLectures,
                TotalHours = x.TotalHours
            }).ToList(),

            PopularPYQs = dto.PopularPYQs.Select(x => new HomePYQVM
            {
                QuestionId = x.QuestionId,
                QuestionText = x.QuestionText,
                ExamName = x.ExamName,
                SubjectName = x.SubjectName,
                Year = x.Year
            }).ToList(),

            PopularTests = dto.PopularTests.Select(x => new HomeTestVM
            {
                TestId = x.TestId,
                TestName = x.TestName,
                ExamName = x.ExamName,
                TotalQuestions = x.TotalQuestions,
                DurationMinutes = x.DurationMinutes,
                TotalMarks = x.TotalMarks
            }).ToList(),

            Features = dto.Features.Select(x => new FeatureVM
            {
                Title = x.Title,
                Description = x.Description,
                Icon = x.Icon,
                ImageUrl = x.ImageUrl,
                DisplayOrder = x.DisplayOrder
            }).ToList(),

            Testimonials = dto.Testimonials.Select(x => new TestimonialVM
            {
                TestimonialId = x.TestimonialId,
                StudentName = x.StudentName,
                ProfileImage = x.ProfileImage,
                ExamName = x.ExamName,
                Review = x.Review,
                Rating = x.Rating,
                DisplayOrder = x.DisplayOrder
            }).ToList(),

            FAQs = dto.FAQs.Select(x => new FAQVM
            {
                FAQId = x.FAQId,
                Question = x.Question,
                Answer = x.Answer,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToList()
        };

        return View(model);
    }
}
