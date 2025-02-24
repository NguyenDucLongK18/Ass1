using Ass1.Models;
using Ass1.Services;
using Ass1.Utils;
using Ass1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ass1.Controllers
{
    public class NewsArticleController : Controller
    {
        private readonly NewsArticleService _newsArticleService;
        private readonly TagService _tagService;
        private readonly CategoryService _categoryService;

        public NewsArticleController(NewsArticleService newsArticleService, TagService tagService, CategoryService categoryService)
        {
            _newsArticleService = newsArticleService;
            _tagService = tagService;
            _categoryService = categoryService;
        }

        public IActionResult GetNewsArticles(string searchTerm = "", int page = 1, int pageSize = 2)
        {
            int totalArticles;

            var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
            bool isStaff = userRoleClaim != null && userRoleClaim.Value == "1";

            var articleViewModels = _newsArticleService.GetPaginatedNews(isStaff, searchTerm, page, pageSize, out totalArticles);

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalArticles / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            // Display success or error messages
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return PartialView("_GetNewsArticles", articleViewModels);
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Create()
        {
            var newsViewModel = new NewsArticleViewModel();
            ViewBag.Categories = _categoryService.GetActiveCategory();
            ViewBag.Tags = _tagService.GetAllTags();
            return PartialView("_CreateNewsArticle", newsViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public IActionResult Create(NewsArticleViewModel articleViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetActiveCategory();
                ViewBag.Tags = _tagService.GetAllTags();
                TempData["ErrorMessage"] = "There was an error creating the news article. Please check the input fields.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                articleViewModel.CreatedById = short.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                // Save to database
                _newsArticleService.CreateNewsArticle(articleViewModel);

                TempData["SuccessMessage"] = "News article created successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the news article. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var newsViewModel = _newsArticleService.GetNewsArticle(id);

            if (newsViewModel == null)
            {
                TempData["ErrorMessage"] = "News article not found!";
                return RedirectToAction(nameof(GetNewsArticles));
            }

            return PartialView("_DetailsNewsArticle", newsViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var newsViewModel = _newsArticleService.GetNewsArticle(id);

            if (newsViewModel == null)
            {
                TempData["ErrorMessage"] = "News article not found!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = _categoryService.GetActiveCategory();
            ViewBag.Tags = _tagService.GetAllTags();

            return PartialView("_EditNewsArticle", newsViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NewsArticleViewModel newViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetActiveCategory();
                ViewBag.Tags = _tagService.GetAllTags();
                TempData["ErrorMessage"] = "There was an error updating the news article. Please check the input fields.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                newViewModel.UpdatedById = short.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var oldViewModel = _newsArticleService.GetNewsArticle(newViewModel.NewsArticleId);
                if (oldViewModel == null)
                {
                    TempData["ErrorMessage"] = "News article not found!";
                    return RedirectToAction("Index", "Home");
                }

                _newsArticleService.UpdateNewsArticle(oldViewModel, newViewModel);

                TempData["SuccessMessage"] = "News article edited successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the news article. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var existingArticle = _newsArticleService.GetNewsArticle(id);
            if (existingArticle == null)
            {
                TempData["ErrorMessage"] = "News article not found!";
                return RedirectToAction(nameof(GetNewsArticles));
            }

            return PartialView("_DeleteNewsArticle", existingArticle);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(NewsArticleViewModel model)
        {
            var existingArticle = _newsArticleService.GetNewsArticle(model.NewsArticleId);
            if (existingArticle == null)
            {
                TempData["ErrorMessage"] = "News article not found!";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                _newsArticleService.DeleteNewsArticle(existingArticle);
                TempData["SuccessMessage"] = "News article deleted successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the news article. Please try again.";
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "0")]
        public IActionResult StatisticReport(DateTime? startDate, DateTime? endDate)
        {
            List<NewsStatisticViewModel> report = new List<NewsStatisticViewModel>();

            // If both dates are provided, get the statistics.
            if (startDate.HasValue && endDate.HasValue)
            {
                report = _newsArticleService.GetNewsStatistics(startDate.Value, endDate.Value);
            }

            // Pass the dates back to the view for re-populating the filter fields.
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(report);
        }

        [Authorize(Roles = "1")]
        public IActionResult ViewHistory(short id)
        {
            var viewModels = _newsArticleService.DisplayHistory(id);
            ViewBag.CurrentUserId = id;
            return View(viewModels);
        }
    }
}