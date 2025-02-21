using Ass1.Models;
using Ass1.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Ass1.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetAllCategories() => _categoryRepository.GetAll().ToList();

        public Category? GetCategoryById(short id) => _categoryRepository.GetById(id);

        public void AddCategory(Category category) => _categoryRepository.Insert(category);

        public void UpdateCategory(Category updatedCategory)
        {
            var category = _categoryRepository.GetById(updatedCategory.CategoryId);
            if (category == null) return;

            category.CategoryName = updatedCategory.CategoryName;
            category.CategoryDesciption = updatedCategory.CategoryDesciption;
            category.ParentCategoryId = updatedCategory.ParentCategoryId;
            category.IsActive = updatedCategory.IsActive;

            _categoryRepository.Update(category);
        }

        public void DeleteCategory(short id)
        {
            var category = _categoryRepository.GetById(id);
            if (category != null)
            {
                _categoryRepository.Delete(category);
            }
        }
    }
}
