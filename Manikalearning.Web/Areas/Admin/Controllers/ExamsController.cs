using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Web.Areas.Admin.ViewModels;
using Manikalearning.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ExamsController : Controller
    {
        private readonly IMediator _mediator;

        public ExamsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "")
        {
            var response = await _mediator.Send(new GetAllExamsQuery(search));

            var exams = response.IsSuccess ? response.Data : new List<ExamDto>();

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["SearchTerm"] = search;
            return View(exams);
        }

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial()
        {
            return PartialView("_CreateUpdate", new ViewModels.ExamViewModel());
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetExamByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new ViewModels.ExamViewModel
            {
                ExamId = response.Data.ExamId,
                ExamName = response.Data.ExamName,
                ExamCode = response.Data.ExamCode,
                Description = response.Data.Description,
                DisplayOrder = response.Data.DisplayOrder,
                IsActive = response.Data.IsActive
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ViewModels.ExamViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ViewModels.ExamViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteExamCommand(id));
            return Json(response); 
        }
    }
}