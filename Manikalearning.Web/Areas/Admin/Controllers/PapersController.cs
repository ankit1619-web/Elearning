using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Papers;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class PapersController : Controller
    {
        private readonly IMediator _mediator;

        public PapersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid examId, string search = "")
        {
            if (examId == Guid.Empty)
                return RedirectToAction("Index", "Exams");

            var response = await _mediator.Send(new GetPapersByExamQuery(examId, search));

            var papers = response.IsSuccess ? response.Data : new List<PaperDto>();

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["ExamId"] = examId;
            ViewData["ExamName"] = response.Message; // Handler returns ExamName in Message or metadata
            ViewData["SearchTerm"] = search;

            return View(papers);
        }

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial(Guid examId)
        {
            var viewModel = new PaperViewModel
            {
                ExamId = examId
            };
            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetPaperByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new PaperViewModel
            {
                PaperId = response.Data.PaperId,
                ExamId = response.Data.ExamId,
                PaperName = response.Data.PaperName,
                PaperCode = response.Data.PaperCode,
                Description = response.Data.Description,
                DisplayOrder = response.Data.DisplayOrder,
                IsActive = response.Data.IsActive
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaperViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PaperViewModel model)
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
            var response = await _mediator.Send(new DeletePaperCommand(id));
            return Json(response);
        }
    }
}
