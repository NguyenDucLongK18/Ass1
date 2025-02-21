using System.ComponentModel.DataAnnotations;

namespace Ass1.ViewModels
{
    public class TagViewModel
    {
        public int TagId { get; set; }

        [Required(ErrorMessage = "Tag Name is required.")]
        public string TagName { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
