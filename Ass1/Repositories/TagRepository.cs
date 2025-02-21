using Ass1.Models;

namespace Ass1.Repositories
{
    public class TagRepository : GenericRepository<Tag>
    {
        public TagRepository(FunewsManagementContext context) : base(context)
        {
        }
        public List<Tag> GetTagsByIds(List<int> tagIds)
        {
            return _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
        }
    }
}
