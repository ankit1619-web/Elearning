using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class SubjectsController : Controller
    {
        private readonly IMediator _mediator;

        public SubjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid paperId, string search = "")
        {
            if (paperId == Guid.Empty)
                return RedirectToAction("Index", "Exams");

            var response = await _mediator.Send(new GetSubjectsByPaperQuery(paperId, search));

            var subjects = response.IsSuccess ? response.Data : new List<SubjectDto>();

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["PaperId"] = paperId;
            ViewData["SearchTerm"] = search;

            return View(subjects);
        }

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial(Guid paperId)
        {
            var viewModel = new SubjectViewModel
            {
                PaperId = paperId
            };
            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetSubjectByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new SubjectViewModel
            {
                SubjectId = response.Data.SubjectId,
                PaperId = response.Data.PaperId,
                SubjectName = response.Data.SubjectName,
                Description = response.Data.Description,
                DisplayOrder = response.Data.DisplayOrder,
                IsActive = response.Data.IsActive
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectViewModel model)
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
            var response = await _mediator.Send(new DeleteSubjectCommand(id));
            return Json(response);
        }
    }
}
