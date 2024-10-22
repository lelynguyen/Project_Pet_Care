using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PetCare.Controllers
{
    public class AccountsController : Controller
    {
        private readonly PetCareContext _context;

        public AccountsController(PetCareContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public IActionResult Index()
        {
            return View();
        }

        // Action để xử lý yêu cầu AJAX
        [HttpGet]
        public async Task<IActionResult> GetAccounts(string searchString, string accountType)
        {
            var accounts = _context.Accounts.AsQueryable();

            // Lọc theo loại tài khoản
            if (!string.IsNullOrEmpty(accountType))
            {
                if (accountType == "Employee")
                {
                    accounts = accounts.Where(a => a.Employees.Any());
                }
                else if (accountType == "Customer")
                {
                    accounts = accounts.Where(a => a.Customers.Any());
                }
            }

            // Tìm kiếm theo email hoặc số điện thoại
            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(a => a.Email.Contains(searchString) || a.PhoneNumber.Contains(searchString));
            }

            var accountList = await accounts.ToListAsync();

            return PartialView("_AccountsList", accountList);
        }



        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa tài khoản thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
