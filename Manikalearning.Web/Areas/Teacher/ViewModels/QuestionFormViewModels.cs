using Manikalearning.Application.Features.Handlers.Teacher;

namespace Manikalearning.Web.Areas.Teacher.ViewModels
{
    public class QuestionFormViewModels
    {
        public TeacherQuestionForm Form { get; set; } = new();
        public List<LookupItem> Subjects { get; set; } = new();
        public List<LookupItem> Sections { get; set; } = new();
        public List<LookupItem> Topics { get; set; } = new();
        public List<LookupItem> SubTopics { get; set; } = new();
        public List<LookupItem> Passages { get; set; } = new();
    }
}
