using Ass1.Models;
using Ass1.Repositories;
using Ass1.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ass1.Services
{
    public class UserManagerService : IService<SystemAccount, SystemAccountViewModel>
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

        public void UpdateProfile(ProfileViewModel _model)
        {
            var user = _repository.GetById(_model.AccountId);

            user.AccountName = _model.AccountName;

            user.AccountEmail = _model.AccountEmail;

            user.AccountPassword = _model.AccountPassword;

            _repository.Update(user);
        }

        public List<SystemAccountViewModel> GetPaginatedAccounts(string newsArticleSearch, int page, int pageSize, out int totalArticles)
        {
            var articles = _repository.GetPaginated(newsArticleSearch, page, pageSize, out totalArticles);
            return articles.Select(ConvertToViewModel).ToList();
        }


        public SystemAccountViewModel? GetAccountById(short id)
        {
            var category = _repository.GetById(id);
            return category != null ? ConvertToViewModel(category) : null;
        }

        public void AddAccount(SystemAccountViewModel viewModel)
        {
            var category = ConvertToEntity(viewModel);
            _repository.Insert(category);
        }

        public void UpdateAccount(SystemAccountViewModel updatedViewModel)
        {
            var existingCategory = _repository.GetById(updatedViewModel.AccountId);
            if (existingCategory == null) return;

            // Update properties
            existingCategory.AccountName = updatedViewModel.AccountName;
            existingCategory.AccountEmail = updatedViewModel.AccountEmail;
            existingCategory.AccountRole = updatedViewModel.AccountRole;
            existingCategory.AccountPassword = updatedViewModel.AccountPassword;

            _repository.Update(existingCategory);
        }

        public void DeleteAccount(short id)
        {
            var category = _repository.GetById(id);
            if (category != null)
            {
                _repository.Delete(category);
            }
        }

        public bool AccountSettings(short id)
        {
            var account = _repository.GetById(id);

            if (account != null)
            {
                if (account.IsActive == null) // if IsActive = null, set to false (active)
                {
                    account.IsActive = false;
                }
                else
                {
                    account.IsActive = !account.IsActive;
                }
                _repository.Update(account);
                return true;
            }
            return false;
        }


        public void ReactivateAccount(short id)
        {
            var category = _repository.GetById(id);
            if (category != null)
            {
                category.IsActive = true;

                _repository.Update(category);
            }
        }

        public SystemAccountViewModel GetUserByEmail(string email)
        {
            return ConvertToViewModel(_repository.GetByEmail(email));
        }

        public List<SystemAccountViewModel> GetAdmin()
        {
            return _repository.GetByRole(0).Select(ConvertToViewModel).ToList();
        }

        public bool isUniqueEmail(string email)
        {
            var account = GetUserByEmail(email);

            return account == null;
        }

        public SystemAccountViewModel ConvertToViewModel(SystemAccount entity)
        {
            return new SystemAccountViewModel
            {
                AccountId = entity.AccountId,
                AccountName = entity.AccountName,
                AccountEmail = entity.AccountEmail,
                AccountRole = entity.AccountRole,
                AccountPassword = entity.AccountPassword,
                IsActive = entity.IsActive,
            };
        }

        public SystemAccount ConvertToEntity(SystemAccountViewModel viewModel)
        {
            return new SystemAccount
            {
                AccountId = viewModel.AccountId,
                AccountName = viewModel.AccountName,
                AccountEmail = viewModel.AccountEmail,
                AccountRole = viewModel.AccountRole,
                AccountPassword = viewModel.AccountPassword,
                IsActive = viewModel.IsActive,
            };
        }
    }
}
