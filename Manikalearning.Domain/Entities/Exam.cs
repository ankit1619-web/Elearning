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

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Paper> Papers { get; set; }
        public ICollection<Question> Questions { get; set; }
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
        public string Slug { get; set; }     // e.g., 'paper-1'

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
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

        [Required, MaxLength(150)]
        public string Slug { get; set; }     

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
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

        [Required, MaxLength(150)]
        public string Slug { get; set; }     // e.g., 'data-structures'

        public string Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
        public ICollection<Topic> Topics { get; set; }
    }

    [Table("Topics")]
    public class Topic
    {
        [Key]
        public Guid TopicId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SectionId { get; set; }
        [ForeignKey("SectionId")]
        public Section Section { get; set; }

        [Required, MaxLength(150)]
        public string TopicName { get; set; } // e.g., 'Arrays and Linked Lists' or 'Learner Characteristics'

        [Required, MaxLength(150)]
        public string Slug { get; set; }     // e.g., 'arrays-and-linked-lists'

        public string Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<SubTopic> SubTopics { get; set; }
        public ICollection<Question> Questions { get; set; }
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
        public string SubTopicName { get; set; } // e.g., 'Singly Linked Lists' or 'Adolescent Learners'

        [Required, MaxLength(150)]
        public string Slug { get; set; }     // e.g., 'singly-linked-lists'

        public string Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Question> Questions { get; set; }
    }
}
