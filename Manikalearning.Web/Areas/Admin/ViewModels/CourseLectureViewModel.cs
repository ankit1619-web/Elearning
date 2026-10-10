using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class CourseLectureViewModel
    {
        public Guid LectureId { get; set; }

        [Required(ErrorMessage = "Section ID is required.")]
        public Guid CourseSectionId { get; set; }

        [Required(ErrorMessage = "Lecture Title is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
        [Display(Name = "Lecture Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Video URL / Embed Link")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? VideoUrl { get; set; }

        [Display(Name = "Resource / PDF URL")]
        public string? PdfUrl { get; set; } 
        public IFormFile? PdfFile { get; set; }
        public IFormFile? VideoFile { get; set; }

        [Display(Name = "Duration (in Seconds)")]
        [Range(0, 1000000, ErrorMessage = "Duration must be between 0 and 1000000 Seconds.")]
        public int DurationInMinutes { get; set; } = 0;

        [Display(Name = "Display Order")]
        [Range(0, int.MaxValue, ErrorMessage = "Display order must be 0 or greater.")]
        public int DisplayOrder { get; set; } = 1;

        [Display(Name = "Allow Free Preview")]
        public bool IsPreview { get; set; } = false;

        [Display(Name = "Publish Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;

        public bool IsEdit => LectureId != Guid.Empty;
        public Guid? TopicId { get; set; }
        public IEnumerable<SelectListItem>? Topics { get; set; }
        public CreateCourseLectureCommand ToCreateCommand()
        {
            return new CreateCourseLectureCommand(CourseSectionId, Title, VideoUrl, PdfUrl,Notes, DurationInMinutes, DisplayOrder, IsPreview, TopicId);
        }

        public UpdateCourseLectureCommand ToUpdateCommand()
        {
            return new UpdateCourseLectureCommand(LectureId, CourseSectionId, Title, VideoUrl, PdfUrl,Notes, DurationInMinutes, DisplayOrder, IsPreview, IsActive, TopicId);
        }
    }
}
