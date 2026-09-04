using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class ExamsController : BaseController
    {
        public ExamsController(IMediator mediator)
        : base(mediator)
        {
        }
        public async Task<IActionResult> Index()
        {
            var dto = await _mediator.Send(new GetExamPageQuery());

            var model = dto.Select(x => new ExamViewModel
            {
                Id = x.Id,

                Name = x.Name,

                ShortDescription = x.ShortDescription,
                    
                TotalQuestions = x.TotalQuestions, 

                TotalPYQs = x.TotalPYQs,

                TotalTests = x.TotalTests,

                TotalStudents = x.TotalStudents

            }).ToList();

            return View(model);
        }
        public async Task<IActionResult> Details(
             Guid id,
             CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(
                new GetExamDetailsQuery(id),
                cancellationToken);

            if (dto == null)
                return NotFound();


            var model = new ExamVM
            {
                ExamId = dto.ExamId,

                ExamName = dto.Name, 

                Description = dto.Description,

                ImageUrl = dto.Image, 

                TotalQuestions = dto.TotalQuestions,

                TotalPYQs = dto.TotalPYQs,

                TotalTests = dto.TotalTests 
            };


            return View(model);
        }
    }
}
