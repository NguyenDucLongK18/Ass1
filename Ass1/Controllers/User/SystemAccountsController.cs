using Microsoft.AspNetCore.Mvc;
using Ass1.Models;
using Ass1.ViewModels;
using Ass1.Services;
using System.Threading.Tasks;

namespace Ass1.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly UserManagerService _userManagerService;

        public SystemAccountsController(UserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 5)
        {
            int totalAccounts;
            var accountViewModels = _userManagerService.GetPaginatedAccounts(searchTerm, page, pageSize, out totalAccounts);

            // Pass pagination info to ViewBag
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalAccounts / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            return View("Index", accountViewModels);
        }

        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Account ID is missing.";
                return RedirectToAction(nameof(Index));
            }

            var accountViewModel = _userManagerService.GetAccountById(id.Value);
            if (accountViewModel == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToAction(nameof(Index));
            }

            return PartialView("_DetailsAccount", accountViewModel);
        }

        public IActionResult Create()
        {
            return PartialView("_CreateAccount", new SystemAccountViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemAccountViewModel model)
        {

            if (!_userManagerService.isUniqueEmail(model.AccountEmail))
            {
                TempData["ErrorMessage"] = "Email has already been registered, please try again!";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                model.AccountId = GenerateShortUniqueAccountId();
                _userManagerService.AddAccount(model);
                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "There was an error creating the account.";
            return PartialView("_CreateAccount", model);
        }

        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Account ID is missing.";
                return RedirectToAction(nameof(Index));
            }

            var accountViewModel = _userManagerService.GetAccountById(id.Value);
            if (accountViewModel == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToAction(nameof(Index));
            }

            return PartialView("_EditAccount", accountViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, SystemAccountViewModel model)
        {
            if (id != model.AccountId)
            {
                TempData["ErrorMessage"] = "Account ID mismatch.";
                return RedirectToAction(nameof(Index));
            }
            if (!_userManagerService.isUniqueEmail(model.AccountEmail))
            {
                TempData["ErrorMessage"] = "Email has already been registered, please try again!";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                _userManagerService.UpdateAccount(model);
                TempData["SuccessMessage"] = "Account updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "There was an error updating the account.";
            return PartialView("_EditAccount", model);
        }

        public async Task<IActionResult> Deactivate(short? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Account ID is missing.";
                return RedirectToAction(nameof(Index));
            }

            var accountViewModel = _userManagerService.GetAccountById(id.Value);
            if (accountViewModel == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToAction(nameof(Index));
            }

            return PartialView("_DeleteAccount", accountViewModel);
        }

        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveAccount(short id)
        {
            var success = _userManagerService.AccountSettings(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Account deactivated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to deactivate the account.";
            }

            return RedirectToAction(nameof(Index));
        }

        private short GenerateShortUniqueAccountId()
        {
            // Get the current timestamp in milliseconds
            var timestamp = DateTime.UtcNow.Ticks;

            // Apply a modulo operation to limit the value to a short (0 to 32,767)
            return (short)(timestamp % short.MaxValue);
        }
    }
}