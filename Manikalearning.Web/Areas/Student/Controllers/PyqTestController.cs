using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Features.Handlers.Assessments.Pyq;
using Manikalearning.Application.Features.Handlers.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    public class PyqRunState : PyqTestState           
    {
        public string Kind { get; set; } = "";
        public string Key { get; set; } = "";
        public string BackUrl { get; set; } = "/"; 
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? SubjectId { get; set; }
        public string? SessionName { get; set; }
    }

    [Area("Student")]
    [AllowAnonymous]                                      
    [Route("pyq-test")]
    public class PyqTestController : Controller
    {
        private const string SessionKey = "pyq_run_state";
        private const string Flow = "PyqTest";            
        private readonly IMediator _mediator;

        public PyqTestController(IMediator mediator) => _mediator = mediator;
         
        [HttpGet("start/{kind}/{key}")]
        public async Task<IActionResult> Start(string kind, string key, Guid? courseId, string? back,
                                               Guid? examId, Guid? paperId, Guid? subjectId, string? session, CancellationToken ct)
        {
            kind = (kind ?? "").ToLowerInvariant();
            var userId = CurrentUserId();

            var st = new PyqRunState
            {
                Kind = kind,
                Key = key,
                BackUrl = ResolveBack(kind, courseId, back, key, userId),
                ExamId = examId,
                PaperId = paperId,
                SubjectId = subjectId,
                SessionName = string.IsNullOrWhiteSpace(session) ? null : session.Trim()
            };

            var data = await LoadAsync(st, userId, ct);
            if (data?.Questions == null || data.Questions.Count == 0)
            {
                TempData["ErrorMessage"] = "There are no questions here yet.";
                return Redirect(st.BackUrl);
            }

            st.StartedAtUtc = DateTime.UtcNow;
            st.DurationSeconds = data.Questions.Count * 60;           
            st.Answers = data.Questions.Select(x => new PyqAnswerState { QuestionId = x.QuestionId }).ToList();
            Save(st);
            return RedirectToAction(nameof(Question), new { q = 0 });
        }
         
        [HttpGet("question")]
        public async Task<IActionResult> Question(int q, CancellationToken ct)
        {
            var st = Load();
            if (st == null) return Redirect("/");
            if (SecondsLeft(st) <= 0) return await FinishAsync(st, auto: true, ct);

            var userId = CurrentUserId();
            var data = await LoadAsync(st, userId, ct);
            if (data?.Questions == null) { HttpContext.Session.Remove(SessionKey); return Redirect(st.BackUrl); }

            q = Math.Clamp(q, 0, st.Answers.Count - 1);
            var question = data.Questions.FirstOrDefault(x => x.QuestionId == st.Answers[q].QuestionId);
            if (question == null)
            {
                HttpContext.Session.Remove(SessionKey);
                TempData["ErrorMessage"] = "This test is no longer available. Please start again.";
                return Redirect(st.BackUrl);
            }

            if (st.Answers[q].Status == 0) { st.Answers[q].Status = 1; Save(st); }

            var policy = await GetSavePolicyAsync(userId, ct);

            var vm = new PyqQuestionVm
            {
                Controller = Flow,
                ExitUrl = st.BackUrl,
                CanBookmark = policy.CanSave,
                Question = question,
                Index = q,
                Total = st.Answers.Count,
                Selected = st.Answers[q].Selected,
                Bookmarked = st.Answers[q].Bookmarked,
                Statuses = st.Answers.Select(a => a.Status).ToList(),
                SecondsLeft = SecondsLeft(st),
                IsPreviewOnly = data.IsPreviewOnly,
                BannerMessage = data.BannerMessage
            };

            if (!policy.CanSave)                                  
            {
                vm.IsPreviewOnly = true;
                vm.BannerMessage = policy.Banner;
                vm.BannerLink = policy.Link;
                vm.BannerLinkText = policy.LinkText;
            }
            return View("Test", vm);                               
        }

        [HttpPost("question")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Question(int q, int? option, string? nav, CancellationToken ct)
        {
            var st = Load();
            if (st == null) return Redirect("/");
            if (SecondsLeft(st) <= 0) return await FinishAsync(st, auto: true, ct);
            if (q < 0 || q >= st.Answers.Count) q = 0;

            var a = st.Answers[q];
            var last = st.Answers.Count - 1;

            if (nav == "clear")
            {
                a.Selected = null; a.Status = 1;
                Save(st);
                return RedirectToAction(nameof(Question), new { q });
            }

            a.Selected = option;
            if (nav == "save") a.Status = option != null ? 2 : 1;
            else if (nav == "mark") a.Status = 3;
            else if (a.Status != 3) a.Status = option != null ? 2 : 1;

            var next = q;
            if (nav == "save" || nav == "mark" || nav == "next") next = Math.Min(q + 1, last);
            else if (nav == "prev") next = Math.Max(q - 1, 0);
            else if (nav == "bookmark")
            { 
                var userId = CurrentUserId();
                if (userId != null && (await GetSavePolicyAsync(userId, ct)).CanSave)
                {
                    var res = await _mediator.Send(new ToggleBookmarkCommand(userId.Value, null, a.QuestionId), ct);
                    a.Bookmarked = res.Data;
                }
            }
            else if (nav != null && nav.StartsWith("go:") && int.TryParse(nav.AsSpan(3), out var go)) next = Math.Clamp(go, 0, last);
            else if (nav == "submit") { Save(st); return RedirectToAction(nameof(Review)); }

            Save(st);
            return RedirectToAction(nameof(Question), new { q = next });
        }
         
        [HttpGet("review")]
        public async Task<IActionResult> Review(CancellationToken ct)
        {
            var st = Load();
            if (st == null) return Redirect("/");
            if (SecondsLeft(st) <= 0) return await FinishAsync(st, auto: true, ct);

            return View(new PyqReviewVm
            {
                Controller = Flow,
                Total = st.Answers.Count,
                Answered = st.Answers.Count(a => a.Selected != null),
                NotAnswered = st.Answers.Count(a => a.Selected == null),
                Marked = st.Answers.Count(a => a.Status == 3),
                SecondsLeft = SecondsLeft(st)
            });
        }

        [HttpPost("review")]
        [ValidateAntiForgeryToken]
        [ActionName("Review")]
        public async Task<IActionResult> ReviewConfirmed(CancellationToken ct)
        {
            var st = Load();
            if (st == null) return Redirect("/");
            return await FinishAsync(st, auto: false, ct);
        }
         
        private async Task<IActionResult> FinishAsync(PyqRunState st, bool auto, CancellationToken ct)
        {
            var userId = CurrentUserId();
            var policy = await GetSavePolicyAsync(userId, ct);

            var answers = st.Answers.Select(a => new UserAnswerDto
            {
                QuestionId = a.QuestionId,
                SelectedOptionNumber = a.Selected,
                IsAnswered = a.Selected != null,
                IsMarkedForReview = a.Status == 3
            }).ToList();

            TopicTestResultDto? r;

            if (policy.CanSave)
            {
                var spent = Math.Min(st.DurationSeconds, (int)(DateTime.UtcNow - st.StartedAtUtc).TotalSeconds);
                 
                var res = await _mediator.Send(new SubmitTopicPyqTestCommand(Guid.Empty, userId!.Value, null, Math.Max(1, spent), answers), ct);

                if (!res.IsSuccess || res.Data == null)
                {
                    TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(res.Message) ? "We could not submit your test. Please try again." : res.Message;
                    if (auto) { HttpContext.Session.Remove(SessionKey); return Redirect(st.BackUrl); }    
                    return RedirectToAction(nameof(Review));                         
                }
                r = res.Data;
            }
            else
            {
                try { r = await _mediator.Send(new GradePyqTestQuery(answers), ct); }
                catch (ValidationException) { HttpContext.Session.Remove(SessionKey); return Redirect(st.BackUrl); }
            }

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
                AutoSubmitted = auto,
                ExitUrl = st.BackUrl,
                ExitLabel = "Back",
                Saved = policy.CanSave,
                SaveNotice = policy.CanSave ? null : policy.ResultNotice,
                SaveLink = policy.CanSave ? null : policy.Link,
                SaveLinkText = policy.CanSave ? null : policy.LinkText
            });
        }
         
        private async Task<TopicPyqListDto?> LoadAsync(PyqRunState st, Guid? userId, CancellationToken ct)
        {
            switch (st.Kind)
            {
                case "section" when Guid.TryParse(st.Key, out var sectionId):
                    return (await _mediator.Send(new GetPYQQuestionsQuery(sectionId, userId), ct)).Data;

                case "topic" when Guid.TryParse(st.Key, out var lectureId):
                    return (await _mediator.Send(new GetTopicPyqsTestQuery(lectureId, userId), ct)).Data;

                case "year" when int.TryParse(st.Key, out var year):
                    try { return await _mediator.Send(new GetPyqByYearQuery(year, st.ExamId, st.PaperId, st.SessionName, st.SubjectId), ct); }
                    catch (ValidationException) { return null; }

                default:
                    return null;
            }
        }
         
        private record SavePolicy(bool CanSave, string Banner, string ResultNotice, string Link, string LinkText);

        private async Task<SavePolicy> GetSavePolicyAsync(Guid? userId, CancellationToken ct)
        {
            if (userId != null && User.IsInRole("Student") &&
                await _mediator.Send(new HasFeatureQuery(userId.Value, PlanFeatureCodes.Pyq), ct))
                return new SavePolicy(true, "", "", "", "");

            if (userId == null || !User.IsInRole("Student"))
            {
                var login = Url.Action("Login", "Account", new { area = "", returnUrl = Request.Path.ToString() }) ?? "/";
                return new SavePolicy(false,
                    "You are practising as a guest. Your answers are not saved.",
                    "Log in with a plan that includes PYQs to save your attempts, mistakes and bookmarks.",
                    login, "Log in");
            }

            return new SavePolicy(false,
                "Your plan does not include saving PYQ results.",
                "Your current plan does not include saving PYQ attempts. Choose a plan with PYQs to keep your results and build your Mistake Book.",
                "/Student/Subscription", "See plans");
        }
         
        private string ResolveBack(string kind, Guid? courseId, string? back, string key, Guid? userId)
        {
            if (!string.IsNullOrEmpty(back) && Url.IsLocalUrl(back)) return back;          

            if (kind == "year") return Url.Action("Index", "PyqYears", new { area = "Student" }) ?? "/";    

            if (userId != null && User.IsInRole("Student"))
            {
                if (kind == "topic" && courseId.HasValue && Guid.TryParse(key, out var lectureId))
                    return Url.Action("Index", "Watch", new { area = "Student", courseId = courseId.Value, lessonId = lectureId }) ?? "/Student/MyCourses";
                return Url.Action("Index", "PYQPractice", new { area = "Student" }) ?? "/";
            }
            return "/";
        }

        private Guid? CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        private static int SecondsLeft(PyqTestState st) =>
            Math.Max(0, st.DurationSeconds - (int)(DateTime.UtcNow - st.StartedAtUtc).TotalSeconds);

        private PyqRunState? Load()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<PyqRunState>(json);
        }

        private void Save(PyqRunState st) => HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(st));
    }
}
