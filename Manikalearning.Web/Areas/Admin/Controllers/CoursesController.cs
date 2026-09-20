using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")] 
    public class CoursesController : Controller
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }
         
        [HttpGet]
        public async Task<IActionResult> Index(string search = "")
        {
            var response = await _mediator.Send(new GetCoursesQuery(search));
             
            var courses = response.IsSuccess ? response.Data : new List<CourseDto>();
             
            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["SearchTerm"] = search;
            return View(courses);
        }

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial()
        {
            return PartialView("_CreateUpdate", new CourseViewModel());
        }
         
        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetCourseByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new CourseViewModel
            {
                CourseId = response.Data.Id,
                CourseName = response.Data.CourseName,
                Description = response.Data.Description,
                Price = response.Data.Price,
                Ispublished = response.Data.Ispublished
            };

            return PartialView("_CreateUpdate", viewModel);
        }
         
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }
         
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteCourseCommand(id));

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
            }
            else
            {
                TempData["ErrorMessage"] = response.Errors.Count > 0 ? string.Join(" | ", response.Errors) : response.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
