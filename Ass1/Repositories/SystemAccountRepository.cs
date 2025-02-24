using Ass1.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace Ass1.Repositories
{
    public class SystemAccountRepository : GenericRepository<SystemAccount>
    {
        public SystemAccountRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        }

        public List<SystemAccount> GetPaginated(string? findQuery, int page, int pageSize, out int totalArticles)
        {
            var query = _context.SystemAccounts.AsQueryable();

            // Filter by name or email (if search query is provided)
            if (!string.IsNullOrEmpty(findQuery))
            {
                query = query.Where(a => a.AccountName.Contains(findQuery) || a.AccountEmail.Contains(findQuery));
            }

            // Get the total count before pagination
            totalArticles = query.Count();

            // Apply pagination
            return query
                .OrderBy(a => a.AccountId) // Ensure a stable sort
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public SystemAccount GetByEmail(string email) => _context.SystemAccounts.FirstOrDefault(a => a.AccountEmail == email);

        public List<SystemAccount> GetByRole(int roleId) => _context.SystemAccounts.Where(a => a.AccountRole == roleId).ToList();
    }
}
