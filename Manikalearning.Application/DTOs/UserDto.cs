namespace Manikalearning.Application.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public string? ProfileImage { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsMobileVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<RoleDto> Roles { get; set; } = new();
    }
}
