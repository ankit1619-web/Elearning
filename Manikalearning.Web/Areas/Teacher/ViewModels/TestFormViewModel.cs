using Manikalearning.Application.Features.Handlers.Teacher;

namespace Manikalearning.Web.Areas.Teacher.ViewModels
{ 
    public class TestFormViewModel
    {
        public TeacherTestForm Form { get; set; } = new();
        public List<LookupItem> Exams { get; set; } = new();
        public List<LookupItem> Papers { get; set; } = new();
        public List<LookupItem> Subjects { get; set; } = new();
        public List<LookupItem> Sections { get; set; } = new();
        public List<LookupItem> Topics { get; set; } = new();
    }
}
