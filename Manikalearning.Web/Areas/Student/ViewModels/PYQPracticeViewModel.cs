using Manikalearning.Application.DTOs;

namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class PYQPracticeViewModel
    {
        public List<SubjectDto> Subjects { get; set; } = new();
        public Guid? SelectedSubjectId { get; set; }
        public List<SectionDto> Sections { get; set; } = new();
    }
}
