using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Manikalearning.Web.Models;

namespace Manikalearning.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var model = new HomeVM
        {
            SiteTitle = "EduPrep",

            HeroTitle = "Prepare smarter. Score higher.",

            HeroDescription =
                "Master your exam with PYQs, mock tests, practice and expert courses.",

            TotalStudents = 12500,

            TotalQuestions = 50000,

            TotalPYQs = 15000,

            TotalTests = 500
        };
        model.PopularExams = new()
        {
            new HomeExamVM
            {
                ExamId = Guid.NewGuid(),
                ExamName = "UGC NET",
                TotalQuestions = 5000,
                TotalPYQs = 1200
            },
        
            new HomeExamVM
            {
                ExamId = Guid.NewGuid(),
                ExamName = "SSC CGL",
                TotalQuestions = 8000,
                TotalPYQs = 2500
            },
        
            new HomeExamVM
            {
                ExamId = Guid.NewGuid(),
                ExamName = "Banking Exams",
                TotalQuestions = 6500,
                TotalPYQs = 1800
            }
        };
        return View(model);
    } 
}
