using System.Text.RegularExpressions;

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
    public static class MobileNumber
    { 
        public static string? Normalize(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var digits = Regex.Replace(raw, @"[\s\-\(\)\.]", "");
            if (digits.StartsWith("+91")) digits = digits[3..];
            else if (digits.StartsWith("91") && digits.Length == 12) digits = digits[2..];
            else if (digits.StartsWith("0") && digits.Length == 11) digits = digits[1..];
            return digits;
        }
        public static bool IsValid(string? normalized) => normalized != null && Regex.IsMatch(normalized, "^[6-9][0-9]{9}$");
    }
     
    public class ProfileDto
    {
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public bool IsMobileVerified { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime MemberSince { get; set; }
        public Guid? TargetExamId { get; set; }
        public DateTime? TargetExamDate { get; set; }
        public int DailyGoal { get; set; } = 30;
        public List<ExamOptionDto> Exams { get; set; } = new();
    }
}
