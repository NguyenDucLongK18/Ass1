using Ass1.Models;
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
    }
}
