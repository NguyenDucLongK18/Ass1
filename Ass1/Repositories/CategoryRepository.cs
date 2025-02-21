using Ass1.Models;

namespace Ass1.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
          public CategoryRepository(FunewsManagementContext context) : base(context) { }
    }
}
