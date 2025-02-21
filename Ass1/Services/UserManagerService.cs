using Ass1.Models;
using Ass1.Repositories;

namespace Ass1.Services
{
    public class UserManagerService
    {
        private SystemAccountRepository _repository;

        public UserManagerService(SystemAccountRepository accountRepository)
        {
            _repository = accountRepository;
        }

        public async Task<SystemAccount?> LoginAsync(string email, string password)
        {
            return await _repository.AuthenticateAsync(email, password);
        }

        public SystemAccount? GetById(short id) => _repository.GetById(id);

        public List<SystemAccount> GetSystemAccounts() {
            return _repository.GetAll();
        }
    }
}
