using Manikalearning.Application.Features.Handlers.Teacher;
using Manikalearning.Web.Areas.Teacher.ViewModels;
using Manikalearning.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class CoursesController : TeacherBaseController
    {
        private const long BigUpload = 524_288_000; // 500 MB (video)
        private readonly IWebHostEnvironment _env;

        public CoursesController(IMediator mediator, IWebHostEnvironment env) : base(mediator) { _env = env; }

        // ------------------------------ COURSES ------------------------------
        [HttpGet("")]
        public async Task<IActionResult> Index()
            => View(await Mediator.Send(new GetTeacherCoursesQuery(TeacherId)));

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial() => PartialView("_CourseForm", new TeacherCourseForm());

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var form = await Mediator.Send(new GetTeacherCourseFormQuery(TeacherId, id));
            return form == null ? NotFound() : PartialView("_CourseForm", form);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TeacherCourseForm form, IFormFile? thumbnail)
        {
            var (url, err) = await TeacherFileStore.SaveAsync(_env, thumbnail, "courses", TeacherFileStore.Images, TeacherFileStore.MaxImage);
            if (err != null) return Json(TeacherActionResult.Fail("Thumbnail: " + err));
            if (url != null) form.ThumbnailUrl = url;

            return Json(await Mediator.Send(new SaveTeacherCourseCommand(TeacherId, form)));
        }

        [HttpPost("TogglePublish/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(Guid id)
            => Json(await Mediator.Send(new ToggleCoursePublishCommand(TeacherId, id)));

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
            => Json(await Mediator.Send(new DeleteTeacherCourseCommand(TeacherId, id)));

        // ------------------------------ MANAGE (sections + lectures) ------------------------------
        [HttpGet("Manage/{id:guid}")]
        public async Task<IActionResult> Manage(Guid id)
        {
            var model = await Mediator.Send(new GetCourseManageQuery(TeacherId, id));
            return model == null ? NotFound() : View(model);
        }

        // ------------------------------ SECTIONS ------------------------------
        [HttpGet("SectionPartial")]
        public async Task<IActionResult> SectionPartial(Guid courseId, Guid? id)
        {
            if (id != null && id != Guid.Empty)
            {
                var form = await Mediator.Send(new GetTeacherSectionFormQuery(TeacherId, id.Value));
                return form == null ? NotFound() : PartialView("_SectionForm", form);
            }
            return PartialView("_SectionForm", new TeacherSectionForm { CourseId = courseId });
        }

        [HttpPost("SaveSection")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSection(TeacherSectionForm form)
            => Json(await Mediator.Send(new SaveTeacherSectionCommand(TeacherId, form)));

        [HttpPost("DeleteSection/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSection(Guid id)
            => Json(await Mediator.Send(new DeleteTeacherSectionCommand(TeacherId, id)));

        [HttpPost("MoveSection/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveSection(Guid id, bool up)
            => Json(await Mediator.Send(new MoveCourseSectionCommand(TeacherId, id, up)));

        // ------------------------------ LECTURES ------------------------------
        [HttpGet("LecturePartial")]
        public async Task<IActionResult> LecturePartial(Guid sectionId, Guid? id)
        {
            var form = new TeacherLectureForm { CourseSectionId = sectionId };
            if (id != null && id != Guid.Empty)
            {
                var existing = await Mediator.Send(new GetTeacherLectureFormQuery(TeacherId, id.Value));
                if (existing == null) return NotFound();
                form = existing;
            }

            var vm = new LectureFormVm { Form = form };
            vm.Subjects = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Subjects));
            if (form.PickSubjectId != null)
                vm.Sections = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Sections, form.PickSubjectId));
            if (form.PickSectionId != null)
                vm.Topics = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Topics, form.PickSectionId));

            return PartialView("_LectureForm", vm);
        }

        [HttpPost("SaveLecture")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(BigUpload)]
        [RequestFormLimits(MultipartBodyLengthLimit = BigUpload)]
        public async Task<IActionResult> SaveLecture(TeacherLectureForm form, IFormFile? videoFile, IFormFile? notesFile)
        {
            var (video, vErr) = await TeacherFileStore.SaveAsync(_env, videoFile, "lectures", TeacherFileStore.Videos, TeacherFileStore.MaxVideo);
            if (vErr != null) return Json(TeacherActionResult.Fail("Video: " + vErr));
            var (notes, nErr) = await TeacherFileStore.SaveAsync(_env, notesFile, "notes", TeacherFileStore.Docs, TeacherFileStore.MaxNotes);
            if (nErr != null) return Json(TeacherActionResult.Fail("Notes: " + nErr));

            if (video != null) form.VideoUrl = video;      // upload ho to URL field ko override
            if (notes != null) form.NotesUrl = notes;

            return Json(await Mediator.Send(new SaveTeacherLectureCommand(TeacherId, form)));
        }

        [HttpPost("DeleteLecture/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLecture(Guid id)
            => Json(await Mediator.Send(new DeleteTeacherLectureCommand(TeacherId, id)));

        [HttpPost("ToggleLecture/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLecture(Guid id, LectureFlag flag)
            => Json(await Mediator.Send(new ToggleLectureFlagCommand(TeacherId, id, flag)));

        [HttpPost("MoveLecture/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveLecture(Guid id, bool up)
            => Json(await Mediator.Send(new MoveLectureCommand(TeacherId, id, up)));
    }
}
