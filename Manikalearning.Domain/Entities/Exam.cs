using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Exams")]
    public class Exam
    {
        [Key]
        public Guid ExamId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string ExamName { get; set; } // e.g., 'UGC NET'

        [Required, MaxLength(150)]
        public string ExamCode { get; set; }     // e.g., 'ugc-net'

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
        public ICollection<Paper> Papers { get; set; }
        public ICollection<Question> Questions { get; set; }
    }

     
    [Table("ExamCourses")]
    public class ExamCourse
    {
        public Guid ExamId { get; set; }
        public Guid CourseId { get; set; }
         
        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
    [Table("Papers")]
    public class Paper
    {
        [Key]
        public Guid PaperId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ExamId { get; set; }
        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }

        [Required, MaxLength(150)]
        public string PaperName { get; set; } // e.g., 'Paper 1 (General Teaching & Research Aptitude)' or 'Paper 2'

        [Required, MaxLength(150)]
        public string PaperCode { get; set; }     // e.g., 'paper-1'

        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
          
        public ICollection<Question> Questions { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }

    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public Guid SubjectId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PaperId { get; set; }
        [ForeignKey("PaperId")]
        public Paper Paper { get; set; }

        [Required, MaxLength(150)]
        public string SubjectName { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; } = true; 
        public ICollection<Section> Sections { get; set; }
        public ICollection<Question> Questions { get; set; }
    }

    [Table("Sections")]
    public class Section
    {
        [Key]
        public Guid SectionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        [Required, MaxLength(150)]
        public string SectionName { get; set; } // e.g., 'Data Structures' under Computer Science, or 'Characteristics of Teaching'
           
        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>(); 
    }

    [Table("Topics")]
    public class Topic
    {
        [Key]
        public Guid TopicId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SectionId { get; set; } 

        [Required, MaxLength(150)]
        public string TopicName { get; set; } 

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; } = null!;
    }

    [Table("SubTopics")]
    public class SubTopic
    {
        [Key]
        public Guid SubTopicId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TopicId { get; set; }
        [ForeignKey("TopicId")]
        public Topic Topic { get; set; }

        [Required, MaxLength(150)]
        public string SubTopicName { get; set; }  
        public bool IsActive { get; set; } = true; 
        // Navigation Properties
        public ICollection<Question> Questions { get; set; }
    }
}
