namespace Manikalearning.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string? FullName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsMobileVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
    public class RoleDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public int UsersCount { get; set; }
    }  
    public class ManageUserRoleViewModel
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public Guid? SelectedRoleId { get; set; }
        public List<RoleDto> AvailableRoles { get; set; } = new();
    }
}
