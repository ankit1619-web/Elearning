using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("ContactMessages")]
    public class ContactMessage
    {
        [Key]
        public Guid ContactMessageId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(150)]
        public string Email { get; set; } = "";

        [Required, MaxLength(200)]
        public string Subject { get; set; } = "";

        [Required, MaxLength(2000)]
        public string Message { get; set; } = "";

        [MaxLength(64)]
        public string? IpAddress { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
