using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Manikalearning.Application.Features.Commands.Assessments.Coursesection;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CourseLecturesController : Controller
    {
        private readonly IFileStorageService _fileService;
        private readonly IMediator _mediator;

        public CourseLecturesController(IMediator mediator, IFileStorageService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
        }

        [HttpGet("{sectionId:guid}")]
        [HttpGet("Index/{sectionId:guid}")]
        public async Task<IActionResult> Index(Guid sectionId)
        {
            var lecturesResponse = await _mediator.Send(new GetCourseLecturesBySectionIdQuery(sectionId));
            var lectures = lecturesResponse.IsSuccess ? lecturesResponse.Data : new List<CourseLectureDto>();

            var sectionResponse = await _mediator.Send(new GetCourseSectionByIdQuery(sectionId));
            if (sectionResponse.IsSuccess && sectionResponse.Data != null)
            {
                ViewData["SectionTitle"] = sectionResponse.Data.SectionName;
                ViewData["CourseId"] = sectionResponse.Data.CourseId;
            }

            ViewData["SectionId"] = sectionId;
            return View(lectures ?? new());
        }

        [HttpGet("CreatePartial/{sectionId:guid}")]
        public async Task<IActionResult> CreatePartial(Guid sectionId)
        {
            var topicsLookup = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));
            var topics = topicsLookup.IsSuccess ? topicsLookup.Data : new List<LookupDto>();

            var viewModel = new CourseLectureViewModel
            {
                CourseSectionId = sectionId,
                DisplayOrder = 1,
                IsActive = true,
                Topics = topics.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList()
            };
            return PartialView("_CreateUpdateLecture", viewModel);
        }

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetCourseLectureByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var topicsLookup = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));
            var topics = topicsLookup.IsSuccess ? topicsLookup.Data : new List<LookupDto>();

            var viewModel = new CourseLectureViewModel
            {
                LectureId = response.Data.LectureId,
                CourseSectionId = response.Data.CourseSectionId,
                Title = response.Data.LectureTitle,
                VideoUrl = response.Data.VideoUrl,
                PdfUrl = response.Data.NotesUrl,
                Notes = response.Data.Notes ?? "",
                DurationInMinutes = response.Data.VideoDurationSeconds ?? 0,
                DisplayOrder = response.Data.DisplayOrder,
                IsPreview = response.Data.IsFreePreview,
                IsActive = response.Data.IsPublished,
                TopicId = response.Data.TopicId,
                Topics = topics.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList()
            };

            return PartialView("_CreateUpdateLecture", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseLectureViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdateLecture", model);
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                string savedPdfFileName = await _fileService.SaveFileAsync(model.PdfFile, "Pdfs");
                model.PdfUrl = $"/Admin/CourseLectures/GetMedia/{savedPdfFileName}/Pdfs";
            }
             
            if (model.VideoFile != null && model.VideoFile.Length > 0)
            {
                string savedVideoFileName = await _fileService.SaveFileAsync(model.VideoFile, "Videos");
                model.VideoUrl = $"/Admin/CourseLectures/GetMedia/{savedVideoFileName}/Videos";
            }
            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseLectureViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdateLecture", model);
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                string savedPdfFileName = await _fileService.SaveFileAsync(model.PdfFile, "Pdfs");
                model.PdfUrl = $"/{savedPdfFileName}/Pdfs";
            }
            else if (!string.IsNullOrEmpty(model.PdfUrl))
            {
                model.PdfUrl = model.PdfUrl;
            }
             
            if (model.VideoFile != null && model.VideoFile.Length > 0)
            {
                string savedVideoFileName = await _fileService.SaveFileAsync(model.VideoFile, "Videos");
                model.VideoUrl = $"/{savedVideoFileName}/Videos";
            }
            else if (!string.IsNullOrEmpty(model.VideoUrl))
            {
                model.VideoUrl = model.VideoUrl;
            }
            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("UploadMedia")]
        public async Task<IActionResult> UploadMedia(IFormFile file, string mediaType) 
        {
            try
            {
                string savedFileName = await _fileService.SaveFileAsync(file, mediaType);
                return Ok(new { success = true, fileName = savedFileName });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetMedia")]
        [Authorize]  
        public IActionResult GetMedia([FromQuery] string fileName, [FromQuery] string mediaType = "pdfs")
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest("File name is required.");

            var filePath = _fileService.GetFilePath(fileName, mediaType);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Requested media file does not exist.");
            }

            var contentType = mediaType.Equals("pdfs", StringComparison.OrdinalIgnoreCase)
                ? "application/pdf"
                : "video/mp4";
             
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");

            return PhysicalFile(filePath, contentType, enableRangeProcessing: true);
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteCourseLectureCommand(id));
            return Json(response);
        }
    }
}