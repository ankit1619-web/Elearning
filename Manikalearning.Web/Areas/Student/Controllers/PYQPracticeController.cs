using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Web.Areas.Student.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    public class PYQPracticeController : Controller
    {
        private const string SessionKey = "pyq_test_state";

        private readonly IMediator _mediator;

        public PYQPracticeController(IMediator mediator) => _mediator = mediator;


        [HttpGet("")]
        public async Task<IActionResult> Index(Guid? subjectId, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            var subjects = await _mediator.Send(new GetSubjectsByStudentQuery { UserId = userId }, ct);
            var selectedSubjectId = subjectId ?? subjects.FirstOrDefault()?.SubjectId;

            List<SectionDto> sections = new();
            if (selectedSubjectId.HasValue)
            {
                var response = await _mediator.Send(new GetSectionsBySubjectQuery(selectedSubjectId.Value), ct);
                sections = response.Data;
            }

            var vm = new PYQPracticeViewModel
            {
                Subjects = subjects,
                SelectedSubjectId = selectedSubjectId,
                Sections = sections
            };

            return View(vm);
        }

        [HttpGet("Practice/{sectionId}")]
        public async Task<IActionResult> Practice(Guid sectionId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetPYQQuestionsQuery(sectionId, CurrentUserId()), ct);
            return PartialView("_SubjectWisePyq", result.Data ?? new TopicPyqListDto());
        }

        [HttpGet("Start/{sectionId:guid}")]
        public async Task<IActionResult> Start(Guid sectionId, CancellationToken ct)
        {
            var data = await LoadQuestionsAsync(sectionId, ct);
            if (data.Questions == null || data.Questions.Count == 0)
            {
                TempData["ErrorMessage"] = "No questions found for this topic.";
                return RedirectToAction(nameof(Index));
            }

            SaveState(new PyqTestState
            {
                SectionId = sectionId,
                StartedAtUtc = DateTime.UtcNow,
                DurationSeconds = data.Questions.Count * 60, 
                Answers = data.Questions.Select(x => new PyqAnswerState { QuestionId = x.QuestionId }).ToList()
            });

            return RedirectToAction(nameof(Question), new { q = 0 });
        }

        [HttpGet("Question")]
        public async Task<IActionResult> Question(int q, CancellationToken ct)
        {
            var st = LoadState();
            if (st == null) return RedirectToAction(nameof(Index));
            if (SecondsLeft(st) <= 0) return await FinishAsync(st, auto: true, ct);

            var data = await LoadQuestionsAsync(st.SectionId, ct);
            q = Math.Clamp(q, 0, st.Answers.Count - 1);

            var wantedId = st.Answers[q].QuestionId;
            var question = data.Questions.FirstOrDefault(x => x.QuestionId == wantedId);
            if (question == null)
            {
                TempData["ErrorMessage"] = "This test is no longer available. Please start again.";
                HttpContext.Session.Remove(SessionKey);
                return RedirectToAction(nameof(Index));
            }

            if (st.Answers[q].Status == 0) { st.Answers[q].Status = 1; SaveState(st); }

            return View("Test", new PyqQuestionVm
            {
                Question = question,
                Index = q,
                Total = st.Answers.Count,
                Selected = st.Answers[q].Selected,
                Bookmarked = st.Answers[q].Bookmarked,
                Statuses = st.Answers.Select(a => a.Status).ToList(),
                SecondsLeft = SecondsLeft(st),
                IsPreviewOnly = data.IsPreviewOnly,
                BannerMessage = data.BannerMessage
            });
        }

        [HttpPost("Question")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Question(int q, int? option, string? nav, CancellationToken ct)
        {
            var st = LoadState();
            if (st == null) return RedirectToAction(nameof(Index));
            if (SecondsLeft(st) <= 0) return await FinishAsync(st, auto: true, ct);
            if (q < 0 || q >= st.Answers.Count) q = 0;

            var a = st.Answers[q];
            var last = st.Answers.Count - 1;

            if (nav == "clear")
            {
                a.Selected = null;
                a.Status = 1;
                SaveState(st);
                return RedirectToAction(nameof(Question), new { q });
            }

            a.Selected = option;                                        
            if (nav == "save") a.Status = option != null ? 2 : 1;
            else if (nav == "mark") a.Status = 3;
            else if (a.Status != 3) a.Status = option != null ? 2 : 1;   

            var next = q;
            if (nav == "save" || nav == "mark" || nav == "next") next = Math.Min(q + 1, last);
            else if (nav == "prev") next = Math.Max(q - 1, 0);
            else if (nav == "bookmark") a.Bookmarked = !a.Bookmarked;
            else if (nav != null && nav.StartsWith("go:") && int.TryParse(nav.AsSpan(3), out var go)) next = Math.Clamp(go, 0, last);
            else if (nav == "submit") { SaveState(st); return RedirectToAction(nameof(Review)); }

            SaveState(st);
            return RedirectToAction(nameof(Question), new { q = next }); 
        }

        [HttpGet("Review")]
        public async Task<IActionResult> Review(CancellationToken ct)
        {
            var st = LoadState();
            if (st == null) return RedirectToAction(nameof(Index));
            if (SecondsLeft(st) <= 0) return await FinishAsync(st, auto: true, ct);

            return View(new PyqReviewVm
            {
                Total = st.Answers.Count,
                Answered = st.Answers.Count(a => a.Selected != null),
                NotAnswered = st.Answers.Count(a => a.Selected == null),
                Marked = st.Answers.Count(a => a.Status == 3),
                SecondsLeft = SecondsLeft(st)
            });
        }

        [HttpPost("Review")]
        [ValidateAntiForgeryToken]
        [ActionName("Review")]
        public async Task<IActionResult> ReviewConfirmed(CancellationToken ct)
        {
            var st = LoadState();
            if (st == null) return RedirectToAction(nameof(Index));
            return await FinishAsync(st, auto: false, ct);
        }
         

        private async Task<IActionResult> FinishAsync(PyqTestState st, bool auto, CancellationToken ct)
        {
            var spent = Math.Min(st.DurationSeconds, (int)(DateTime.UtcNow - st.StartedAtUtc).TotalSeconds);
             
            var res = await _mediator.Send(new SubmitPyqTestCommand
            {
                UserId = CurrentUserId() ?? Guid.Empty,
                SectionId = st.SectionId,
                TimeTakenSeconds = Math.Max(1, spent),
                Answers = st.Answers.Select(a => new PyqAnswerDto
                {
                    QuestionId = a.QuestionId,
                    SelectedOptionNumber = a.Selected,
                    IsAnswered = a.Selected != null,
                    IsMarkedForReview = a.Status == 3
                }).ToList()
            }, ct);

            var r = res;   

            HttpContext.Session.Remove(SessionKey);

            return View("TestResult", new PyqResultVm
            {
                TotalQuestions = r.TotalQuestions,
                CorrectAnswers = r.CorrectAnswers,
                WrongAnswers = r.WrongAnswers,
                SkippedQuestions = r.SkippedQuestions,
                Score = r.Score,
                TotalMarks = r.TotalMarks,
                Accuracy = r.Accuracy,
                AutoSubmitted = auto
            });
        }

        private async Task<TopicPyqListDto> LoadQuestionsAsync(Guid sectionId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetPYQQuestionsQuery(sectionId, CurrentUserId()), ct);
            return result.Data ?? new TopicPyqListDto();
        }

        private Guid? CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        private static int SecondsLeft(PyqTestState st) =>
            Math.Max(0, st.DurationSeconds - (int)(DateTime.UtcNow - st.StartedAtUtc).TotalSeconds);

        private PyqTestState? LoadState()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<PyqTestState>(json);
        }

        private void SaveState(PyqTestState st) =>
            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(st));
    }
}