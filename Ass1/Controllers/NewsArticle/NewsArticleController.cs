using Ass1.Models;
using Ass1.Services;
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

        public IActionResult GetNewsArticles(int page = 1, int pageSize = 2)
        {
            int totalArticles;

            var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);

            bool isStaff = userRoleClaim != null && userRoleClaim.Value == "1";

            var articles = _newsArticleService.GetPaginatedNews(isStaff, page, pageSize, out totalArticles);

            var articleViewModels = articles.Select(article => new NewsArticleViewModel
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                SelectedCategoryId = article.CategoryId,
                SelectedCategoryName = article.Category?.CategoryName,
                NewsStatus = article.NewsStatus ?? false,
                CreatedBy = article.CreatedBy?.AccountName,
                UpdatedBy = article.UpdatedBy?.AccountName,
                ModifiedDate = article.ModifiedDate,
                SelectedTagNames = article.Tags.Select(t => t.TagName).ToList()
            }).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalArticles / pageSize);
            ViewBag.CurrentPage = page;

            return PartialView("_GetNewsArticles", articleViewModels);
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Create()
        {
            var newsViewModel = new NewsArticleViewModel{
                Tags = _tagService.GetAllTags(),
                Categories = _categoryService.GetAllCategories(),
            };
            return View(newsViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public IActionResult Create(NewsArticleViewModel articleViewModel)
        {
            if (!ModelState.IsValid)
            {
                articleViewModel.Tags = _tagService.GetAllTags();
                articleViewModel.Categories = _categoryService.GetAllCategories();
                return View(articleViewModel);
            }
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            short createdById = short.Parse(userIdClaim.Value);

            var selectedTag = _tagService.GetByIds(articleViewModel.SelectedTagIds);
            // convert data to viewModel
            var newsArticle = new NewsArticle
            {
                NewsTitle = articleViewModel.NewsTitle,
                Headline = articleViewModel.Headline,
                CreatedDate = DateTime.UtcNow,
                NewsContent = articleViewModel.NewsContent,
                NewsSource = articleViewModel.NewsSource,
                CategoryId = articleViewModel.SelectedCategoryId,
                NewsStatus = articleViewModel.NewsStatus,
                CreatedById = createdById,
                Tags = selectedTag ?? new List<Tag>()
            };

            // Save to database
            _newsArticleService.CreateNewsArticle(newsArticle);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Details(string id)
        {
            var newsDetail = _newsArticleService.GetNewsArticle(id);

            if (newsDetail == null)
            {
                return NotFound(); // Return 404 if article doesn't exist
            }

            string updatedName = null;

            if (newsDetail.UpdatedBy != null)
            {
                updatedName = newsDetail.UpdatedBy.AccountName;
            }

            var newsViewModel = new NewsArticleViewModel
            {
                NewsArticleId = newsDetail.NewsArticleId,
                NewsTitle = newsDetail.NewsTitle,
                Headline = newsDetail.Headline,
                NewsContent = newsDetail.NewsContent,
                NewsSource = newsDetail.NewsSource,
                CreatedDate = newsDetail.CreatedDate,
                NewsStatus = newsDetail.NewsStatus ?? false,
                CreatedBy = newsDetail.CreatedBy.AccountName,
                ModifiedDate = newsDetail.ModifiedDate ?? null,
                UpdatedBy = updatedName ?? null,
                Tags = (List<Tag>)newsDetail.Tags
            };

            return View(newsViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var newsArticle = _newsArticleService.GetNewsArticle(id);

            if (newsArticle == null)
            {
                return NotFound(); // Return 404 if article doesn't exist
            }

            var newsViewModel = new NewsArticleViewModel
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CreatedDate = newsArticle.CreatedDate,
                NewsStatus = newsArticle.NewsStatus ?? false,
                SelectedCategoryId = newsArticle.CategoryId,
                SelectedTagIds = newsArticle.Tags?.Select(t => t.TagId).ToList()
            };

            ViewBag.Categories = _categoryService.GetAllCategories(); // For dropdown selection
            ViewBag.Tags = _tagService.GetAllTags(); // For tag selection

            return View(newsViewModel);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NewsArticleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetAllCategories();
                ViewBag.Tags = _tagService.GetAllTags();
                return View(model);
            }

            var existingArticle = _newsArticleService.GetNewsArticle(model.NewsArticleId);
            if (existingArticle == null)
            {
                return NotFound();
            }

            var selectedTag = _tagService.GetByIds(model.SelectedTagIds);

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            short updatedById = short.Parse(userIdClaim.Value);

            existingArticle.NewsTitle = model.NewsTitle;
            existingArticle.Headline = model.Headline;
            existingArticle.NewsContent = model.NewsContent;
            existingArticle.NewsSource = model.NewsSource;
            existingArticle.NewsStatus = model.NewsStatus;
            existingArticle.CategoryId = model.SelectedCategoryId;
            existingArticle.UpdatedById = updatedById;
            existingArticle.ModifiedDate = DateTime.UtcNow;

            existingArticle.Tags.Clear();

            // Update tags
            existingArticle.Tags = selectedTag ?? new List<Tag>();

            _newsArticleService.UpdateNewsArticle(existingArticle);

            return RedirectToAction("Details", new { id = model.NewsArticleId });
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var existingArticle = _newsArticleService.GetNewsArticle(id);
            if (existingArticle == null)
            {
                return NotFound();
            }
            existingArticle.Tags.Clear();

            _newsArticleService.DeleteNewsArticle(existingArticle);

            return RedirectToAction("Index", "Home");
        }
    }
}
