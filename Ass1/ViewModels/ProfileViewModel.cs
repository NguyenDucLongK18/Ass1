using System.ComponentModel.DataAnnotations;

namespace Ass1.ViewModels
{
    public class ProfileViewModel
    {
        public short AccountId { get; set; }
        [Required]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }
    }
}
