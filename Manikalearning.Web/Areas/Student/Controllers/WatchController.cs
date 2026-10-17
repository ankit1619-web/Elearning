 using Manikalearning.Application.DTOs; 
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Features.Handlers.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")] 
    public class WatchController : Controller
    {
        private readonly IFileStorageService _fileService;
        private readonly IMediator _mediator;

        public WatchController(IMediator mediator, IFileStorageService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
         
        [HttpGet("Index/{courseId}/{lessonId?}")]
        [RequireCourseAccess("courseId", "lessonId")]      
        [RequireLectureAccess("lessonId", true)]
        public async Task<IActionResult> Index(Guid courseId, Guid? lessonId, CancellationToken cancellationToken)
        {
            var userId = UserId;

            var overview = await _mediator.Send(new GetWatchOverviewQuery(userId, courseId), cancellationToken);

            var activeLessonId = lessonId ?? overview.Modules.FirstOrDefault()?.Lessons.FirstOrDefault()?.LessonId;

            LessonPlayerViewModel? currentLesson = null;
            if (activeLessonId.HasValue && activeLessonId.Value != Guid.Empty)
            {
                currentLesson = await _mediator.Send(new GetLessonPlayerQuery(userId, courseId, activeLessonId.Value), cancellationToken);
                await ApplyPlanAccessAsync(currentLesson, cancellationToken);
            }
            var viewModel = new WatchIndexViewModel
            {
                CourseOverview = overview,
                CurrentLesson = currentLesson ?? new LessonPlayerViewModel()
            };

            return View(viewModel);
        }

        [HttpGet("GetLesson")]
        [RequireLectureAccess("lessonId")]              
        public async Task<IActionResult> GetLesson(Guid courseId, Guid lessonId, CancellationToken cancellationToken)
        {
            
            var lessonModel = await _mediator.Send(new GetLessonPlayerQuery(UserId, courseId, lessonId), cancellationToken);
            await ApplyPlanAccessAsync(lessonModel, cancellationToken);
            return PartialView("_LessonPlayerPartial", lessonModel); 
        }

        [HttpGet("AskDoubtPartial/{lessonId:guid}")]
        [RequireLectureAccess("lessonId")]
        public async Task<IActionResult> AskDoubtPartial(Guid lessonId)
        {
            var userId = UserId;
            if (!await _mediator.Send(new HasFeatureQuery(userId, PlanFeatureCodes.Doubts)))
                return PartialView("_FeatureLocked", PlanFeatureCodes.Doubts);

            return PartialView("_AskDoubtPartial", new AskDoubtViewModel { LessonId = lessonId });
        }

        [HttpGet("GetMedia")]
        [Authorize]
        public async Task<IActionResult> GetMedia([FromQuery] string fileName, [FromQuery] string mediaType = "pdfs", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest("File name is required.");
           //  if (!string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal)) return BadRequest("Invalid file name.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(mediaType ?? "", "^[A-Za-z]{1,20}$")) return BadRequest("Invalid media type.");

            var isPdf = mediaType!.Equals("pdfs", StringComparison.OrdinalIgnoreCase);
            var feature = isPdf ? PlanFeatureCodes.Notes : PlanFeatureCodes.Lectures;

            var access = await _mediator.Send(new CheckMediaAccessQuery(UserId, fileName), cancellationToken);
            if (!access.HasAccess) return StatusCode(StatusCodes.Status403Forbidden);
             
            if (!await _mediator.Send(new HasFeatureQuery(UserId, feature)))
            {
                Response.Headers["X-Feature-Locked"] = feature;
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var filePath = _fileService.GetFilePath(fileName, mediaType);
            if (!System.IO.File.Exists(filePath)) return NotFound("Requested media file does not exist.");

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
            return PhysicalFile(filePath, isPdf ? "application/pdf" : "video/mp4", enableRangeProcessing: true);
        }
         
        [HttpGet("FeatureLocked")]
        [Authorize]
        public IActionResult FeatureLocked([FromQuery] string code)
        {
            if (!PlanFeatureCodes.All.Contains(code, StringComparer.OrdinalIgnoreCase)) return BadRequest();
            return PartialView("_FeatureLocked", code);
        }

        [HttpPost("AskDoubt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskDoubt(AskDoubtViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AskDoubtPartial", model);
            }

            var userId = UserId;
             
            var access = await _mediator.Send(new CheckLectureAccessQuery(userId, model.LessonId), cancellationToken);
            if (!access.HasAccess) return Forbid();
             
            if (!await _mediator.Send(new HasFeatureQuery(userId, PlanFeatureCodes.Doubts), cancellationToken)) return Forbid();

            var response = await _mediator.Send(
                new SaveAskDoubtCommand(userId, model.LessonId, model.DoubtText),
                cancellationToken
            );
            return Json(response);
        }

        [HttpPost("ToggleBookmark")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBookmark(Guid? lectureId, Guid? questionId, CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }
            if (!await _mediator.Send(new HasFeatureQuery(userId, PlanFeatureCodes.BookMark)))
                return Json(new
                {
                    success = false,
                    isBookmarked = false,
                    message = "You do not have access to this feature."
                });

            if (lectureId.HasValue)                                  
            {
                var access = await _mediator.Send(new CheckLectureAccessQuery(userId, lectureId.Value), cancellationToken);
                if (!access.HasAccess) return Forbid();
            }

            var command = new ToggleBookmarkCommand(userId, lectureId, questionId);
            var result = await _mediator.Send(command, cancellationToken);
            return Json(new
            {
                success = result.IsSuccess,
                isBookmarked = result.Data,
                message = result.Message
            });
        }
        [HttpPost("SaveProgress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProgress([FromForm] Guid lectureId, [FromForm] int position, [FromForm] int duration,
                                                      [FromForm] bool notesRead, [FromForm] bool markComplete, CancellationToken cancellationToken)
        {
            var userId = UserId;
            var access = await _mediator.Send(new CheckLectureAccessQuery(userId, lectureId), cancellationToken);
            if (!access.HasAccess) return Forbid();

            var r = await _mediator.Send(
                new SaveLectureProgressCommand(userId, lectureId, position, duration, notesRead, markComplete), cancellationToken);
            return Json(new { success = true, isCompleted = r.IsCompleted, watchedSeconds = r.LastPositionSeconds });
        }
        private async Task ApplyPlanAccessAsync(LessonPlayerViewModel m, CancellationToken ct)
        {
            if (m.LessonId == Guid.Empty) return;
            m.HasNotesPdf = !string.IsNullOrEmpty(m.NotesUrl);

            if (m.IsFree) return;   

            var userId = UserId;
            m.CanWatchVideo = await _mediator.Send(new HasFeatureQuery(userId, PlanFeatureCodes.Lectures), ct);
            m.CanOpenNotes = await _mediator.Send(new HasFeatureQuery(userId, PlanFeatureCodes.Notes), ct);
            m.CanAskDoubt = await _mediator.Send(new HasFeatureQuery(userId, PlanFeatureCodes.Doubts), ct);
             
            if (!m.CanWatchVideo) m.VideoUrl = null;
            if (!m.CanOpenNotes) { m.NotesUrl = null; m.Notes = null; }
        }
    }
}