using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ass1.Services;
using Ass1.ViewModels;
using System.Drawing.Printing;
using Ass1.Models;

namespace Ass1.Controllers.Categories
{
    public class CategoriesController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public IActionResult Index(string searchTerm = "", int page = 1, int pageSize = 6)
        {
            int totalArticles;

            var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
            bool isStaff = userRoleClaim != null && userRoleClaim.Value == "1";

            var articleViewModels = _categoryService.GetPaginatedCategories(searchTerm, page, pageSize, out totalArticles);

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalArticles / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            // Display success or error messages
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(articleViewModels);
        }

        // GET: Categories/Details/5
        public IActionResult Details(short? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            return PartialView("_DetailsCategory", category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetAllCategories(), "CategoryId", "CategoryName");
            return PartialView("_CreateCategory");
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.AddCategory(categoryViewModel);
                    TempData["SuccessMessage"] = "Category added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the category. Please try again.";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetAllCategories(), "CategoryId", "CategoryName", categoryViewModel.ParentCategoryId);
            return PartialView("_CreateCategory");
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(short? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetAllCategories(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return PartialView("_EditCategory", category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, CategoryViewModel categoryViewModel)
        {
            if (id != categoryViewModel.CategoryId)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(categoryViewModel);
                    TempData["SuccessMessage"] = "Category edited successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the category. Please try again.";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetAllCategories(), "CategoryId", "CategoryName", categoryViewModel.ParentCategoryId);
            return PartialView("_EditCategory");
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(short? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            return PartialView("_DeleteCategory", category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(short id)
        {
            try
            {
                _categoryService.DeleteCategory(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Category deletion failed! Make sure the category is not associated with any news, then try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}