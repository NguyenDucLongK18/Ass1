using Ass1.Models;
using System.ComponentModel.DataAnnotations;

namespace Ass1.ViewModels
{
    public class CategoryViewModel
    {
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        public string CategoryName { get; set; } = string.Empty;

        public string CategoryDescription { get; set; } = string.Empty;

        public short? ParentCategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public Category? ParentCategory { get; set; }
        public List<CategoryViewModel>? SubCategories { get; set; } = new();
    }

}
