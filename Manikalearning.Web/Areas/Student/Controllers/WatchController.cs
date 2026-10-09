using Azure;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet("Index/{courseId}/{lessonId?}")]
        public async Task<IActionResult> Index(Guid courseId, Guid? lessonId, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
             
            var overview = await _mediator.Send(new GetWatchOverviewQuery(userId, courseId), cancellationToken);
             
            var activeLessonId = lessonId ?? overview.Modules.FirstOrDefault()?.Lessons.FirstOrDefault()?.LessonId;

            LessonPlayerViewModel? currentLesson = null;
            if (activeLessonId.HasValue && activeLessonId.Value != Guid.Empty)
            {
                currentLesson = await _mediator.Send(new GetLessonPlayerQuery(userId, courseId, activeLessonId.Value), cancellationToken);
            } 
            var viewModel = new WatchIndexViewModel
            {
                CourseOverview = overview,
                CurrentLesson = currentLesson ?? new LessonPlayerViewModel()
            };

            return View(viewModel);
        }

        [HttpGet("GetLesson")]
        public async Task<IActionResult> GetLesson(Guid courseId, Guid lessonId, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var lessonModel = await _mediator.Send(new GetLessonPlayerQuery(userId, courseId, lessonId), cancellationToken);

            return PartialView("_LessonPlayerPartial", lessonModel);
        }

        [HttpGet("AskDoubtPartial/{lessonId:guid}")] 
        public async Task<IActionResult> AskDoubtPartial(Guid lessonId, CancellationToken cancellationToken)
        {
            var model = new AskDoubtViewModel
            {
                LessonId = lessonId
            };
            return PartialView("_AskDoubtPartial", model);
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


        [HttpPost("AskDoubt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskDoubt(AskDoubtViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AskDoubtPartial", model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
             
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

            var command = new ToggleBookmarkCommand(userId, lectureId, questionId);
            var result = await _mediator.Send(command, cancellationToken);
            return Json(new
            {
                success = result.IsSuccess,
                isBookmarked = result.Data,
                message = result.Message
            }); 
        }

        [AllowAnonymous]
        [HttpGet("TopicPyqsTestPartial/{lectureId}")]
        public async Task<IActionResult> TopicPyqsTestPartial(Guid lectureId, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var parsedId))
            {
                userId = parsedId;
            }

            var query = new GetTopicPyqsTestQuery(lectureId, userId);
            var result = await _mediator.Send(query, cancellationToken);

            return PartialView("_TopicPyqsTestPartial", result.Data ?? new TopicPyqListDto());
        }
        [HttpPost("SubmitTopicPyqTest")]
        public async Task<IActionResult> SubmitTopicPyqTest([FromBody] SubmitTopicPyqTestCommand command, CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return Ok(new
                {
                    success = true,
                    isGuest = true,
                    message = "Guest mode: Result calculated locally without saving to database."
                });
            }

            var authenticatedCommand = new SubmitTopicPyqTestCommand(
                command.LectureId,
                userId,
                command.TestId,
                command.TimeTakenSeconds,
                command.Answers
            );

            var result = await _mediator.Send(authenticatedCommand, cancellationToken);

            return Json(result);
        }
    }
}
