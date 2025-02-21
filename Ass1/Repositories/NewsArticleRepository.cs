using Ass1.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Ass1.Repositories
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context) { }

        public List<NewsArticle> GetPaginated(bool isStaff, int page, int pageSize, out int totalArticles)
        {
            totalArticles = _context.NewsArticles.Count();
            if (isStaff)
            {
                return _context.NewsArticles
                .OrderByDescending(n => n.CreatedDate) // Sort by newest first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            }
            return _context.NewsArticles
                .Where(a => a.NewsStatus == true)
                .OrderByDescending(n => n.CreatedDate) // Sort by newest first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
