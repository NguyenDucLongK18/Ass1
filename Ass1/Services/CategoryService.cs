using Ass1.Models;
using Ass1.Repositories;
using Ass1.ViewModels;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Ass1.Services
{
    public class CategoryService : IService<Category, CategoryViewModel>
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<CategoryViewModel> GetAllCategories() {
             var category = _categoryRepository.GetAll();
            return category.Select(ConvertToViewModel).ToList();
        }

        public List<CategoryViewModel> GetPaginatedCategories(string categorySearch, int page, int pageSize, out int totalArticles)
        {
            var articles = _categoryRepository.GetPaginated(categorySearch, page, pageSize, out totalArticles);
            return articles.Select(ConvertToViewModel).ToList();
        }

        public CategoryViewModel? GetCategoryById(short id)
        {
            var category = _categoryRepository.GetById(id);
            return category != null ? ConvertToViewModel(category) : null;
        }

        public void AddCategory(CategoryViewModel viewModel)
        {
            var category = ConvertToEntity(viewModel);
            _categoryRepository.Insert(category);
        }

        public void UpdateCategory(CategoryViewModel updatedViewModel)
        {
            var existingCategory = _categoryRepository.GetById(updatedViewModel.CategoryId);
            if (existingCategory == null) return;

            // Update properties
            existingCategory.CategoryName = updatedViewModel.CategoryName;
            existingCategory.CategoryDesciption = updatedViewModel.CategoryDescription;
            existingCategory.ParentCategoryId = updatedViewModel.ParentCategoryId;
            existingCategory.IsActive = updatedViewModel.IsActive;

            _categoryRepository.Update(existingCategory);
        }

        public void DeleteCategory(short id)
        {
            var category = _categoryRepository.GetById(id);
            if (category != null)
            {
                _categoryRepository.Delete(category);
            }
        }

        public List<CategoryViewModel> GetActiveCategory() => _categoryRepository.GetActiveCategory().Select(ConvertToViewModel).ToList();

        public CategoryViewModel ConvertToViewModel(Category entity)
        {
            return new CategoryViewModel
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                CategoryDescription = entity.CategoryDesciption,
                ParentCategoryId = entity.ParentCategoryId,
                IsActive = entity.IsActive ?? false,
                ParentCategory = entity.ParentCategory,
                SubCategories = entity.InverseParentCategory?
                .Where(sub => (sub != null && sub.CategoryId != entity.CategoryId))
                .Select(ConvertToViewModel)
                .ToList()
            };
        }

        public Category ConvertToEntity(CategoryViewModel viewModel)
        {
            return new Category
            {
                CategoryId = viewModel.CategoryId,
                CategoryName = viewModel.CategoryName,
                CategoryDesciption = viewModel.CategoryDescription,
                ParentCategoryId = viewModel.ParentCategoryId,
                IsActive = viewModel.IsActive
            };
        }
    }
}