using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class UserVM
    {
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public string? ProfileImage { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; }

        public bool IsMobileVerified { get; set; }


        public List<Guid> RoleIds { get; set; } = new();

        public List<RoleDropdownVM> Roles { get; set; } = new();


        // List
        public string? Search { get; set; }

        public List<UserVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }


    public class RoleDropdownVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
