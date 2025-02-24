using Ass1.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Ass1.Repositories
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context) { }

        public List<NewsArticle> GetPaginated(bool isStaff, string newsArticleSearch, int page, int pageSize, out int totalArticles)
        {
            IQueryable<NewsArticle> query = _context.NewsArticles;

            if (!string.IsNullOrWhiteSpace(newsArticleSearch))
            {
                query = query.Where(a => a.NewsTitle.Contains(newsArticleSearch));
            }

            if (!isStaff)
            {
                query = query.Where(a => a.NewsStatus == true);
            }

            totalArticles = query.Count();

            return query
                .OrderByDescending(n => n.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<NewsArticle> ShowHistory(int userId)
        {
            return _context.NewsArticles
                .Where(a => a.CreatedById == userId || a.UpdatedById == userId) // User created OR updated
                .OrderByDescending(n => n.ModifiedDate > n.CreatedDate ? n.ModifiedDate : n.CreatedDate) // Sort by latest activity
                .ToList();
        }
    }
}
