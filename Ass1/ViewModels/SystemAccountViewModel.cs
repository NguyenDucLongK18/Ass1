using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ass1.ViewModels
{
    public class SystemAccountViewModel
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(50, ErrorMessage = "Account name cannot exceed 50 characters.")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [Range(0, 3, ErrorMessage = "Invalid role")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Password must be between 8 and 100 characters.")]
        [DataType(DataType.Password)]
        public string? AccountPassword { get; set; }

        public List<SelectListItem> Roles => new List<SelectListItem>
        {
            new SelectListItem { Text = "Staff", Value = "1" },
            new SelectListItem { Text = "Lecturer", Value = "2" }
        };

        public bool? IsActive { get; set; }
    }
}
