using Ass1.Models;

namespace Ass1.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(FunewsManagementContext context) : base(context) { }

        public List<Category> GetActiveCategory()
        {
            return _context.Categories.Where(a => a.IsActive == true).ToList();
        }

        public List<Category> GetPaginated(string categorySearch, int page, int pageSize, out int totalArticles)
        {
            IQueryable<Category> query = _context.Categories;

            if (!string.IsNullOrWhiteSpace(categorySearch))
            {
                query = query.Where(a => a.CategoryName.Contains(categorySearch));
            }

            totalArticles = query.Count();

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
