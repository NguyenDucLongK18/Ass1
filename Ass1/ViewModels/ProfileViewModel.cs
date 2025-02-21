using System.ComponentModel.DataAnnotations;

namespace Ass1.ViewModels
{
    public class ProfileViewModel
    {
        public int AccountId { get; set; }
        [Required]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }

        public List<string> Roles => new List<string> { "Lecturer", "Staff" };
        
        public int SelectedRole { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }
    }
}
