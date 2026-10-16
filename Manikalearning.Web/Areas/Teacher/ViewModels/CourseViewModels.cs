using Manikalearning.Application.Features.Handlers.Teacher;

namespace Manikalearning.Web.Areas.Teacher.ViewModels
{
    public class LectureFormVm
    {
        public TeacherLectureForm Form { get; set; } = new();
        public List<LookupItem> Subjects { get; set; } = new();
        public List<LookupItem> Sections { get; set; } = new();    
        public List<LookupItem> Topics { get; set; } = new();    
    }
}
